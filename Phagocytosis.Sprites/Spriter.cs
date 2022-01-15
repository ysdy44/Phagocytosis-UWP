using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;

namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Spriter
    /// </summary>
    public sealed partial class Spriter
    {

        public readonly int Root;
        public readonly ICanvasResourceCreatorWithDpi ResourceCreator;

        public SpriteState State { get; private set; }
        public SpriteType Type { get; private set; }

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        public int Level { get; private set; }
        public int MaxLevel { get; private set; }
        public float Radius { get; private set; }
        public float Speed { get; private set; }

        public float Progress { get; private set; }
        public float Duration { get; private set; }
        public float Opacity { get; private set; }

        public Rect DividingRect { get; private set; }
        public Matrix3x2 DividingTransform { get; private set; }
        public Matrix3x2 DividingTransformClone { get; private set; }

        public CanvasRenderTarget Nuvleus { get; private set; }
        public CanvasRenderTarget Cytoplasm { get; private set; }

        Spriter HunterTarget;
        Spriter PreyTarget;
        Food FoodTarget;

        public Vector2 Offset => new Vector2(this.Position.X - this.Radius, this.Position.Y - this.Radius);

        //@Constructs
        /// <summary>
        /// Initialize a Spriter.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <param name="type"> The type. </param>
        /// <param name="position"> The position. </param>
        /// <param name="level"> The level. </param>
        /// <param name="root"> The root. </param>
        public Spriter(ICanvasResourceCreatorWithDpi resourceCreator, SpriteType type, Vector2 position, int level = 192 * 4, int root = 0)
        {
            this.Root = root;
            this.ResourceCreator = resourceCreator;

            this.Type = type;
            this.Position = position;
            this.Level = level;
            this.MaxLevel = Spriter.GetMaxLevel(root, type);
            this.Radius = Spriter.GetRadius(level);
            this.Speed = Spriter.GetSpeed(level, type);

            CellRenderTarget render = this.RenderCore();
            this.Nuvleus = render.Nuvleus;
            this.Cytoplasm = render.Cytoplasm;
        }

        public void Render()
        {
            CellRenderTarget render = this.RenderCore();
            this.Nuvleus.Dispose();
            this.Nuvleus = render.Nuvleus;
            this.Cytoplasm.Dispose();
            this.Cytoplasm = render.Cytoplasm;
        }
        private CellRenderTarget RenderCore()
        {
            Vector2 origin = new Vector2(this.Radius, this.Radius);
            float diameter = this.Radius + this.Radius;
            switch (this.Type)
            {
                case SpriteType.Bacteria:
                    return this.ResourceCreator.RenderBacteria(this.Level, this.Radius, diameter, origin);
                case SpriteType.Virus:
                    return this.ResourceCreator.RenderVirus(this.Level, this.Radius, diameter, origin);
                case SpriteType.Paramecium:
                    return this.ResourceCreator.RenderParamecium(this.Level, this.Radius, diameter, origin);
                case SpriteType.Leukocyte:
                    return this.ResourceCreator.RenderLeukocyte(this.Level, this.Radius, diameter, origin);
                case SpriteType.Prion:
                    return this.ResourceCreator.RenderPrion(this.Level, this.Radius, diameter, origin);
                case SpriteType.Cancer:
                    return this.ResourceCreator.RenderCancer(this.Level, this.Radius, diameter, origin);
                default:
                    return this.ResourceCreator.RenderCell(this.Level, this.Radius, diameter, origin);
            }
        }

        public void Update(IList<Spriter> enemyPlayers, FoodMap map, float elapsedTime)
        {
            bool withRestricteds = Spriter.GetIsRestricteds(this.Type);
            Vector2 position = this.Position + this.Velocity * this.Speed * elapsedTime;
            this.Position = map.Restricteds.Range(position, withRestricteds);

            switch (this.Type)
            {
                case SpriteType.Player:
                    this.UpdatePlayer(enemyPlayers, map, elapsedTime);
                    break;
                default:
                    this.UpdateRobot(enemyPlayers, map, map.Restricteds, elapsedTime);
                    break;
            }
        }


        //@Property
        public static Spriter Load(ICanvasResourceCreatorWithDpi resourceCreator, ISprite item)
        {
            if (item == null) return null;

            return new Spriter(resourceCreator, (SpriteType)item.Type, new Vector2(item.X, item.Y), item.Level);
        }
        public ISprite Save() => new Sprite
        {
            X = this.Position.X,
            Y = this.Position.Y,
            Level = this.Level,
            Type = (int)this.Type,
        };

    }
}