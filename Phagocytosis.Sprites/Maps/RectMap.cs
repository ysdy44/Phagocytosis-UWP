using System.Collections.Generic;
using System.Numerics;

namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Map of <see cref = "Rect2" />s.
    /// </summary>
    public sealed class RectMap : List<Rect2>
    {

        /// <summary> Gets map's width. </summary>
        public int Width => this.BugMap.Width;

        /// <summary> Gets map's height. </summary>
        public int Height => this.BugMap.Height;

        /// <summary> Gets map's center. </summary>
        public Vector2 Center => this.BugMap.Center;

        /// <summary>
        /// Map of <see cref = "Bug" />s.
        /// </summary>
        public readonly BugMap BugMap;

        //@Constructs
        /// <summary>
        /// Initialize a RectMap.
        /// </summary>
        /// <param name="width"> The width. </param>
        /// <param name="height"> The height. </param>
        public RectMap(int width, int height)
        {
            this.BugMap = new BugMap(width, height);
        }

        /// <summary>
        /// Returns a point contained within the range.
        /// </summary>
        /// <param name="point"> The point. </param>
        /// <param name="withRestricteds"> With restrictions. </param>
        /// <returns> Returns a point contained within the range. </returns>
        public Vector2 Range(Vector2 point, bool withRestricteds)
        {
            point = this.BugMap.Range(point);
            if (withRestricteds == false) return point;

            foreach (Rect2 item in this)
            {
                point = item.Range(point);
            }

            return point;
        }

        /// <summary>
        /// Returns whether the area filled by the map contains the specified point.
        /// </summary>
        /// <param name="point"> The point. </param>
        /// <param name="withRestricteds"> With restrictions. </param>
        /// <returns> Return **true** if the area filled by the map contains the specified point, otherwise **false**. </returns>
        public bool Contains(Vector2 point, bool withRestricteds)
        {
            bool isContains = this.BugMap.Rect.Contains(point);
            if (withRestricteds == false) return isContains;
            if (isContains == false) return false;

            foreach (Rect2 item in this)
            {
                if (item.Contains(point))
                {
                    return false;
                }
            }

            return true;
        }

    }
}