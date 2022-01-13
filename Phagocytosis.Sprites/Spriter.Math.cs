using System;
using System.Numerics;

namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Spriter
    /// </summary>
    public sealed partial class Spriter
    {

        //@Static
        public static float GetRadius(int level) => 2 * (float)Math.Sqrt(level);
        public static float GetOpacity(float duration) => (float)(Math.Sin(duration / 100f) + 1) / 2;
        public static int GetLevel(SpriteType type)
        {
            switch (type)
            {
                case SpriteType.Virus:
                    return 192 * 2;
                case SpriteType.Paramecium:
                    return 192 * 64;
                case SpriteType.Leukocyte:
                    return 192 * 16;
                case SpriteType.Prion:
                    return 192 * 8;
                case SpriteType.Cancer:
                    return 192 * 1;
                default:
                    return 192 * 4;
            }
        }

        public static bool GetIsRestricteds(SpriteType type) => type != SpriteType.Leukocyte;
        public static int GetMaxLevel(int root, SpriteType type)
        {
            double splitLevel = 3200;
            for (int i = 0; i < root; i++)
            {
                splitLevel /= 1.2;
            }
            int level = (int)Math.Max(400, splitLevel);

            switch (type)
            {
                case SpriteType.Bacteria:
                    return level / 2;
                case SpriteType.Virus:
                    return level / 4;
                case SpriteType.Paramecium:
                case SpriteType.Leukocyte:
                    return level * 32;
                case SpriteType.Cancer:
                    return level / 16;
                default:
                    return level;
            }
        }
        public static float GetSpeed(int level, SpriteType type)
        {
            float speed = (float)Math.Pow(level, 0.333333333333333333333333333333333f);

            switch (type)
            {
                case SpriteType.Player:
                    return 2 / speed;
                case SpriteType.Virus:
                case SpriteType.Paramecium:
                    return 0.5f / speed;
                case SpriteType.Leukocyte:
                    return 1.5f / speed;
                case SpriteType.Cancer:
                    return 2f / speed;
                default:
                    return 1 / speed;
            }
        }
        public static SpriteState GetState(SpriteType type, SpriteType preyType)
        {
            switch (type)
            {
                case SpriteType.Player:
                    switch (preyType)
                    {
                        case SpriteType.Virus:
                            return SpriteState.Infected;
                        default:
                            return SpriteState.None;
                    }
                case SpriteType.Cell:
                    switch (preyType)
                    {
                        case SpriteType.Virus:
                            return SpriteState.Infected;
                        default:
                            return SpriteState.Upgrade;
                    }
                case SpriteType.Prion:
                    return SpriteState.None;
                default:
                    return SpriteState.Upgrade;
            }
        }

        public static bool CanEat(Spriter hunter, Spriter prey, float distance) => hunter.Radius - prey.Radius / 2 > distance;
        public static bool CanChase(Spriter hunter, Spriter prey) => hunter.Radius > prey.Radius / 4 + prey.Radius;
        public static bool CanFind(Spriter hunter, Spriter prey, float distance) => hunter.Radius + prey.Radius + 100 > distance;

        public static Vector2 GetVelocity(Vector2 vector) => vector / vector.Length();
        public static Vector2 GetVelocity(Spriter player, Spriter hunter, RectMap map, bool withRestricteds)
        {
            Vector2 velocity = player.Velocity;
            Vector2 position = player.Position;
            Vector2 target = velocity * player.Radius;

            Vector2 positionH = hunter.Position;
            float radiusH = hunter.Radius;

            // Straught
            Vector2 position2 = position + target;
            if (map.Contains(position2, withRestricteds))
            {
                float distance = Vector2.Distance(position2, positionH);
                if (distance > radiusH) return velocity;
            }

            // Left Turn
            Vector2 position3 = new Vector2(position.X + target.Y, position.Y - target.X);
            if (map.Contains(position2, withRestricteds))
            {
                float distance = Vector2.Distance(position3, positionH);
                if (distance > radiusH) return new Vector2(velocity.Y, -velocity.X);
            }

            // Right Turn
            Vector2 position4 = new Vector2(position.X - target.Y, position.Y + target.X);
            if (map.Contains(position2, withRestricteds))
            {
                float distance = Vector2.Distance(position4, positionH);
                if (distance > radiusH) return new Vector2(-velocity.Y, velocity.X);
            }

            // Return
            Vector2 position5 = position - target;
            if (map.Contains(position2, withRestricteds))
            {
                float distance = Vector2.Distance(position5, positionH);
                if (distance > radiusH) return -velocity;
            }

            return velocity;
        }


    }
}