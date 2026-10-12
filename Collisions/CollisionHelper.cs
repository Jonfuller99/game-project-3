using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CollisionExample.Collisions
{
    /// <summary>
    /// struct representing circular bounds
    /// </summary>
    public static class CollisionHelper
    {
        /// <summary>
        /// Detects Collision between two BoundingCirlces
        /// </summary>
        /// <param name="a"> The first boudning circle</param>
        /// <param name="b">The second boudning circle</param>
        /// <returns></returns>
        public static bool Collides(BoudningCircle a, BoudningCircle b)
        {
            return Math.Pow(a.Radius + b.Radius, 2) >= (
                Math.Pow(a.Center.X - b.Center.X, 2) +
                Math.Pow(a.Center.Y - b.Center.Y, 2));
        }

        /// <summary>
        /// Detects a collsion between two BoudningRectangles
        /// </summary>
        /// <param name="a">The first rectangle</param>
        /// <param name="b">The second rectangle</param>
        /// <returns></returns>
        public static bool Collides(BoudningRectangle a, BoudningRectangle b)
        {
            return !(a.Right < b.Left || a.Left > b.Right || a.Top > b.Bottom || a.Bottom < b.Top);
        }

        /// <summary>
        /// detects a collision between a circle and rectangle
        /// </summary>
        /// <param name="c">The circle</param>
        /// <param name="r">The rectangle</param>
        /// <returns></returns>
        public static bool Collides(BoudningCircle c, BoudningRectangle r)
        {
            float nearestX = MathHelper.Clamp(c.Center.X, r.Left, r.Right);
            float nearestY = MathHelper.Clamp(c.Center.Y, r.Top, r.Bottom);

            return Math.Pow(c.Radius, 2) >= (
                Math.Pow(c.Center.X - nearestX, 2) +
                Math.Pow(c.Center.Y - nearestY, 2));
            
        }

        public static bool Collides(BoudningRectangle r, BoudningCircle c) => Collides(c, r);

    }
}