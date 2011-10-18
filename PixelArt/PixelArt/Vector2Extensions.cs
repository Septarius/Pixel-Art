using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelArt
{
    public static class Vector2Extensions
    {
        public static Vector2 ToIntVector(this Vector2 v)
        {
            return new Vector2((int)v.X, (int)v.Y);
        }
    }
}
