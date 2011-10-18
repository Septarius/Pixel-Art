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
    class Directions
    {
        public static Point UP = new Point(0, -1);
        public static Point DOWN = new Point(0, 1);
        public static Point LEFT = new Point(-1, 0);
        public static Point RIGHT = new Point(1, 0);

        public static Point UPLEFT
        {
            get { return UP + LEFT; }
        }
        public static Point UPRIGHT
        {
            get { return UP + RIGHT; }
        }

        public static Point DOWNLEFT
        {
            get { return DOWN + LEFT; }
        }
        public static Point DOWNRIGHT
        {
            get { return DOWN + RIGHT; }
        }
    }
}
