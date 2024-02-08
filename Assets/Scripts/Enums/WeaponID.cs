namespace ZombiesShooter
{
    public enum WeaponID
    {
        NONE = 0,

        RANGED_FIRST      = 1,
        PISTOL_1          = RANGED_FIRST,
        PISTOL_2          = 2,
        PISTOL_3          = 3,
        AUTOMATIC_RIFLE_1 = 21,
        AUTOMATIC_RIFLE_2 = 22,
        AUTOMATIC_RIFLE_3 = 23,
        MACHINE_GUN_1     = 41,
        MACHINE_GUN_2     = 42,
        MACHINE_GUN_3     = 43,
        SNIPER_RIFLE_1    = 61,
        SNIPER_RIFLE_2    = 62,
        SNIPER_RIFLE_3    = 63,
        RANGED_LAST       = 500,

        MELEE_FIRST = 501,
        HANDS       = MELEE_LAST,
        MELEE_LAST  = 1000,
    }
}