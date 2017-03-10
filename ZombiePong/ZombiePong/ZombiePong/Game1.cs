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
        float ballSpeed = 550;

        Sprite paddle1, paddle2, ball;
        int score1, score2;
        List<Sprite> zombies = new List<Sprite>();
        Random rand = new Random(System.Environment.TickCount);
      

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

            score1 = 0;
            score2 = 0;

            background = Content.Load<Texture2D>("background");
            spritesheet = Content.Load<Texture2D>("spritesheet");

            paddle1 = new Sprite(new Vector2(20, 20), spritesheet, new Rectangle(0, 516, 25, 150), Vector2.Zero);
            paddle2 = new Sprite(new Vector2(970, 20), spritesheet, new Rectangle(32, 516, 25, 150), new Vector2(0, 90));
            ball = new Sprite(new Vector2(700, 350), spritesheet, new Rectangle(76, 510, 40, 40), new Vector2(180, 30));

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

            if (rand.Next(0, 100) < 60)
            {
                paddle2.Velocity = new Vector2(0, ball.Center.Y - paddle2.Center.Y);
            }
            else
            {
                float speed = paddle2.Velocity.Length();
                speed *= 0.9f;

                Vector2 vel = paddle2.Velocity;
                vel.Normalize();
                vel *= speed;
                paddle2.Velocity = vel;
            }

            if (ball.Location.X < -32 || ball.Location.X > this.Window.ClientBounds.Width)
            {
                if (ball.Location.X < -32)
                    score2++;
                else
                    score1++;

                ball.Location = new Vector2(400, 300);

                int flipX = 1, flipY = 1;
                if (rand.Next(0, 100) < 50)
                    flipX *= -1;

                if (rand.Next(0, 100) < 50)
                    flipY *= -1;

                ball.Velocity *= new Vector2(flipX, flipY);


            }
            Window.Title = "Player 1: " + score1 + " | " + "Player 2: " + score2;

            //paddle2.Location = new Vector2(paddle2.Location.X, ball.Center.Y - 60);
            if (paddle2.IsBoxColliding(ball.BoundingBoxRect))
            {
                if (ball.Center.Y > paddle1.Center.Y)
                {
                    if (ball.Velocity.Y < 0)
                        ball.Velocity *= new Vector2(-1, -1);
                    else
                        ball.Velocity *= new Vector2(-1, 1);
                }
                else
                {
                    if (ball.Velocity.Y < 0)
                        ball.Velocity *= new Vector2(-1, 1);
                    else
                        ball.Velocity *= new Vector2(-1, -1);
                }
            

                ball.Location = new Vector2(paddle2.Location.X - ball.BoundingBoxRect.Width, ball.Location.Y);
            }

            if (paddle1.IsBoxColliding(ball.BoundingBoxRect))
            {
                if (ball.Center.Y > paddle1.Center.Y)
                {
                    if (ball.Velocity.Y < 0)
                        ball.Velocity *= new Vector2(-1, -1);
                    else
                        ball.Velocity *= new Vector2(-1, 1);
                }
                else
                {
                    if (ball.Velocity.Y < 0)
                        ball.Velocity *= new Vector2(-1, 1);
                    else
                        ball.Velocity *= new Vector2(-1, -1);
                }




                  ball.Location = new Vector2(paddle1.Location.X + paddle1.BoundingBoxRect.Width, ball.Location.Y);

            }

            if (ball.Location.Y < 0 || ball.Location.Y > this.Window.ClientBounds.Height - ball.BoundingBoxRect.Height)
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
                    ball.Velocity *= new Vector2(-1, 1);
                }
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
            paddle2.Update(gameTime);

            MouseState ms = Mouse.GetState();
            paddle1.Location = new Vector2(paddle1.Location.X, MathHelper.Clamp(ms.Y, 0, this.Window.ClientBounds.Height - paddle1.BoundingBoxRect.Height));
            paddle2.Location = new Vector2(paddle2.Location.X, MathHelper.Clamp(paddle2.Location.Y, 0, this.Window.ClientBounds.Height - paddle2.BoundingBoxRect.Height));

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
