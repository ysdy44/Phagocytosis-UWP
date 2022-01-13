namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Interface of <see cref="Spriter"/>.
    /// </summary>
    public interface ISprite
    {
        float X { get; }
        float Y { get; }
        int Level { get; }
        int Type { get; }
    }
}