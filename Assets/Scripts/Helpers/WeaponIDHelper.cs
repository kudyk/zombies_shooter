namespace ZombiesShooter
{
    public struct WeaponIDHelper
    {
        public static bool IsRangedWeapon(WeaponID weaponID)
        {
            return weaponID is >= WeaponID.RANGED_FIRST and <= WeaponID.RANGED_LAST;
        }

        public static bool IsMeleeWeapon(WeaponID weaponID)
        {
            return weaponID is >= WeaponID.MELEE_FIRST and <= WeaponID.MELEE_LAST;
        }
    }
}