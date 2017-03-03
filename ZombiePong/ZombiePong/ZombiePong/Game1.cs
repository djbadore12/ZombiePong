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

namespace ZombiePong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background, spritesheet;
        float ballSpeed = 350;

        Sprite paddle1, paddle2, ball;

        List<Sprite> zombies = new List<Sprite>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("background");
            spritesheet = Content.Load<Texture2D>("spritesheet");

            paddle1 = new Sprite(new Vector2(20, 20), spritesheet, new Rectangle(0, 516, 25, 150), Vector2.Zero);
            paddle2 = new Sprite(new Vector2(970, 20), spritesheet, new Rectangle(32, 516, 25, 150), Vector2.Zero);
            ball = new Sprite(new Vector2(700, 350), spritesheet, new Rectangle(76, 510, 40, 40), new Vector2(120, 30));

            SpawnZombie(new Vector2(400, 400), new Vector2(-20, 0));
            SpawnZombie(new Vector2(420, 300), new Vector2(20, 0));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void SpawnZombie(Vector2 location, Vector2 velocity)
        {
            Sprite zombie = new Sprite(location, spritesheet, new Rectangle(0, 25, 160, 150), velocity);

            for (int i = 1; i < 10; i++)
            {
                zombie.AddFrame(new Rectangle(i * 165, 25, 160, 150));
            }

            zombies.Add(zombie);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            Vector2 bvel = ball.Velocity;
            bvel.Normalize();
            bvel *= ballSpeed;
            ball.Velocity = bvel;


            // TODO: Add your update logic here
            ball.Update(gameTime);

            paddle2.Location = new Vector2(paddle2.Location.X, ball.Center.Y - 60);
            if (paddle2.IsBoxColliding(ball.BoundingBoxRect) || paddle1.IsBoxColliding(ball.BoundingBoxRect))
            {
                ball.Velocity *= new Vector2(-1, 1);
            }

            if (ball.Location.Y < 0 || ball.Location.Y > this.Window.ClientBounds.Height-ball.BoundingBoxRect.Height)
            { 
                ball.Velocity *= new Vector2(1, -1);
               
            }
            for (int i = 0; i < zombies.Count; i++)
            {
                zombies[i].Update(gameTime);

                // int width = this.Window.ClientBounds.Width;
                if (zombies[i].Location.X > 700 || zombies[i].Location.X < 150)
                    zombies[i].Velocity *= new Vector2(-1, -1);


                if (Vector2.Distance(zombies[i].Center, ball.Center) < 50)
                {
                    ball.Velocity *= new Vector2(-1, 1)
;                }
                // Zombie logic goes here.. 
                if (zombies[i].Velocity.X > 0)
                {
                    zombies[i].FlipHorizontal = true;
                }
                else
                {
                    zombies[i].FlipHorizontal = false;
                }
            }

            MouseState ms = Mouse.GetState();
            paddle1.Location = new Vector2(paddle1.Location.X, ms.Y);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            
            spriteBatch.Draw(background, Vector2.Zero, Color.White);



            paddle1.Draw(spriteBatch);
            paddle2.Draw(spriteBatch);
            ball.Draw(spriteBatch);

            for (int i = 0; i < zombies.Count; i++)
            {
                
                zombies[i].Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
