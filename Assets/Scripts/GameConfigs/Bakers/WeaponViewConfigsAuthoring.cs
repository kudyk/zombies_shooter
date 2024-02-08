using System;
using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class WeaponViewConfigsAuthoring : MonoBehaviour
    {
        public WeaponViewConfig[] viewConfigs;

        [Serializable]
        public struct WeaponViewConfig
        {
            public WeaponID   weaponID;
            public GameObject entityPrefab;
        }
    }

    public class WeaponViewConfigsBaker : Baker<WeaponViewConfigsAuthoring>
    {
        public override void Bake(WeaponViewConfigsAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.None);
            AddBuffer<WeaponViewConfig>(currentEntity);

            foreach (WeaponViewConfigsAuthoring.WeaponViewConfig setting in authoring.viewConfigs)
            {
                AppendToBuffer(currentEntity, new WeaponViewConfig()
                {
                    entityPrefab = GetEntity(setting.entityPrefab, TransformUsageFlags.Dynamic),
                    weaponID     = setting.weaponID,
                });
            }
        }
    }
}