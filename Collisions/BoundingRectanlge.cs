
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CollisionExample.Collisions
{
    /// <summary>
    /// struct representing circular bounds
    /// </summary>
    public struct BoudningRectangle
    {
    
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public float Left => X;
        public float Right => X + Width;
        public float Top => Y;
        public float Bottom => Y + Height;

        public BoudningRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;
        }

        public BoudningRectangle(Vector2 position, float width, float height)
        {
            X = position.X;
            Y = position.Y;
            Width = width;
            Height = height;
        }
         

        public bool CollidesWith(BoudningRectangle other)
        {
            
            return CollisionHelper.Collides(this, other);
        }

        public bool CollidesWith(BoudningCircle other)
        {
            return CollisionHelper.Collides(other, this);
        }
    }
}