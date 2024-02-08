using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ZombiesShooter
{
    public class MeleeWeaponConfigsAuthoring : MonoBehaviour
    {
        public MeleeWeaponConfig[] weaponConfigs;
    }

    public class MeleeWeaponConfigsBaker : Baker<MeleeWeaponConfigsAuthoring>
    {
        public override void Bake(MeleeWeaponConfigsAuthoring authoring)
        {
            BlobBuilder builder = new BlobBuilder(Allocator.Temp);

            ref MeleeWeaponConfigs configs = ref builder.ConstructRoot<MeleeWeaponConfigs>();

            BlobBuilderArray<MeleeWeaponConfig> arrayBuilder = builder.Allocate(
                ref configs.configs,
                authoring.weaponConfigs.Length
            );

            for (int i = 0; i < authoring.weaponConfigs.Length; i++)
                arrayBuilder[i] = authoring.weaponConfigs[i];

            BlobAssetReference<MeleeWeaponConfigs> blobReference
                = builder.CreateBlobAssetReference<MeleeWeaponConfigs>(Allocator.Persistent);

            builder.Dispose();

            AddBlobAsset(ref blobReference, out _);
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new MeleeWeaponConfigsBlobReference { blobReference = blobReference });
        }
    }
}