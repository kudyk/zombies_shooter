using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class RangedWeaponConfigsAuthoring : MonoBehaviour
    {
        public RangedWeaponConfig[] weaponConfigs;
    }

    public class RangedWeaponConfigsBaker : Baker<RangedWeaponConfigsAuthoring>
    {
        public override void Bake(RangedWeaponConfigsAuthoring authoring)
        {
            BlobBuilder builder = new BlobBuilder(Allocator.Temp);

            ref RangedWeaponConfigs configs = ref builder.ConstructRoot<RangedWeaponConfigs>();

            BlobBuilderArray<RangedWeaponConfig> arrayBuilder = builder.Allocate(
                ref configs.configs,
                authoring.weaponConfigs.Length
            );

            for (int i = 0; i < authoring.weaponConfigs.Length; i++)
                arrayBuilder[i] = authoring.weaponConfigs[i];

            BlobAssetReference<RangedWeaponConfigs> blobReference
                = builder.CreateBlobAssetReference<RangedWeaponConfigs>(Allocator.Persistent);

            builder.Dispose();

            AddBlobAsset(ref blobReference, out _);
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new RangedWeaponConfigsBlobReference { blobReference = blobReference });
        }
    }
}