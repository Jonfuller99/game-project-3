using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CollisionExample.Collisions;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.Tracing;


namespace game_project_3
{

    public class GemSprite
    {
        private const float ANIMATION_SPEED = 0.1f;

        private double animationTimer;

        private int animationFrame;

        private Vector2 _position;

        private Texture2D texture;

        private BoudningCircle bounds; 

        public bool Collected {get; set;} = false;

        /// <summary>
        /// The boudning volume of the sprite
        /// </summary>
        public BoudningCircle Bounds => bounds;

        private const int SpriteSize = 32;
        private const float ScaleY = 0.7f;


        /// <summary>
        /// Creates a new gem sprite
        /// </summary> 
        /// <param name="position">The position of the sprite in the game</param>
        public GemSprite(Vector2 position)
        {
            // Spawn point = bottom-center of the (scaled) sprite.
            // The sprite's center is half its scaled height above that point.
            _position = new Vector2(position.X, position.Y - (SpriteSize * ScaleY) / 2f);

            // Hitbox centered on the same point the sprite is drawn around
            bounds = new BoudningCircle(_position, SpriteSize / 2f);
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("BlueGem");
        }

        /// <summary>
        /// Draws the animated sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(Collected) return;
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if(animationTimer > ANIMATION_SPEED)
            {
                animationFrame++;
                if (animationFrame > 7) animationFrame = 0;
                animationTimer -= ANIMATION_SPEED;
            }

            var source = new Rectangle(animationFrame * 32, 0, 32, 32);
            spriteBatch.Draw(texture, _position, source, Color.White, 0f,   
                                            new Vector2(16, 16), new Vector2(1, ScaleY), SpriteEffects.None, 0f);
        }
    }
}
