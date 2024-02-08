using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ZombiesShooter
{
    // TODO: Don't convert to IJobEntity, remove comment before release!
    [UpdateInGroup(typeof(WeaponAssignerSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial struct MeleeWeaponAssignerSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WeaponViewConfig>();
            state.RequireForUpdate<MeleeWeaponConfigsBlobReference>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<WeaponAssigner, MeleeWeapon>().Build(ref state);
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            MeleeWeaponConfigsBlobReference meleeWeaponConfigs = SystemAPI.GetSingleton<MeleeWeaponConfigsBlobReference>();
            DynamicBuffer<WeaponViewConfig> weaponViewConfigs  = SystemAPI.GetSingletonBuffer<WeaponViewConfig>();

            EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (Entity entity in entityQuery.ToEntityArray(Allocator.Temp))
            {
                RefRW<WeaponAssigner> weaponAssigner = SystemAPI.GetComponentRW<WeaponAssigner>(entity);
                if (weaponAssigner.ValueRO.weaponChangeTo == WeaponID.NONE)
                    continue;

                if (!WeaponIDHelper.IsMeleeWeapon(weaponAssigner.ValueRO.weaponChangeTo))
                    continue;

                MeleeWeaponConfig meleeWeaponConfig = ConfigsHelper.TryGetMeleeWeaponConfig(in meleeWeaponConfigs.blobReference, weaponAssigner.ValueRO.weaponChangeTo);
                if (meleeWeaponConfig.weaponID == WeaponID.NONE)
                    continue;

                RefRW<MeleeWeapon> meleeWeapon = SystemAPI.GetComponentRW<MeleeWeapon>(entity);
                meleeWeapon.ValueRW.weaponConfig = meleeWeaponConfig;

                Entity oldView = meleeWeapon.ValueRO.weaponView;
                if (oldView != Entity.Null)
                    commandBuffer.DestroyEntity(oldView);

                WeaponViewConfig weaponViewConfig = ConfigsHelper.TryGetWeaponViewConfig(in weaponViewConfigs, weaponAssigner.ValueRO.weaponChangeTo);
                if (weaponViewConfig.weaponID != WeaponID.NONE)
                {
                    Entity viewInstance = InstantiationHelper.InstantiateInContainer(weaponViewConfig.entityPrefab, weaponAssigner.ValueRO.weaponContainer, ref state);

                    weaponAssigner = SystemAPI.GetComponentRW<WeaponAssigner>(entity);
                    meleeWeapon    = SystemAPI.GetComponentRW<MeleeWeapon>(entity);

                    meleeWeapon.ValueRW.weaponView = viewInstance;
                }

                weaponAssigner.ValueRW.weaponChangeTo = WeaponID.NONE;
            }
        }
    }
}