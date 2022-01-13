using System;
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

        public void UpdateRobot(IList<Spriter> enemyPlayers, IList<Food> foods, RectMap map, float elapsedTime)
        {
            switch (this.State)
            {
                case SpriteState.None:
                    {
                        if (this.Level > this.MaxLevel)
                        {
                            this.Dividing();
                            return;
                        }

                        {
                            bool isFind = false;
                            float minDistance = float.MaxValue;

                            foreach (Spriter item in enemyPlayers)
                            {
                                switch (item.State)
                                {
                                    case SpriteState.Dead:
                                    case SpriteState.Rebirth:
                                    case SpriteState.Infected:
                                    case SpriteState.Cancerous:
                                        break;

                                    default:
                                        if (Spriter.CanChase(item, this))
                                        {
                                            float distance = Vector2.Distance(this.Position, item.Position);
                                            if (minDistance > distance)
                                            {
                                                if (Spriter.CanFind(item, this, distance))
                                                {
                                                    isFind = true;
                                                    minDistance = distance;
                                                    this.HunterTarget = item;
                                                }
                                            }
                                        }
                                        break;
                                }
                            }

                            if (isFind)
                            {
                                this.Velocity = Spriter.GetVelocity(this.Position - this.HunterTarget.Position);
                                this.State = SpriteState.Escaping;
                                break;
                            }
                        }

                        {
                            bool isFind = false;
                            float minDistance = float.MaxValue;

                            foreach (Spriter item in enemyPlayers)
                            {
                                switch (item.State)
                                {
                                    case SpriteState.Dead:
                                    case SpriteState.Rebirth:
                                    case SpriteState.Infected:
                                    case SpriteState.Cancerous:
                                        break;

                                    default:
                                        if (Spriter.CanChase(this, item))
                                        {
                                            float distance = Vector2.Distance(this.Position, item.Position);
                                            if (minDistance > distance)
                                            {
                                                if (Spriter.CanFind(this, item, distance))
                                                {
                                                    isFind = true;
                                                    minDistance = distance;
                                                    this.PreyTarget = item;
                                                }
                                            }
                                        }
                                        break;
                                }
                            }

                            if (isFind)
                            {
                                this.Velocity = Spriter.GetVelocity(this.PreyTarget.Position - this.Position);
                                this.State = SpriteState.Chasing;
                                break;
                            }
                        }

                        {
                            bool isFind = false;
                            float minDistance = float.MaxValue;

                            foreach (Food item in foods)
                            {
                                if (item.IsDead == false)
                                {
                                    float distance = Vector2.Distance(this.Position, item.Position);
                                    if (minDistance > distance)
                                    {
                                        isFind = true;
                                        minDistance = distance;
                                        this.FoodTarget = item;
                                    }
                                }
                            }

                            if (isFind)
                            {
                                this.Velocity = Spriter.GetVelocity(this.FoodTarget.Position - this.Position);
                                this.State = SpriteState.Foraging;
                                break;
                            }
                        }

                    }
                    break;

                case SpriteState.Dead:
                case SpriteState.Cancerous:
                    break;
                case SpriteState.Infected:
                    {
                        float duration = 2000;
                        this.Duration += elapsedTime;
                        this.Progress = Math.Max(0, Math.Min(1, this.Duration / duration));
                        this.Opacity = Spriter.GetOpacity(this.Duration);
                        if (this.Duration >= duration) // duration s
                        {
                            this.State = SpriteState.Dead;
                        }
                    }
                    break;

                case SpriteState.Upgrade:
                    this.Render();
                    this.State = SpriteState.None;
                    break;
                case SpriteState.Dividing:
                    this.UpdateDividing(elapsedTime);
                    break;

                case SpriteState.Escaping:
                    {
                        if (this.HunterTarget != null)
                        {
                            switch (this.HunterTarget.State)
                            {
                                case SpriteState.Dead:
                                case SpriteState.Cancerous:
                                    this.State = SpriteState.None;
                                    break;

                                default:
                                    {
                                        if (Spriter.CanChase(this.HunterTarget, this))
                                        {
                                            float distance = Vector2.Distance(this.Position, this.HunterTarget.Position);
                                            if (Spriter.CanFind(this.HunterTarget, this, distance))
                                            {
                                                bool withRestricteds = Spriter.GetIsRestricteds(this.Type);
                                                this.Velocity = Spriter.GetVelocity(this, this.HunterTarget, map, withRestricteds);
                                                return;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }

                        this.Velocity = Vector2.Zero;
                        this.State = SpriteState.None;
                    }
                    break;
                case SpriteState.Chasing:
                    {
                        switch (this.PreyTarget.State)
                        {
                            case SpriteState.Dead:
                            case SpriteState.Rebirth:
                            case SpriteState.Infected:
                            case SpriteState.Cancerous:
                                this.State = SpriteState.None;
                                break;

                            default:
                                {
                                    if (Spriter.CanChase(this, this.PreyTarget))
                                    {
                                        float distance = Vector2.Distance(this.Position, this.PreyTarget.Position);
                                        if (Spriter.CanEat(this, this.PreyTarget, distance))
                                        {
                                            if (enemyPlayers.Contains(this.PreyTarget) == false)
                                            {
                                                this.Velocity = Vector2.Zero;
                                                this.State = SpriteState.None;
                                            }
                                            else
                                            {
                                                this.PreyTarget.State = SpriteState.Dead;
                                                switch (this.Type)
                                                {
                                                    case SpriteType.Prion:
                                                        this.Upgrade(this.Level - this.PreyTarget.Level);
                                                        break;
                                                    default:
                                                        switch (this.PreyTarget.Type)
                                                        {
                                                            case SpriteType.Prion:
                                                            case SpriteType.Cancer:
                                                                this.Upgrade(this.Level - this.PreyTarget.Level);
                                                                break;
                                                            default:
                                                                this.Upgrade(this.Level + this.PreyTarget.Level);
                                                                break;
                                                        }
                                                        break;
                                                }

                                                this.Velocity = Vector2.Zero;
                                                this.State = Spriter.GetState(this.Type, this.PreyTarget.Type);
                                            }
                                            return;
                                        }
                                        else if (Spriter.CanFind(this, this.PreyTarget, distance))
                                        {
                                            this.Velocity = Spriter.GetVelocity(this.PreyTarget.Position - this.Position);
                                            return;
                                        }
                                    }
                                }
                                break;
                        }

                        this.Velocity = Vector2.Zero;
                        this.State = SpriteState.None;
                    }
                    break;
                case SpriteState.Foraging:
                    {
                        if (this.FoodTarget.IsDead == false)
                        {
                            float distance = Vector2.Distance(this.Position, this.FoodTarget.Position);
                            if (this.Radius > distance)
                            {
                                if (foods.Contains(this.FoodTarget) == false)
                                {
                                    this.Velocity = Vector2.Zero;
                                    this.State = SpriteState.None;
                                }
                                else
                                {
                                    this.FoodTarget.IsDead = true;
                                    foods.Remove(this.FoodTarget);
                                    this.Upgrade(this.Level + Food.Level);

                                    this.Velocity = Vector2.Zero;
                                    this.State = SpriteState.Upgrade;
                                }
                            }
                            else
                            {
                                this.Velocity = Spriter.GetVelocity(this.FoodTarget.Position - this.Position);
                            }
                            return;
                        }

                        this.Velocity = Vector2.Zero;
                        this.State = SpriteState.None;
                    }
                    break;
            }
        }

        public void UpdatePlayer(IList<Spriter> enemyPlayers, IList<Food> foods, float elapsedTime)
        {
            switch (this.State)
            {
                case SpriteState.None:
                    {
                        foreach (Spriter item in enemyPlayers)
                        {
                            switch (item.State)
                            {
                                case SpriteState.None:
                                case SpriteState.Escaping:
                                case SpriteState.Chasing:
                                case SpriteState.Foraging:
                                    {
                                        if (Spriter.CanChase(this, item))
                                        {
                                            float distance = Vector2.Distance(this.Position, item.Position);
                                            if (Spriter.CanEat(this, item, distance))
                                            {
                                                item.State = SpriteState.Dead;
                                                switch (item.Type)
                                                {
                                                    case SpriteType.Prion:
                                                    case SpriteType.Cancer:
                                                        this.Upgrade(this.Level - item.Level);
                                                        break;
                                                    default:
                                                        this.Upgrade(this.Level + item.Level);
                                                        break;
                                                }

                                                this.State = Spriter.GetState(this.Type, item.Type);
                                                switch (this.State)
                                                {
                                                    case SpriteState.Infected:
                                                        this.Duration = 0;
                                                        break;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }

                        foreach (Food item in foods)
                        {
                            // if (this.Radius > item.Radius)
                            {
                                float distance = Vector2.Distance(this.Position, item.Position);
                                if (this.Radius > distance)
                                {
                                    item.IsDead = true;
                                    foods.Remove(item);
                                    this.Upgrade(this.Level + Food.Level);
                                    break;
                                }
                            }
                        }

                    }
                    break;

                case SpriteState.Rebirth:
                    this.Duration += elapsedTime;
                    this.Opacity = Spriter.GetOpacity(this.Duration);
                    if (this.Duration >= 1500) // 1.5 s
                    {
                        this.State = SpriteState.None;
                    }
                    break;

                case SpriteState.Infected:
                    {
                        float duration = 2000;
                        this.Duration += elapsedTime;
                        this.Progress = Math.Max(0, Math.Min(1, this.Duration / duration));
                        this.Opacity = Spriter.GetOpacity(this.Duration);
                        if (this.Duration >= duration) // duration s
                        {
                            this.State = SpriteState.Dead;
                        }
                    }
                    break;

                case SpriteState.Upgrade:
                    this.Render();
                    this.State = SpriteState.None;
                    break;
                case SpriteState.Dividing:
                    this.UpdateDividing(elapsedTime);
                    break;
            }
        }

        private void UpdateDividing(float elapsedTime)
        {
            float duration = this.Radius * 50;
            this.Duration += elapsedTime;
            this.Progress = Math.Max(0, Math.Min(1, this.Duration / duration));

            // float scale = (1 - sprite.Progress) * 1.0f + sprite.Progress * (1.0f / 2.0f * 1.4142135623730950488016887242097f);
            float scale = 1 - this.Progress * 0.29289321881345247559915563789515f;
            float widthR = this.Progress * this.Radius;
            this.DividingRect = new Rect(this.Radius - widthR, 0, this.Radius + widthR, this.Radius + this.Radius);

            this.DividingTransform =
                    Matrix3x2.CreateTranslation(0, -this.Radius) *
                    Matrix3x2.CreateScale(scale, scale) *
                    Matrix3x2.CreateTranslation(this.Position);
            this.DividingTransformClone =
                    Matrix3x2.CreateTranslation(0, -this.Radius) *
                    Matrix3x2.CreateScale(-scale, scale) *
                    Matrix3x2.CreateTranslation(this.Position);

            if (this.Duration >= duration) // duration s
            {
                this.State = SpriteState.Divided;
            }
        }

    }
}