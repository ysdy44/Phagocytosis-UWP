namespace Phagocytosis.Sprites
{
    /// <summary>
    /// State of <see cref="Spriter"/>.
    /// </summary>
    public enum SpriteState
    {
        None,

        Dead,
        Rebirth,
        Infected,
        Cancerous,
        
        Upgrade,
        Divided,
        Dividing,

        Escaping,
        Chasing,
        Foraging,
    }
}