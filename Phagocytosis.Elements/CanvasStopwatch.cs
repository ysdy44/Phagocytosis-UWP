using System;

namespace Phagocytosis.Elements
{
    /// <summary>
    /// Stopwatch of <see cref="CanvasAnimatedControl"/>.
    /// </summary>
    public class CanvasStopwatch
    {
        TimeSpan Duration;
        TimeSpan Starting;
        TimeSpan Now;
        public TimeSpan TotalTime() => this.Now - this.Starting + this.Duration;
        public void Update(TimeSpan now) => this.Now = now;
        public void Restart() => this.Duration = TimeSpan.Zero;
        public void Play() => this.Starting = this.Now;
        public void Pause() => this.Duration += this.Now - this.Starting;
    }
}