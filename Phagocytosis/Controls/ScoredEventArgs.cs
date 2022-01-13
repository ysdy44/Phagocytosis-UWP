using System;

namespace Phagocytosis.Controls
{
    /// <summary>
    /// Provides data about the <see cref="MainCanvasControl.Scored"/> event.
    /// </summary>
    public class ScoredEventArgs : EventArgs
    {
        public int FriendSpritesSumLevel { get; set; }
        public int EnemySpritesSumLevel { get; set; }
    }
}