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

     /// <summary>
    /// The vertices of the quad
    /// </summary>
    VertexPositionTexture[] vertices;

    /// <summary>
    /// The vertex indices of the quad
    /// </summary>
    short[] indices;

    /// <summary>
    /// The effect to use rendering the triangle
    /// </summary>
    BasicEffect effect;

    /// <summary>
    /// The game this triangle belongs to 
    /// </summary>
    Game game;
    

    public bool Collected { get; set; }
    public BoudningCircle Bounds => new BoudningCircle(_position - new Vector2(-16, -16), 32);

    public GlowPickup(Game game, Vector2 position)
    {
        _position = position;
        this.game = game;
        InitializeVertices();
        InitializeIndices();
        InitializeEffect();
    }

    /// <summary>
    /// Initializes the vertices of our quad
    /// </summary>
    public void InitializeVertices() 
    {
        // Unit quad centered on origin, y-down to match pixel space
        vertices = new VertexPositionTexture[4];
        vertices[0] = new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 0)); // top left
        vertices[1] = new VertexPositionTexture(new Vector3( 1, -1, 0), new Vector2(1, 0)); // top right
        vertices[2] = new VertexPositionTexture(new Vector3( 1,  1, 0), new Vector2(1, 1)); // bottom right
        vertices[3] = new VertexPositionTexture(new Vector3(-1,  1, 0), new Vector2(0, 1)); // bottom left
    }


    /// <summary>
    /// Initialize the indices of our quad
    /// </summary>
    public void InitializeIndices() 
    {
        indices = new short[] { 0, 1, 2, 2, 3, 0 };

    }


    /// <summary>
    /// Initializes the basic effect used to draw the quad
    /// </summary>
    public void InitializeEffect()
    {
        var vp = game.GraphicsDevice.Viewport;
        effect = new BasicEffect(game.GraphicsDevice)
        {
            World = Matrix.Identity,
            View = Matrix.Identity,
            Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, vp.Height, 0, -100, 100),
            TextureEnabled = true,
            VertexColorEnabled = false
        };
    }


    public void LoadContent(ContentManager content, GraphicsDevice device)
    {
        _texture = content.Load<Texture2D>("bow"); 
        _glow = CreateGlow(device, 128);
        effect.Texture = _texture;
    }

    public void Update(GameTime gameTime)
    {
        _time = gameTime.TotalGameTime.TotalSeconds;   // also drives the glow pulse
        _rotation = (float)_time * 2f;

        // scale the unit quad up to the texture's pixel size, spin around Y, move to _position
        effect.World =
        Matrix.CreateScale(_texture.Width / 2f, _texture.Height / 2f, 1f) *
        Matrix.CreateRotationY(_rotation) *
        Matrix.CreateTranslation(_position.X, _position.Y, 0);
    }

    public void Draw(Matrix cameraTransform)
    {
        if (Collected) return;

        // float pulse = 0.5f + 0.5f * (float)Math.Sin(_time * 4);
        // Color tint = Color.Lerp(Color.White, Color.LightGoldenrodYellow, pulse);

        // spriteBatch.Draw(_texture, _position, null, tint, _rotation, new Vector2(_texture.Width / 2f, _texture.Height / 2f), 1f, SpriteEffects.None, 0);

        var device = game.GraphicsDevice;
        device.BlendState = BlendState.AlphaBlend;
        device.DepthStencilState = DepthStencilState.None;
        device.RasterizerState = RasterizerState.CullNone;
        device.SamplerStates[0] = SamplerState.LinearClamp;

        effect.View = cameraTransform;   // same matrix the sprite batch uses

        effect.CurrentTechnique.Passes[0].Apply();
        device.DrawUserIndexedPrimitives(
            PrimitiveType.TriangleList,
            vertices, 0, 4,
            indices, 0, 2);
    
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