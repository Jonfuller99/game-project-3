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
using GameScreens;
using Input;
using Screens;



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


        var screenManager = new ScreenManager(this);
        Components.Add(screenManager);

        screenManager.AddScreen(new BackgroundScreen(), null);
        screenManager.AddScreen(new MainMenuScreen(), null);        

        base.Initialize();
    }

protected override void LoadContent()
{
    


}

    protected override void Update(GameTime gameTime)
    {
        
 

        base.Update(gameTime);
    }

protected override void Draw(GameTime gameTime)
{

    base.Draw(gameTime);
}




}