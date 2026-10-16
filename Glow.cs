using CollisionExample.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace game_project_3;

public class GlowPickup
{
    private Texture2D _texture;
    private Texture2D _glow;
    private readonly Vector2 _position;
    private float _rotation;
    private double _time;

    public bool Collected { get; set; }
    public BoudningCircle Bounds => new BoudningCircle(_position - new Vector2(-16, -16), 16);

    public GlowPickup(Vector2 position)
    {
        _position = position;
    }

    public void LoadContent(ContentManager content, GraphicsDevice device)
    {
        _texture = content.Load<Texture2D>("DamageOrb"); 
        _glow = CreateGlow(device, 64);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _time += dt;
        _rotation += dt * 2f;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Collected) return;

        float pulse = 0.5f + 0.5f * (float)Math.Sin(_time * 4);
        Color tint = Color.Lerp(Color.White, Color.LightGoldenrodYellow, pulse);

        spriteBatch.Draw(_texture, _position, null, tint, _rotation, new Vector2(_texture.Width / 2f, _texture.Height / 2f), 1f, SpriteEffects.None, 0);
    }

    public void DrawGlow(SpriteBatch spriteBatch)
    {
        if (Collected) return;

        float pulse = 0.5f + 0.5f * (float)Math.Sin(_time * 4);
        Color glowColor = Color.Gold * (0.4f + 0.4f * pulse);
        float scale = 1f + 0.2f * pulse;

        spriteBatch.Draw(_glow, _position, null, glowColor, 0f, new Vector2(_glow.Width / 2f, _glow.Height / 2f), scale, SpriteEffects.None, 0);
    }

    private static Texture2D CreateGlow(GraphicsDevice device, int size)
    {
        var tex = new Texture2D(device, size, size); // Texture for glow
        var data = new Color[size * size]; // 2d color array for the size of the circle in this case
        float r = size / 2f; // radius of glowing circle

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(r, r)) / r; // gets x,y distance from the center which is just vector2(r,r), then dividing by r just normalizes it.
                float a = MathHelper.Clamp(1f - d, 0f, 1f); // translating the distance into brightness for glow. 1f-d flips it so the center is max brightness, then clamping it ensures 1-d doesn't go negative and just stays at 0 for a smoth curve down.
                data[y * size + x] = Color.White * a; // actually storing the brightness values, where a is the brightness value (alpha) of the color white. standard alpha multiplication for 4 chanels, color * alpha for transparency.
            }
        }

        tex.SetData(data);
        return tex;
    }
}