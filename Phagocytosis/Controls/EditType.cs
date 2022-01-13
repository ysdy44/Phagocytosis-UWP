namespace Phagocytosis.Controls
{
    /// <summary>
    /// Type of <see cref="EditCanvasControl"/>.
    /// </summary>
    public enum EditType
    {
        View,
        Crop,
        Clear,

        Cursor,
        AddCell,
        AddBacteria,
        AddVirus,
        AddParamecium,
        AddLeukocyte,
        AddPrion,
        AddCancer,

        CursorRestricted,
        AddRestricted,

        ZoomIn,
        ZoomOut,
    }
}