﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CollisionExample.Collisions;

namespace game_project_3
{
    /// <summary>
    /// A class representing a jet
    /// </summary>
    public class TripSprite
    {
        private Game game;
        private GamePadState gamePadState;

        private const float ANIMATION_SPEED = 0.2f;

        private double animationTimer;

        private int animationFrame;

        private KeyboardState keyboardState;

        protected KeyboardState priorKeyboardState;

        private Texture2D texture;

        private Texture2D hitbox;

        private bool flipped;

        private Direction direction;

        public Vector2 position;
        
        private BoudningRectangle bounds = new BoudningRectangle(new Vector2(50 - 16, 430-16), 32, 32);

        /// <summary>
        /// The boudning volume of the sprite
        /// </summary>
        public BoudningRectangle Bounds => bounds;

        /// <summary>
        /// The color to blend with the ghost
        /// </summary>
        public Color Color {get; set;} = Color.White;
        public Vector2 WorldSize {get; set;}


        public TripSprite(Game game)
        {
            this.game = game;            
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Trip");
            hitbox = content.Load<Texture2D>("32x32HitBox");
        }


        /// <summary>
        /// Updates the sprite's position based on user input
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        public void Update(GameTime gameTime,  List<BoudningRectangle> solids)
        {
            gamePadState = GamePad.GetState(0);
            keyboardState = Keyboard.GetState();
            float movementSpeed = 3;

            // Apply the gamepad movement with inverted Y axis
            Vector2 delta = gamePadState.ThumbSticks.Left * new Vector2(1, -1);
            if (gamePadState.ThumbSticks.Left.X < 0) flipped = true;
            if (gamePadState.ThumbSticks.Left.X > 0) flipped = false;

            // Apply keyboard movement
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                delta.Y -= movementSpeed;
                direction = Direction.Up;
                
            } 
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                delta.Y += movementSpeed;
                direction = Direction.Down;
                
            } 
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            { 
                delta.X -= movementSpeed;
                flipped = true;
                direction = Direction.Left;

            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                delta.X += movementSpeed;
                flipped = false;
                direction = Direction.Right;
                
            }
            
            bool isMoving = delta != Vector2.Zero;

            if(isMoving)
            {
                UpdateAnimation(gameTime);

            }
            else
            {
                animationFrame = 0;
                animationTimer = 0;
            }



            Move(delta, solids);

            if (keyboardState.IsKeyDown(Keys.Space) && priorKeyboardState.IsKeyUp(Keys.Space))
            {
                Vector2 step = direction switch
                {
                    Direction.Up => new Vector2(0, -5),
                    Direction.Down => new Vector2(0, 5),
                    Direction.Left => new Vector2(-5, 0),
                    Direction.Right => new Vector2(5, 0),
                    _ => Vector2.Zero
                };

                for (int i = 0; i < 10; i++) Move(step, solids);
            }

            priorKeyboardState = keyboardState;


            // update the bounding box center
            position.X = MathHelper.Clamp(position.X, bounds.Width / 2, WorldSize.X - bounds.Width / 2);
            position.Y = MathHelper.Clamp(position.Y, bounds.Height / 2, WorldSize.Y - bounds.Height / 2);
            SyncBounds();


            
        }


        private void UpdateAnimation(GameTime gameTime)
        {
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if(animationTimer > ANIMATION_SPEED)
            {
                animationFrame++;
                if (animationFrame > 3) animationFrame = 0;
                animationTimer -= ANIMATION_SPEED;
            }
        }

        /// <summary>
        /// update the bounding box center
        /// </summary>
        private void SyncBounds()
        {
            bounds.X = position.X - bounds.Width / 2;
            bounds.Y = position.Y - bounds.Height / 2;
        }

        private void Move(Vector2 delta, List<BoudningRectangle> solids)
        {
            // Horizontal
            position.X += delta.X;
            SyncBounds();
            foreach (var solid in solids)
            {
                if (!Overlaps(bounds, solid)) continue;
                if (delta.X > 0) position.X = solid.Left - bounds.Width / 2;
                else if (delta.X < 0) position.X = solid.Right + bounds.Width / 2;
                SyncBounds();
            }

            // Vertical
            position.Y += delta.Y;
            SyncBounds();
            foreach (var solid in solids)
            {
                if (!Overlaps(bounds, solid)) continue;
                if (delta.Y > 0) position.Y = solid.Top - bounds.Height / 2;
                else if (delta.Y < 0) position.Y = solid.Bottom + bounds.Height / 2;
                SyncBounds();
            }
        }

        private static bool Overlaps(BoudningRectangle a, BoudningRectangle b)
        {
            const float epsilon = 0.01f; // ignore float error when sitting flush
            return a.Left < b.Right - epsilon
                && a.Right > b.Left + epsilon
                && a.Top < b.Bottom - epsilon
                && a.Bottom > b.Top + epsilon;
        }

        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = (flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            

            

            var source = new Rectangle(animationFrame * 32, 0, 32, 32);
            
            spriteBatch.Draw(texture, position, source, Color, 0, new Vector2(16, 16), 1f, spriteEffects, 0);
            
            // Show Hitbox
            // Rectangle rect = new ((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);
            // spriteBatch.Draw(hitbox, rect, Color.White);
        }



    }
}