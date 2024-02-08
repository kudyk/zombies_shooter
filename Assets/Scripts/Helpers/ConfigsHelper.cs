using Unity.Entities;

namespace ZombiesShooter
{
    public struct ConfigsHelper
    {
        public static RangedWeaponConfig TryGetRangedWeaponConfig(in BlobAssetReference<RangedWeaponConfigs> reference, WeaponID weaponID)
        {
            for (int i = 0; i < reference.Value.configs.Length; i++)
            {
                RangedWeaponConfig rangedWeaponConfig = reference.Value.configs[i];

                if (rangedWeaponConfig.weaponID == weaponID)
                    return rangedWeaponConfig;
            }

            return default;
        }

        public static MeleeWeaponConfig TryGetMeleeWeaponConfig(in BlobAssetReference<MeleeWeaponConfigs> reference, WeaponID weaponID)
        {
            for (int i = 0; i < reference.Value.configs.Length; i++)
            {
                MeleeWeaponConfig rangedWeaponConfig = reference.Value.configs[i];

                if (rangedWeaponConfig.weaponID == weaponID)
                    return rangedWeaponConfig;
            }

            return default;
        }

        public static WeaponViewConfig TryGetWeaponViewConfig(in DynamicBuffer<WeaponViewConfig> dynamicBuffer, WeaponID targetID)
        {
            foreach (WeaponViewConfig weaponViewConfig in dynamicBuffer)
            {
                if (weaponViewConfig.weaponID == targetID)
                    return weaponViewConfig;
            }

            return default;
        }
    }
}