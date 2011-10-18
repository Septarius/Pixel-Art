using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PixelArt
{
    static class Line
    {
        static Texture2D FillTexture;

        public static void CreateTexture(GraphicsDevice graphicsDevice)
        {
            Line.FillTexture = new Texture2D(graphicsDevice, 1, 1);
            Line.FillTexture.SetData<Color>(new Color[] { Color.White });
        }

        public static void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Color color, Point a, Point b, float layer, int thickness)
        {
            Vector2 pointA = a.ToVector2();
            Vector2 pointB = b.ToVector2();

            Vector2 direction = pointA - pointB;

            if (direction != Vector2.Zero)
                direction.Normalize();

            pointA -= direction * 0;
            pointB += direction * 0;

            float length = (pointA - pointB).Length();
            float rotation = (float)Math.Atan2(direction.Y, direction.X);

            Rectangle rect = new Rectangle((int)(pointA.X + pointB.X) / 2,
                                            (int)(pointA.Y + pointB.Y) / 2,
                                            (int)length,
                                            thickness);

            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            spriteBatch.Draw(texture, rect, null, color, rotation, origin, SpriteEffects.None, layer);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Color color, Point a, Point b, float layer, int thickness)
        {
            DrawLine(spriteBatch, FillTexture, color, a, b, layer, thickness);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Color color, Vector2 pointA, Vector2 pointB, float layer, int thickness)
        {
            DrawLine(spriteBatch, color, new Point((int)pointA.X, (int)pointA.Y), new Point((int)pointB.X, (int)pointB.Y), layer, thickness);
        }

    }


}