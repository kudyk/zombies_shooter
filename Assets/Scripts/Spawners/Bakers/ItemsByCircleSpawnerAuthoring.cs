using System;
using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class ItemsByCircleSpawnerAuthoring : MonoBehaviour
    {
        public SpawnSettings[] spawnSettings;

        [Serializable]
        public struct SpawnSettings
        {
            public GameObject itemPrefab;
            public int        spawnCount;
            public float      positionY;
            public float      startAngle;
            public float      startRadius;
            public float      angleStep;
            public float      radiusStep;
        }
    }

    public class EnemiesSpawnerBaker : Baker<ItemsByCircleSpawnerAuthoring>
    {
        public override void Bake(ItemsByCircleSpawnerAuthoring authoring)
        {
            Entity currentEntity = GetEntity(TransformUsageFlags.WorldSpace);

            AddBuffer<ItemsByCircleSpawner>(currentEntity);
            foreach (var setting in authoring.spawnSettings)
            {
                AppendToBuffer(currentEntity, new ItemsByCircleSpawner()
                {
                    itemPrefab  = GetEntity(setting.itemPrefab, TransformUsageFlags.Dynamic),
                    spawnCount  = setting.spawnCount,
                    positionY   = setting.positionY,
                    startAngle  = setting.startAngle,
                    startRadius = setting.startRadius,
                    angleStep   = setting.angleStep,
                    radiusStep  = setting.radiusStep,
                });
            }
        }
    }
}