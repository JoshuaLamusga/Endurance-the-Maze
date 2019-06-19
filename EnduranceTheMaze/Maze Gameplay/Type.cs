namespace EnduranceTheMaze
{
    /// <summary>
    /// Represents a block's identity.
    /// </summary>
    public enum Type
    {
        Actor = 0,
        Belt = 1,
        Checkpoint = 2,
        Coin = 3,
        Crate = 4,
        CrateHole = 5,
        Enemy = 6,
        EAuto = 7,
        ELight = 8,
        EPusher = 9,
        Finish = 10,
        Floor = 11,
        Filter = 12,
        Freeze = 13,
        Gate = 14,
        Goal = 15,
        Health = 16,
        Ice = 17,
        Key = 18,
        Lock = 19,
        Message = 20,
        MultiWay = 21,
        Panel = 22,
        Spawner = 23,
        Spike = 24,
        Stairs = 25,
        Teleporter = 26,
        Thaw = 27,
        Wall = 28,
        /* When adding new objects, the order of types is changed and as
         * they are referenced numerically in saved levels, the levels will be
         * corrupted unless new type entries are located at the end here.
         * TODO: Move new types upwards.
        */
        Click = 29,
        Rotate = 30,
        CrateBroken = 31, //Not an editor object.
        Turret = 32,
        TurretBullet = 33, //Not an editor object.
        Mirror = 34,
        CoinLock = 35,
        LaserActuator = 36
    }
}
