using CollisionExample.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Collections.Generic;
using System;
using Particles;
using GameScreens;
using Input;
using Microsoft.Xna.Framework.Content;
using System.Threading;
using game_project_3;
using System.Runtime.CompilerServices;

namespace Screens
{
    // This screen implements the actual game logic. It is just a
    // placeholder to get the idea across: you'll probably want to
    // put some more interesting gameplay in here!
    public class GameplayScreen : GameScreen
    {
        private Game game;
        private ContentManager _content;
        private SpriteFont _gameFont;

        private Vector2 _playerPosition = new Vector2(100, 100);
        private Vector2 _enemyPosition = new Vector2(100, 100);

        private SpriteBatch _spriteBatch;
    


        private KeyboardState _prevKeys;
        public Tilemap _tilemap;

        private TripSprite characterPlaceHolder;

        private const float Zoom = 3f;

        public int gemsCollected;
        public SoundEffect gemPickup;
        public SoundEffect hit;
        private List<BoundingRectangle> _solids;
        private List<BoundingRectangle> _interact_areas;

        private List<GemSprite> gems = new List<GemSprite>();
        private List<Vector2> _gemPoints;
        
        private RainParticleSystem _rain;

        private GlowPickup _relic; 

        private Song rain;

        private readonly Random _random = new Random();

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back }, true);
        }

        // Load graphics content for the game
        public override void Activate(bool instancePreserved)
        {
                game = ScreenManager.Game;


                _rain = new RainParticleSystem(game) ;
                game.Components.Add(_rain);
                characterPlaceHolder = new(game);

            
                if (_content == null)
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");

                _gameFont = _content.Load<SpriteFont>("gamefont");

             

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            
                _spriteBatch = new SpriteBatch(game.GraphicsDevice);
                // _gameFont = game.Content.Load<SpriteFont>("File");

                gemPickup = game.Content.Load<SoundEffect>("gem_pickup");
                hit = game.Content.Load<SoundEffect>("hit");
                
                
                _tilemap = new Tilemap(Path.Combine(game.Content.RootDirectory, "test.tmx"), "tileset");
                _tilemap.LoadContent(game.Content);
                int mapW = _tilemap.WidthInPixels;
                int mapH = _tilemap.HeightInPixels;

                _solids = _tilemap.GetAllSolidRectangles("Object Layer 1");
                
                _interact_areas = _tilemap.GetRectangleObjects("Object Layer 1", "interact_area");

                _gemPoints = _tilemap.GetPoints("Gems", "Gem");

                foreach (Vector2 p in _gemPoints)
                {
                    gems.Add(new GemSprite(p));  
                }
                foreach(var gem in gems) gem.LoadContent(game.Content);
                

                characterPlaceHolder.LoadContent(game.Content);
                characterPlaceHolder.position = _tilemap.GetPoints("SpawnPoint", "Spawn")[0];
                characterPlaceHolder.WorldSize = new Vector2(mapW, mapH );

                _rain.Source = new Rectangle(0, 0, mapW, mapH);


                List<Vector2> orbPoints = _tilemap.GetPoints("GlowItems", "Bow");
                _relic = new GlowPickup(game, orbPoints[0]); 
                _relic.LoadContent(game.Content, game.GraphicsDevice);    


                rain = game.Content.Load<Song>("rain");
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.50f; 
                MediaPlayer.Play(rain);
            
            


        }


        public override void Deactivate()
        {

            base.Deactivate();
        }

        public override void Unload()
        {
            MediaPlayer.Stop();
            if (_rain != null) game.Components.Remove(_rain);
            _content.Unload();

        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            if (!IsActive) return;

            // // Gradually fade in or out depending on whether we are covered by the pause screen.
            // if (coveredByOtherScreen)
            //     _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            // else
            //     _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            // if (IsActive)
            // {
            //     // Apply some random jitter to make the enemy move around.
            //     const float randomization = 10;

            //     _enemyPosition.X += (float)(_random.NextDouble() - 0.5) * randomization;
            //     _enemyPosition.Y += (float)(_random.NextDouble() - 0.5) * randomization;

            //     // Apply a stabilizing force to stop the enemy moving off the screen.
            //     var targetPosition = new Vector2(
            //         ScreenManager.GraphicsDevice.Viewport.Width / 2 - _gameFont.MeasureString("Insert Gameplay Here").X / 2, 
            //         200);

            //     _enemyPosition = Vector2.Lerp(_enemyPosition, targetPosition, 0.05f);

            //     // This game isn't very fun! You could probably improve
            //     // it by inserting something more interesting in this space :-)
            // }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            game.Exit();//COME BACK CHANGE TO OPTIONS MENU OR PAUSE MENU

            var keys = Keyboard.GetState();
            if(keys.IsKeyDown(Keys.P) && _prevKeys.IsKeyUp(Keys.P))
            {
                SaveGame();
            }
            if(keys.IsKeyDown(Keys.L) && _prevKeys.IsKeyUp(Keys.L))
            {
                LoadGame();
            }
            if(keys.IsKeyDown(Keys.K) && _prevKeys.IsKeyUp(Keys.K))
            {
                DeleteSave();
            }

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


            foreach(var area in _interact_areas)
            {
                if (area.CollidesWith(characterPlaceHolder.Bounds))
                {
                    characterPlaceHolder.Color = Color.Green;
                    if(keys.IsKeyDown(Keys.E) && _prevKeys.IsKeyUp(Keys.E))
                    {
                        LoadingScreen.Load(ScreenManager, true, 0, new Level1Screen());
                        return;
                    }
                                    
                }
            }




            _relic.Update(gameTime); 
            if (!_relic.Collected && _relic.Bounds.CollidesWith(characterPlaceHolder.Bounds))
            {
                _relic.Collected = true;
                gemPickup.Play();
            }



            _prevKeys = keys;
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardState;
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            {

                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);

            }
            else
            {
                // Otherwise move the player position.
                var movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                var thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                

                if (movement.Length() > 1)
                    movement.Normalize();

                _playerPosition += movement * 8f;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(Color.CornflowerBlue);
            
            var viewport = game.GraphicsDevice.Viewport;
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
            characterPlaceHolder.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp,
                            blendState: BlendState.Additive,
                            transformMatrix: transform);

            _relic.DrawGlow(_spriteBatch);
            
            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_gameFont, $"Gems: {gemsCollected}", new Vector2(2, 2), Color.Black);
            _spriteBatch.DrawString(_gameFont, $"Press 'P' to Save", new Vector2(1650, 2), Color.Black);
            _spriteBatch.DrawString(_gameFont, $"Press 'L' to Load'", new Vector2(1650, 22), Color.Black);
            _spriteBatch.DrawString(_gameFont, $"Press 'K' to Delete Save", new Vector2(1650, 44), Color.Black);
            _spriteBatch.End();

            _relic.Draw(transform);

            _rain.Transform = transform;
        }


        private void SaveGame()
        {
            var save = new JsonSave(characterPlaceHolder.position, gemsCollected, gems, _gemPoints);
            save.SaveToFile();
        }

        private void LoadGame()
        {
            var save = JsonSave.LoadFromFile();
            if (save == null) return;

            characterPlaceHolder.position = save.PlayerPosition;
            gemsCollected = save.GemsCollected;

            // Match saved gems to the ones loaded from the map by position
            foreach (var saved in save.Gems)
            {
                for (int i = 0; i < _gemPoints.Count; i++)
                {
                    if (Math.Abs(_gemPoints[i].X - saved.X) < 0.01f &&
                        Math.Abs(_gemPoints[i].Y - saved.Y) < 0.01f)
                    {
                        gems[i].Collected = saved.Collected;
                        break;
                    }
                }
            }
        }

        private void DeleteSave()
        {
            JsonSave.DeleteSave();
        }

    }


    
}