using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelArt
{
    public class Lightning
    {
        //below manages all lightning bolts as a whole
        private static List<Lightning> Bolts = new List<Lightning>();
        private static Random Random = new Random();

        public static void SetupLightning(GraphicsDevice graphics)
        {
            Line.CreateTexture(graphics);
        }

        /// <summary>
        /// Creates a lightning bolt
        /// </summary>
        /// <param name="beginPoint">The beginning of the lightning bolt</param>
        /// <param name="endPoint">The ending of the lightning bolt</param>
        /// <param name="sections">How many sections the bolt will be made of; use -1 to make it be based on distance between start and end</param>
        /// <param name="xOffset">The xOffset that each endpoint will move by</param>
        /// <param name="yOffset">The yOffset that each endpoint will move by</param>
        /// <returns>Returns the lightning bolt instance that was created</returns>
        public static Lightning CreateBolt(Vector2 beginPoint, Vector2 endPoint, int sections, int xOffset, int yOffset)
        {
            Lightning bolt = new Lightning(beginPoint, endPoint, sections, xOffset, yOffset);
            return bolt;
        }

        public static void UpdateAll(GameTime gameTime)
        {
            foreach (var bolt in Bolts)
            {
                bolt.Update(gameTime);
            }
        }

        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (var bolt in Bolts)
            {
                bolt.Draw(spriteBatch);
            }
        }


        //below is stuff for individual bolts
        private List<Vector2> boltPieces = new List<Vector2>();

        public event EventHandler ColorChange;

        #region Properties
        private Color color = Color.White;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        private int thickness = 1;
        public int Thickness
        {
            get { return thickness; }
            set { thickness = Math.Max(1, value); }
        }

        int sectionAmt;
        public int Sections
        {
            get { return sectionAmt; }
            set { sectionAmt = value; }
        }

        int xOffset;
        public int OffsetX
        {
            get { return xOffset; }
            set { xOffset = Math.Max(0, value); }
        }

        int yOffset;
        public int OffsetY
        {
            get { return yOffset; }
            set { yOffset = Math.Max(0, value); }
        }

        Vector2 beginPoint;
        public Vector2 StartPoint
        {
            get { return beginPoint; }
            set { beginPoint = value; }
        }

        Vector2 endPoint;
        public Vector2 EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }

        private bool visible = true;
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
        #endregion

        private Lightning(Vector2 beginPoint, Vector2 endPoint, int sections, int xOffset, int yOffset)
        {
            Lightning.Bolts.Add(this);
            this.beginPoint = beginPoint;
            this.endPoint = endPoint;
            this.sectionAmt = sections;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.ColorChange += new EventHandler(Lightning.ColorChangeDefault);
        }

        static void ColorChangeDefault(object sender, EventArgs e)
        {
            Lightning bolt = (Lightning)sender;
            bolt.color = new Color(
                (byte)0,
                (byte)0,
                (byte)RandomRange(50, 255)
            );
        }


        private void Update(GameTime gameTime)
        {
            if (visible)
            {
                boltPieces = MakeLightning(beginPoint, endPoint, sectionAmt, xOffset, yOffset);
            }
        }

        private List<Vector2> MakeLightning(Vector2 pointA, Vector2 pointB, int k, int xMod, int yMod)
        {
            List<Vector2> lightning = new List<Vector2>();
            lightning.Add(pointA);
            if (k < 1)
            {
                k = (int)(Vector2.Distance(pointA, pointB) / 10);
            }
            for (int i = 0; i < k; i++)
            {
                float value = ((float)(i + 1) / k);
                Vector2 temp = Vector2.Lerp(pointA, pointB, value);
                Vector2 lightningPiece = new Vector2(temp.X + RandomRange(-xMod, xMod), temp.Y + RandomRange(-yMod, yMod));
                lightning.Add(lightningPiece);
            }
            lightning.Add(pointB);

            return lightning;
        }

        private static float RandomRange(int min, int max)
        {
            return (float)Random.NextDouble() * (max - min) + min;
        }

        private void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                for (int i = 0; i < boltPieces.Count - 1; i++)
                {
                    if (ColorChange != null)
                        ColorChange(this, EventArgs.Empty);
                    Line.DrawLine(spriteBatch, color, boltPieces[i], boltPieces[(i + 1)], 1f, thickness);
                }
            }
        }
    }
}
