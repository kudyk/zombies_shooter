using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ZombiesShooter
{
    // TODO: Don't convert to IJobEntity, remove comment before release!
    [UpdateInGroup(typeof(WeaponAssignerSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial struct RangedWeaponAssignerSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WeaponViewConfig>();
            state.RequireForUpdate<RangedWeaponConfigsBlobReference>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<WeaponAssigner, RangedWeapon>().Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            RangedWeaponConfigsBlobReference rangedWeaponConfigs = SystemAPI.GetSingleton<RangedWeaponConfigsBlobReference>();
            DynamicBuffer<WeaponViewConfig>  weaponViewConfigs   = SystemAPI.GetSingletonBuffer<WeaponViewConfig>();

            EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (Entity entity in entityQuery.ToEntityArray(Allocator.Temp))
            {
                RefRW<WeaponAssigner> weaponAssigner = SystemAPI.GetComponentRW<WeaponAssigner>(entity);
                if (weaponAssigner.ValueRO.weaponChangeTo == WeaponID.NONE)
                    continue;

                if (!WeaponIDHelper.IsRangedWeapon(weaponAssigner.ValueRO.weaponChangeTo))
                    continue;

                RangedWeaponConfig rangedWeaponConfig = ConfigsHelper.TryGetRangedWeaponConfig(in rangedWeaponConfigs.blobReference, weaponAssigner.ValueRO.weaponChangeTo);
                if (rangedWeaponConfig.weaponID == WeaponID.NONE)
                    continue;

                RefRW<RangedWeapon> rangedWeapon = SystemAPI.GetComponentRW<RangedWeapon>(entity);
                rangedWeapon.ValueRW.weaponConfig = rangedWeaponConfig;

                Entity oldView = rangedWeapon.ValueRO.weaponView;
                if (oldView != Entity.Null)
                    commandBuffer.DestroyEntity(oldView);

                WeaponViewConfig weaponViewConfig = ConfigsHelper.TryGetWeaponViewConfig(in weaponViewConfigs, weaponAssigner.ValueRO.weaponChangeTo);
                if (weaponViewConfig.weaponID != WeaponID.NONE)
                {
                    Entity viewInstance = InstantiationHelper.InstantiateInContainer(weaponViewConfig.entityPrefab, weaponAssigner.ValueRO.weaponContainer, ref state);

                    weaponAssigner = SystemAPI.GetComponentRW<WeaponAssigner>(entity);
                    rangedWeapon   = SystemAPI.GetComponentRW<RangedWeapon>(entity);

                    rangedWeapon.ValueRW.weaponView = viewInstance;
                }

                weaponAssigner.ValueRW.weaponChangeTo = WeaponID.NONE;
            }
        }
    }
}