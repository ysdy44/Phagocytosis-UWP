namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Data of <see cref="Spriter"/>.
    /// </summary>
    public struct Sprite : ISprite
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Level { get; set; }
        public int Type { get; set; }
    }
}