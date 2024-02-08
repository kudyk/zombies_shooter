using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ZombiesShooter
{
    [RequireMatchingQueriesForUpdate]
    public partial struct EnemyTargetsFinderSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemyTag>();
            state.RequireForUpdate<MeleeAutoAttacker>();
            state.RequireForUpdate<MovableOnTargetByImpulse>();
            state.RequireForUpdate<RotatableOnTarget>();
            state.RequireForUpdate<LocalToWorld>();
            state.RequireForUpdate<EnemyTargetsFinder>();

            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<CharacterTag, LocalToWorld>().Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EnemyTargetsFinder            targetsFinder      = SystemAPI.GetSingleton<EnemyTargetsFinder>();
            ComponentLookup<LocalToWorld> localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            NativeArray<Entity>           targetsArray       = entityQuery.ToEntityArray(Allocator.TempJob);

            state.Dependency = new EnemyTargetsSearchJob()
            {
                targetsFinder      = targetsFinder,
                localToWorldLookup = localToWorldLookup,
                targetsArray       = targetsArray
            }.ScheduleParallel(state.Dependency);

            targetsArray.Dispose(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct EnemyTargetsSearchJob : IJobEntity
    {
        [ReadOnly] public EnemyTargetsFinder            targetsFinder;
        [ReadOnly] public ComponentLookup<LocalToWorld> localToWorldLookup;
        [ReadOnly] public NativeArray<Entity>           targetsArray;

        private void Execute(in EnemyTag _, ref MeleeAutoAttacker autoAttacker, ref MovableOnTargetByImpulse movableOnTarget, ref RotatableOnTarget rotatableOnTarget, in LocalToWorld localToWorld)
        {
            int   indexOfNearest    = ComputeDistancesAndGetNearest(localToWorld.Position, out NativeArray<float> distancesToTargets);
            float distanceToNearest = distancesToTargets[indexOfNearest];

            if (distanceToNearest > targetsFinder.minDistanceForLinkSQ)
            {
                if (movableOnTarget.targetEntity != Entity.Null)
                    LinkTargetOnComponents(ref autoAttacker, ref movableOnTarget, ref rotatableOnTarget, Entity.Null);

                return;
            }

            Entity nearestEntity = targetsArray[indexOfNearest];
            if (movableOnTarget.targetEntity == Entity.Null)
            {
                LinkTargetOnComponents(ref autoAttacker, ref movableOnTarget, ref rotatableOnTarget, nearestEntity);
                return;
            }

            int indexOfCurrent = targetsArray.IndexOf(movableOnTarget.targetEntity);
            if (indexOfCurrent == indexOfNearest)
                return;

            if (distanceToNearest <= targetsFinder.minDistanceDifferenceForResetSQ && distancesToTargets[indexOfCurrent] > targetsFinder.minDistanceDifferenceForResetSQ)
                LinkTargetOnComponents(ref autoAttacker, ref movableOnTarget, ref rotatableOnTarget, nearestEntity);
        }

        private int ComputeDistancesAndGetNearest(in float3 selfPosition, out NativeArray<float> distancesToTargets)
        {
            distancesToTargets = new NativeArray<float>(targetsArray.Length, Allocator.Temp);
            int indexOfNearest = 0;

            for (int i = 0; i < targetsArray.Length; i++)
            {
                float3 targetPosition    = localToWorldLookup[targetsArray[i]].Position;
                float  distanceBetweenSQ = math.distancesq(selfPosition, targetPosition);

                if (distanceBetweenSQ < distancesToTargets[indexOfNearest])
                    indexOfNearest = i;

                distancesToTargets[i] = distanceBetweenSQ;
            }

            return indexOfNearest;
        }

        private void LinkTargetOnComponents(ref MeleeAutoAttacker autoAttacker, ref MovableOnTargetByImpulse movableOnTarget, ref RotatableOnTarget rotatableOnTarget, Entity target)
        {
            autoAttacker.targetEntity      = target;
            movableOnTarget.targetEntity   = target;
            rotatableOnTarget.targetEntity = target;
        }
    }
}