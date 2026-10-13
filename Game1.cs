using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CollisionExample.Collisions;
using Squared.Tiled;
using System;
using System.IO;

namespace game_project_3;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Map _map;
    // private Vector2 _viewportPosition;
    


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here


        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // _tilemap.LoadContent(Content);

        _map = Map.Load(Path.Combine(Content.RootDirectory, "test.tmx"), Content);
        // _map.ObjectGroups["events"].Objects["hero"].Texture = Content.Load<Texture2D>("hero");

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        // _spriteBatch.Begin(/*transformMatrix: Matrix.CreateTranslation(23,23,0)*/);
        // _tilemap.Draw(gameTime, _spriteBatch);
        // _spriteBatch.End();

        _spriteBatch.Begin();
        _map.Draw(_spriteBatch, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Vector2(0, 0));
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
