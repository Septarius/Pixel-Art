using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PixelArt
{
    class Point
    {
        int x = 0;
        int y = 0;

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.x + p2.x, p1.y + p2.y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.x - p2.x, p1.y - p2.y);
        }

        public static Point operator *(Point p, int s)
        {
            return new Point(p.x * s, p.y * s);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }
        public override string ToString()
        {
            return string.Format("{0},{1}", x, y);
        }

        public override bool Equals(object obj)
        {
            Point b = (Point)obj;
            if (!(b is Point))
                return false;
            else
            {
                if (this.x == b.x && this.y == b.y)
                    return true;
            }
            return false;
        }
    }
}
