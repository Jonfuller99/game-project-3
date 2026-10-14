using CollisionExample.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
    private Tilemap _tilemap;

    private JetSprite characterPlaceHolder;


    private GemSprite [] gems;

    public int gemsCollected;
    public SoundEffect gemPickup;
    public SoundEffect hit;
    private List<BoudningRectangle> _solids;

    

public Game1()
{
    _graphics = new GraphicsDeviceManager(this);

    _graphics.PreferredBackBufferWidth = 960;
    _graphics.PreferredBackBufferHeight = 960;

    Content.RootDirectory = "Content";
    IsMouseVisible = true;
}

protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        characterPlaceHolder = new(this);


        System.Random rand = new System.Random();
        gems = new GemSprite[]
        {
          new GemSprite(new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height)),  
          new GemSprite(new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height)),  
          new GemSprite(new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height)),  
          new GemSprite(new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height)),  
          new GemSprite(new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height)),  
        };



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

    _solids = _tilemap.GetSolidRectangles("Object Layer 1");


    characterPlaceHolder.LoadContent(Content);

    foreach(var gem in gems) gem.LoadContent(Content);
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
 

        base.Update(gameTime);
    }

protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);

    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    _tilemap.Draw(_spriteBatch);
    _spriteBatch.DrawString(font, $"Gems: {gemsCollected}", new Vector2(2,2), Color.Black);



    foreach (var gem in gems) gem.Draw(gameTime, _spriteBatch);
    characterPlaceHolder.Draw(gameTime, _spriteBatch);

    _spriteBatch.End();

    base.Draw(gameTime);
}
}