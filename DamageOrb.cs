using System;
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
    /// The direction enum 
    /// </summary>
    public enum Direction
    {
        Down = 0,
        Right = 1,
        Up =2 ,
        Left = 3,
    }
    


    /// <summary>
    /// A class representing a damage orb sprite sprite
    /// </summary>
    public class DamageOrbSprite
    {
        private Texture2D texture;

        private Texture2D hitbox;
        private double directionTimer;

        private BoudningCircle bounds; 
        private int borderMinX = 0;
        private int borderMinY = 0;
        private int borderMaxX = 800;
        private int borderMaxY = 480;

        /// <summary>
        /// The boudning volume of the sprite
        /// </summary>
        public BoudningCircle Bounds => bounds;
        /// <summary>
        /// the direction of the orb
        /// </summary>
        public Direction Direction;

        /// <summary>
        /// The position of the orb
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Creates a new gem sprite
        /// </summary> 
        /// <param name="position">The position of the sprite in the game</param>
        public DamageOrbSprite(Vector2 position)
        {
            this.Position = position;
            this.bounds = new BoudningCircle(position - new Vector2(-16, -16), 16);
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("DamageOrb");
            hitbox = content.Load<Texture2D>("32x32CircleHitBox");
        }

        /// <summary>
        /// Updates the sprite's position based on user input
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        public void Update(GameTime gameTime)
        {
            directionTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if(directionTimer > 2.0)
            {
                switch (Direction)
                {
                    case Direction.Up:
                        Direction = Direction.Down;
                        break;
                    case Direction.Down:
                        Direction = Direction.Right;
                        break;
                    case Direction.Right:
                        Direction = Direction.Left;
                        break;
                    case Direction.Left:
                        Direction = Direction.Up;
                        break;                
                }
                directionTimer -= 2.0;
            }

            // Move damage orb
            switch(Direction)
            {
                    case Direction.Up:
                        Position += new Vector2(0, -1) * 100 * (float) gameTime.ElapsedGameTime.TotalSeconds;
                        break;
                    case Direction.Down:
                        Position += new Vector2(0, 1) * 100 * (float) gameTime.ElapsedGameTime.TotalSeconds;
                        break;
                    case Direction.Right:
                        Position += new Vector2(-1, 0) * 100 * (float) gameTime.ElapsedGameTime.TotalSeconds;
                        break;
                    case Direction.Left:
                        Position += new Vector2(1, 0) * 100 * (float) gameTime.ElapsedGameTime.TotalSeconds;
                        break;                
            }

            Position.X = MathHelper.Clamp(Position.X, borderMinX, borderMaxX);
            Position.Y = MathHelper.Clamp(Position.Y, borderMinY, borderMaxY);
            bounds.Center.X = Position.X + 16;
            bounds.Center.Y = Position.Y + 16;
            
        }            

        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {   
            
            var source = new Rectangle(0, 0, 32, 32);
            spriteBatch.Draw(texture, Position, source, Color.White);

            // Rectangle rect = new ((int)(bounds.Center.X - bounds.Radius), (int)(bounds.Center.Y - bounds.Radius), (int)bounds.Radius*2, (int)bounds.Radius*2);
            // spriteBatch.Draw(hitbox, rect, Color.White);
            
        }
    }
}