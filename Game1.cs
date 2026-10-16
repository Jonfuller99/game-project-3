using CollisionExample.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using Particles;
using System.Linq;



namespace game_project_3;

public enum GameState
{
    Start,
    Playing,
    Win,
    Lose
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private SpriteFont font;
    public Tilemap _tilemap;

    private TripSprite characterPlaceHolder;

    private const float Zoom = 3f;

    public int gemsCollected;
    public SoundEffect gemPickup;
    public SoundEffect hit;
    private List<BoudningRectangle> _solids;

    private List<GemSprite> gems = new List<GemSprite>();
    private List<Vector2> _gemPoints;
    
    private RainParticleSystem _rain;

    private GlowPickup _relic; 
    

public Game1()
{
    _graphics = new GraphicsDeviceManager(this);

    _graphics.PreferredBackBufferWidth = 1920;
    _graphics.PreferredBackBufferHeight = 1080;

    Content.RootDirectory = "Content";
    IsMouseVisible = true;
}

protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        characterPlaceHolder = new(this);
        
        _rain = new RainParticleSystem(this) ;
        // {Source = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height)};
        Components.Add(_rain);

        

        base.Initialize();
    }

protected override void LoadContent()
{
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    font = Content.Load<SpriteFont>("File");

    gemPickup = Content.Load<SoundEffect>("gem_pickup");
    hit = Content.Load<SoundEffect>("hit");
    
    
    _tilemap = new Tilemap(Path.Combine(Content.RootDirectory, "test.tmx"), "tileset");
    _tilemap.LoadContent(Content);
    int mapW = _tilemap.WidthInPixels;
    int mapH = _tilemap.HeightInPixels;

    _solids = _tilemap.GetSolidRectangles("Object Layer 1");

    _gemPoints = _tilemap.GetPoints("Gems", "Gem");

    foreach (Vector2 p in _gemPoints)
    {
        gems.Add(new GemSprite(p));  
    }
    foreach(var gem in gems) gem.LoadContent(Content);
    

    characterPlaceHolder.LoadContent(Content);
    characterPlaceHolder.position = _tilemap.GetPoints("SpawnPoint", "Spawn")[0];
    characterPlaceHolder.WorldSize = new Vector2(mapW, mapH );

    _rain.Source = new Rectangle(0, 0, mapW, mapH);


    List<Vector2> orbPoints = _tilemap.GetPoints("GlowItems", "Magic Orb");
    _relic = new GlowPickup(orbPoints[0]); 
    _relic.LoadContent(Content, GraphicsDevice);    

}

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        characterPlaceHolder.Update(gameTime, _solids);
        characterPlaceHolder.Color = Color.White;
        foreach(var gem in gems)
        {
            if (!gem.Collected && gem.Bounds.CollidesWith(characterPlaceHolder.Bounds))
            {
                characterPlaceHolder.Color = Color.Gold;
                gem.Collected = true;
                gemsCollected++;
                gemPickup.Play();
                
            }
        }

        _relic.Update(gameTime); 
        if (!_relic.Collected && _relic.Bounds.CollidesWith(characterPlaceHolder.Bounds))
        {
            _relic.Collected = true;
            gemPickup.Play();
        }
 

        base.Update(gameTime);
    }

protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);

    var viewport = GraphicsDevice.Viewport;
    Vector2 visibleSize = new Vector2(viewport.Width, viewport.Height) / Zoom;

    Vector2 cameraPos = characterPlaceHolder.position - visibleSize / 2f;

    float maxX = Math.Max(0, _tilemap.WidthInPixels - visibleSize.X);
    float maxY = Math.Max(0, _tilemap.HeightInPixels - visibleSize.Y);
    cameraPos = Vector2.Clamp(cameraPos, Vector2.Zero, new Vector2(maxX, maxY));

    Matrix transform = Matrix.CreateTranslation(-cameraPos.X, -cameraPos.Y, 0) 
                        * Matrix.CreateScale(Zoom, Zoom, 1f);

    _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: transform);
    _tilemap.Draw(_spriteBatch);

    foreach (var gem in gems) gem.Draw(gameTime, _spriteBatch);
    _relic.Draw(_spriteBatch); 
    characterPlaceHolder.Draw(gameTime, _spriteBatch);

    _spriteBatch.End();

    
    _spriteBatch.Begin(samplerState: SamplerState.PointClamp,
                       blendState: BlendState.Additive,
                       transformMatrix: transform);

    _relic.DrawGlow(_spriteBatch);
    
    _spriteBatch.End();

    _spriteBatch.Begin();
    _spriteBatch.DrawString(font, $"Gems: {gemsCollected}", new Vector2(2, 2), Color.Black);
    _spriteBatch.End();

    _rain.Transform = transform;
    base.Draw(gameTime);
}
}