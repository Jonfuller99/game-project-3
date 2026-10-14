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
    public class JetSprite
    {
        private Game game;
        private GamePadState gamePadState;

        private KeyboardState keyboardState;
        protected KeyboardState priorKeyboardState;

        private Texture2D texture;

        private Texture2D hitbox;


        private bool flipped;

        private Direction direction;

        private int borderMinX = 0;
        // private int borderMinY = 0;
        // private int borderMaxX = 800;
        private int borderMaxY = 480;
        private Vector2 position = new Vector2(50, 430);
        

        private BoudningRectangle bounds = new BoudningRectangle(new Vector2(50-32, 430-32), 64, 64);

        /// <summary>
        /// The boudning volume of the sprite
        /// </summary>
        public BoudningRectangle Bounds => bounds;

        /// <summary>
        /// The color to blend with the ghost
        /// </summary>
        public Color Color {get; set;} = Color.White;


        public JetSprite(Game game)
        {
            this.game = game;            
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Jet");
            hitbox = content.Load<Texture2D>("64x64HitBox");
        }

        public void Reset(ContentManager content)
        {
            LoadContent(content);
            position = new Vector2(borderMinX + 50, borderMaxY - 50);
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


            // position.X = MathHelper.Clamp(position.X, borderMinX, borderMaxX);
            // position.Y = MathHelper.Clamp(position.Y, borderMinY, borderMaxY);

            // update the bounding box center
          SyncBounds();

            // Wrap the ship to keep it on-screen
            var viewport = game.GraphicsDevice.Viewport;
            if (position.Y < 0) position.Y = viewport.Height;
            if (position.Y > viewport.Height) position.Y = 0;
            if (position.X < 0) position.X = viewport.Width;
            if (position.X > viewport.Width) position.X = 0;
            
        }


        /// <summary>
        /// update the bounding box center
        /// </summary>
        private void SyncBounds()
        {
            bounds.X = position.X - 32;
            bounds.Y = position.Y - 32;
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
            
            spriteBatch.Draw(texture, position, null, Color, 0, new Vector2(32, 32), 1f, spriteEffects, 0);
            // Rectangle rect = new ((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);
            // spriteBatch.Draw(hitbox, rect, Color.White);
        }



    }
}