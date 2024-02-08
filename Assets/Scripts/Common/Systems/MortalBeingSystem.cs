using Unity.Burst;
using Unity.Entities;

namespace ZombiesShooter
{
    public partial struct MortalBeingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MortalBeing>();
            state.RequireForUpdate<MortalBeingHealthParams>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new DamageApplyingJob().ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct DamageApplyingJob : IJobEntity
    {
        private void Execute(ref MortalBeing mortalBeing, in MortalBeingHealthParams healthParams)
        {
            if (mortalBeing.currentHealth <= 0)
                return;

            if (mortalBeing.damageImpactQueue.Length == 0)
                return;

            foreach (int damage in mortalBeing.damageImpactQueue)
            {
                if (mortalBeing.currentHealth <= damage)
                {
                    mortalBeing.currentHealth = healthParams.minHealth;
                    mortalBeing.damageImpactQueue.Clear();
                    break;
                }

                mortalBeing.currentHealth -= damage;
            }

            mortalBeing.damageImpactQueue.Clear();
        }
    }
}