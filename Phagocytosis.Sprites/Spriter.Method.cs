using System.Numerics;

namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Spriter
    /// </summary>
    public sealed partial class Spriter
    {

        public void Upgrade(int level)
        {
            this.Level = level;
            this.Radius = Spriter.GetRadius(this.Level);
            this.Speed = Spriter.GetSpeed(this.Level, this.Type);
            this.Render();
        }

        public Spriter Divided()
        {
            // Level / 2, Radius / 2
            this.Upgrade(this.Level / 2);

            Vector2 position = this.Position;
            this.State = SpriteState.None;
            this.Position = new Vector2(this.Radius + position.X, position.Y);

            if (this.Type == SpriteType.Player)
            {
                this.Rebirth();
                return new Spriter(this.ResourceCreator, SpriteType.Cell, new Vector2(-this.Radius + position.X, position.Y), this.Level, this.Root + 1);
            }
            else
            {
                return new Spriter(this.ResourceCreator, this.Type, new Vector2(-this.Radius + position.X, position.Y), this.Level, this.Root + 1);
            }
        }

        public void Dividing()
        {
            switch (this.State)
            {
                case SpriteState.None:
                case SpriteState.Escaping:
                case SpriteState.Chasing:
                case SpriteState.Foraging:
                    if (this.Level > 192 * 1)
                    {
                        this.State = SpriteState.Dividing;
                        this.Progress = 0;
                        this.Duration = 0;
                    }
                    break;
            }
        }

        public Spriter Rebirth()
        {
            this.Type = SpriteType.Player;
            this.State = SpriteState.Rebirth;
            this.Velocity = Vector2.Zero;
            this.Duration = 0;
            this.MaxLevel = Spriter.GetMaxLevel(this.Root, SpriteType.Player);
            this.Speed = Spriter.GetSpeed(this.Level, SpriteType.Player);
            return this;
        }

        public Spriter Prions()
        {
            this.Type = SpriteType.Prion;
            this.State = SpriteState.None;
            this.Velocity = Vector2.Zero;
            this.Duration = 0;
            this.MaxLevel = Spriter.GetMaxLevel(this.Root, SpriteType.Prion);
            this.Speed = Spriter.GetSpeed(this.Level, SpriteType.Prion);
            this.Render();
            return this;
        }


    }
}