using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CollisionExample.Collisions
{
    /// <summary>
    /// struct representing circular bounds
    /// </summary>
    public struct BoudningCircle
    {
        /// <summary>
        /// The center of the BoundingCircle
        /// </summary>
        public Vector2 Center;
        /// <summary>
        /// The radius of the BoundingCircle
        /// </summary>
        public float Radius;

        /// <summary>
        /// Constructs a new Bounding Circle
        /// </summary>
        /// <param name="center">The center</param>
        /// <param name="radius">The radius</param>
        public BoudningCircle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;    
        }

        /// <summary>
        /// Tests for a collision between this and another boudning circle
        /// </summary>
        /// <param name="other">The other bounding circle </param>
        /// <returns>true for collision, false if otherwise</returns>
        public bool CollidesWith(BoudningCircle other)
        {
            return CollisionHelper.Collides(this, other);
        }

        public bool CollidesWith(BoudningRectangle other)
        {
            return CollisionHelper.Collides(this, other);
        }
    }
}