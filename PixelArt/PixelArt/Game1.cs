using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PixelArt
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const bool SCREENSAVER = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Grid grid;
        RandomDFS sw;
        MouseState pMs;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //Config.ResolutionWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //Config.ResolutionHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            graphics.PreferredBackBufferWidth = Config.ResolutionWidth;
            graphics.PreferredBackBufferHeight = Config.ResolutionHeight;
            graphics.IsFullScreen = SCREENSAVER;
            this.IsMouseVisible = !SCREENSAVER;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Line.CreateTexture(GraphicsDevice);
            Config.LoadContent(Content, GraphicsDevice);
            grid = new Grid();
            sw = new RandomDFS();
            pMs = Mouse.GetState();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            sw.Update(gameTime);
            //grid.Update();
            MouseState ms = Mouse.GetState();

            if (SCREENSAVER)
            {
                if (ms.X != pMs.X || ms.Y != pMs.Y)
                {
                    this.Exit();
                }
            }



            pMs = ms;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            sw.Draw(spriteBatch);
            //grid.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
