namespace EnduranceTheMaze
{
    /// <summary>
    /// Represents a block's identity.
    /// </summary>
    public enum Type
    {
        Actor,
        Belt,
        Checkpoint,
        Coin,
        Crate,
        CrateHole,
        Enemy,
        EAuto,
        ELight,
        EPusher,
        Finish,
        Floor,
        Filter,
        Freeze,
        Gate,
        Goal,
        Health,
        Ice,
        Key,
        Lock,
        Message,
        MultiWay,
        Panel,
        Spawner,
        Spike,
        Stairs,
        Teleporter,
        Thaw,
        Wall,
        /* When adding new objects, the order of types is changed and as
         * they are referenced numerically in saved levels, the levels will be
         * corrupted unless new type entries are located at the end here.
         * TODO: Move new types upwards.
        */
        Click,
        Rotate,
        CrateBroken, //Not an editor object.
        Turret,
        TurretBullet, //Not an editor object.
        Mirror,
        CoinLock
    }
}
