using Phagocytosis.Sprites;
using System.Collections.Generic;

namespace Phagocytosis.ViewModels
{
    /// <summary>
    /// Chapter
    /// </summary>
    public class Chapter
    {
        public bool IsGuider { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }
        public int MaximumFoods { set; get; }
        public int IncreaseFoods { set; get; }
        public IEnumerable<Rect2> Restricteds { set; get; }
        public IEnumerable<ISprite> FriendSprites { set; get; }
        public IEnumerable<ISprite> EnemySprites { set; get; }
    }
}