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

    public class GemSprite
    {
        private const float ANIMATION_SPEED = 0.1f;

        private double animationTimer;

        private int animationFrame;

        private Vector2 position;

        private Texture2D texture;

        private BoudningCircle bounds; 

        public bool Collected {get; set;} = false;

        /// <summary>
        /// The boudning volume of the sprite
        /// </summary>
        public BoudningCircle Bounds => bounds;



        /// <summary>
        /// Creates a new gem sprite
        /// </summary> 
        /// <param name="position">The position of the sprite in the game</param>
        public GemSprite(Vector2 position)
        {
            this.position = position;
            this.bounds = new BoudningCircle(position - new Vector2(-16, -16), 16);
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
            spriteBatch.Draw(texture, position, source, Color.White);
        }
    }
}
