using System;

namespace Phagocytosis.Controls
{
    /// <summary>
    /// Provides data about the <see cref="MainCanvasControl.Record"/> event.
    /// </summary>
    public class RecordEventArgs : EventArgs
    {
        public int FriendSpritesMaxLevel { get; set; }
        public int FriendSpritesCount { get; set; }
        public TimeSpan TotalTime { get; set; }
    }
}