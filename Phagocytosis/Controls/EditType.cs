namespace Phagocytosis.Controls
{
    /// <summary>
    /// Type of <see cref="EditCanvasControl"/>.
    /// </summary>
    public enum EditType
    {
        None,
        Restricted,
        Player,
        Friend,
        Enemy,
    }
    public enum EditMoveType
    {
        Move,
        ResizeMap,
        MoveRestricted,
        ResizeRestricted,
        MoveSprite,
    }
    public enum EditSelectionMode
    {
        None,
        Add,
        Select
    }
}