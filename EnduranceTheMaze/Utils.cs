using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace EnduranceTheMaze
{
    /// <summary>Contains useful tools.</summary>
    public static class Utils
    {
        public static Random rng { get; private set; }
        public static Game game;

        /// <summary>
        /// Sets the Random instance.
        /// </summary>
        static Utils()
        {
            rng = new Random();
        }

        /// <summary>
        /// Returns the next direction to the one given.
        /// </summary>
        /// <param name="dir">The given direction.</param>
        public static Dir DirNextAll(Dir dir)
        {
            switch (dir)
            {
                case Dir.Right:
                    return Dir.DownRight;
                case Dir.DownRight:
                    return Dir.Down;
                case Dir.Down:
                    return Dir.DownLeft;
                case Dir.DownLeft:
                    return Dir.Left;
                case Dir.Left:
                    return Dir.UpLeft;
                case Dir.UpLeft:
                    return Dir.Up;
                case Dir.Up:
                    return Dir.UpRight;
                default:
                    return Dir.Right;
            }
        }

        /// <summary>
        /// Returns the previous direction to the one given.
        /// </summary>
        /// <param name="dir">The given direction.</param>
        public static Dir DirPrevAll(Dir dir)
        {
            switch (dir)
            {
                case Dir.Right:
                    return Dir.UpRight;
                case Dir.UpRight:
                    return Dir.Up;
                case Dir.Up:
                    return Dir.UpLeft;
                case Dir.UpLeft:
                    return Dir.Left;
                case Dir.Left:
                    return Dir.DownLeft;
                case Dir.DownLeft:
                    return Dir.Down;
                case Dir.Down:
                    return Dir.DownRight;
                default:
                    return Dir.Right;
            }
        }

        /// <summary>
        /// Returns the next 90 degree direction to the one given.
        /// </summary>
        /// <param name="dir">The given direction.</param>
        public static Dir DirNext(Dir dir)
        {
            switch (dir)
            {
                case Dir.Right:
                    return Dir.Down;
                case Dir.Down:
                    return Dir.Left;
                case Dir.Left:
                    return Dir.Up;
                case Dir.Up:
                    return Dir.Right;
                case Dir.DownLeft:
                    return Dir.UpLeft;
                case Dir.DownRight:
                    return Dir.DownLeft;
                case Dir.UpLeft:
                    return Dir.UpRight;
                default:
                    return Dir.DownRight;
            }
        }

        /// <summary>
        /// Returns the opposite direction to the one given.
        /// </summary>
        /// <param name="dir">The given direction.</param>
        public static Dir DirOpp(Dir dir)
        {
            switch (dir)
            {
                case Dir.Right:
                    return Dir.Left;
                case Dir.Down:
                    return Dir.Up;
                case Dir.Left:
                    return Dir.Right;
                case Dir.Up:
                    return Dir.Down;
                case Dir.DownLeft:
                    return Dir.UpRight;
                case Dir.DownRight:
                    return Dir.UpLeft;
                case Dir.UpLeft:
                    return Dir.DownRight;
                default:
                    return Dir.DownLeft;
            }
        }

        /// <summary>
        /// Returns the last 90 degree direction to the one given.
        /// </summary>
        /// <param name="dir">The given direction.</param>
        public static Dir DirPrev(Dir dir)
        {
            switch (dir)
            {
                case Dir.Right:
                    return Dir.Up;
                case Dir.Down:
                    return Dir.Right;
                case Dir.Left:
                    return Dir.Down;
                case Dir.Up:
                    return Dir.Left;
                case Dir.DownLeft:
                    return Dir.DownRight;
                case Dir.DownRight:
                    return Dir.UpRight;
                case Dir.UpRight:
                    return Dir.UpLeft;
                default:
                    return Dir.DownLeft;
            }
        }

        /// <summary>
        /// Returns a basic unit vector for the chosen direction.
        /// Right: (1, 0) Left: (-1, 0)
        /// Down: (0, 1) Up: (0, -1)
        /// </summary>
        /// <param name="dir">The chosen direction.</param>
        public static Vector2 DirVector(Dir dir)
        {
            switch (dir)
            {
                case Dir.Right:
                    return new Vector2(1, 0);
                case Dir.Down:
                    return new Vector2(0, 1);
                case Dir.Left:
                    return new Vector2(-1, 0);
                case Dir.Up:
                    return new Vector2(0, -1);
                case Dir.DownLeft:
                    return new Vector2(-1, 1);
                case Dir.DownRight:
                    return new Vector2(1, 1);
                case Dir.UpLeft:
                    return new Vector2(-1, -1);
                default:
                    return new Vector2(1, -1);
            }
        }

        /// <summary>
        /// Returns true if the given direction is left, right, up, or down,
        /// and false otherwise.
        /// </summary>
        /// <param name="dir">The chosen direction.</param>
        public static bool DirCardinal(Dir dir)
        {
            if (dir == Dir.Right ||
                dir == Dir.Down ||
                dir == Dir.Left ||
                dir == Dir.Up)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a block corresponding to the given type.
        /// </summary>
        /// <param name="type">A block type.</param>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public static GameObj BlockFromType(MainLoop game, Type type,
            int x, int y, int layer)
        {
            switch (type)
            {
                case Type.Actor:
                    return new MazeActor(game, x, y, layer);
                case Type.Belt:
                    return new MazeBelt(game, x, y, layer);
                case Type.Checkpoint:
                    return new MazeCheckpoint(game, x, y, layer);
                case Type.Click:
                    return new MazeClick(game, x, y, layer);
                case Type.Coin:
                    return new MazeCoin(game, x, y, layer);
                case Type.CoinLock:
                    return new MazeCoinLock(game, x, y, layer);
                case Type.Crate:
                    return new MazeCrate(game, x, y, layer);
                case Type.CrateBroken:
                    return new MazeCrateBroken(game, x, y, layer);
                case Type.CrateHole:
                    return new MazeCrateHole(game, x, y, layer);
                case Type.EAuto:
                    return new MazeEAuto(game, x, y, layer);
                case Type.ELight:
                    return new MazeELight(game, x, y, layer);
                case Type.Enemy:
                    return new MazeEnemy(game, x, y, layer);
                case Type.EPusher:
                    return new MazeEPusher(game, x, y, layer);
                case Type.Filter:
                    return new MazeFilter(game, x, y, layer);
                case Type.Finish:
                    return new MazeFinish(game, x, y, layer);
                case Type.Floor:
                    return new MazeFloor(game, x, y, layer);
                case Type.Freeze:
                    return new MazeFreeze(game, x, y, layer);
                case Type.Gate:
                    return new MazeGate(game, x, y, layer);
                case Type.Goal:
                    return new MazeGoal(game, x, y, layer);
                case Type.Health:
                    return new MazeHealth(game, x, y, layer);
                case Type.Ice:
                    return new MazeIce(game, x, y, layer);
                case Type.Key:
                    return new MazeKey(game, x, y, layer);
                case Type.Lock:
                    return new MazeLock(game, x, y, layer);
                case Type.Message:
                    return new MazeMessage(game, x, y, layer);
                case Type.Mirror:
                    return new MazeMirror(game, x, y, layer);
                case Type.MultiWay:
                    return new MazeMultiWay(game, x, y, layer);
                case Type.Panel:
                    return new MazePanel(game, x, y, layer);
                case Type.Rotate:
                    return new MazeRotate(game, x, y, layer);
                case Type.Spawner:
                    return new MazeSpawner(game, x, y, layer);
                case Type.Spike:
                    return new MazeSpike(game, x, y, layer);
                case Type.Stairs:
                    return new MazeStairs(game, x, y, layer);
                case Type.Teleporter:
                    return new MazeTeleporter(game, x, y, layer);
                case Type.Thaw:
                    return new MazeThaw(game, x, y, layer);
                case Type.Turret:
                    return new MazeTurret(game, x, y, layer);
                case Type.TurretBullet:
                    return new MazeTurretBullet(game, x, y, layer);
                case Type.Wall:
                    return new MazeWall(game, x, y, layer);
                default:
                    return null;
            }
        }
    }
}