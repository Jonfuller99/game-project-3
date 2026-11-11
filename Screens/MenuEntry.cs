using System;
using Input;
using GameScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Screens
{
    // Helper class represents a single entry in a MenuScreen. By default this
    // just draws the entry text string, but it can be customized to display menu
    // entries in different ways. This also provides an event that will be raised
    // when the menu entry is selected.
    public class MenuEntry
    {
        private string _text;
        private float _selectionFade;    // Entries transition out of the selection effect when they are deselected
        private Vector2 _position;    // This is set by the MenuScreen each frame in Update


        public Vector2 Size { get; set; } = new Vector2(400, 90);

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

        public virtual int GetWidth(MenuScreen screen) => (int)Size.X;
        public virtual int GetHeight(MenuScreen screen) => (int)Size.Y;
        public string Text
        {
            private get => _text;
            set => _text = value;
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public event EventHandler<PlayerIndexEventArgs> Selected;
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            Selected?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
        }

        public MenuEntry(string text)
        {
            _text = text;
        }

        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {

            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1);
            else
                _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0);
        }


        // This can be overridden to customize the appearance.
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {

            // var color = isSelected ? Color.Yellow : Color.White;

            // // Pulsate the size of the selected menu entry.
            // double time = gameTime.TotalGameTime.TotalSeconds;
            // float pulsate = (float)Math.Sin(time * 6) + 1;
            // float scale = 2 + pulsate * 0.05f * _selectionFade;

            // // Modify the alpha to fade text out during transitions.
            // color *= screen.TransitionAlpha;

            // // Draw text, centered on the middle of each line.
            // var screenManager = screen.ScreenManager;
            // var spriteBatch = screenManager.SpriteBatch;
            // var font = screenManager.MenuFont;

            // var origin = new Vector2(0, font.LineSpacing / 2);

            // spriteBatch.DrawString(font, _text, _position, color, 0,
            //     origin, scale, SpriteEffects.None, 0);

            var screenManager = screen.ScreenManager;
            var spriteBatch = screenManager.SpriteBatch;
            var font = screenManager.MenuFont;

            Color fill = isSelected ? new Color(255, 190, 0) : new Color(40, 40, 40);
            Color textColor = isSelected ? Color.Black : Color.White;

            // button background (BlankTexture = 1x1 white texture; create one if your manager lacks it)
            spriteBatch.Draw(screenManager.BlankTexture, Bounds, fill * 0.9f * screen.TransitionAlpha);

            // optional border
            int b = 3;
            var border = Color.White * screen.TransitionAlpha;
            spriteBatch.Draw(screenManager.BlankTexture, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, b), border);
            spriteBatch.Draw(screenManager.BlankTexture, new Rectangle(Bounds.X, Bounds.Bottom - b, Bounds.Width, b), border);
            spriteBatch.Draw(screenManager.BlankTexture, new Rectangle(Bounds.X, Bounds.Y, b, Bounds.Height), border);
            spriteBatch.Draw(screenManager.BlankTexture, new Rectangle(Bounds.Right - b, Bounds.Y, b, Bounds.Height), border);

            // centered text (Text is your entry's label property)
            Vector2 textSize = font.MeasureString(Text);
            Vector2 textPos = Position + (Size - textSize) / 2f;
            spriteBatch.DrawString(font, Text, textPos, textColor * screen.TransitionAlpha);
        }

        // public virtual int GetHeight(MenuScreen screen)
        // {
        //     return screen.ScreenManager.MenuFont.LineSpacing;
        // }

        // public virtual int GetWidth(MenuScreen screen)
        // {
        //     return (int)screen.ScreenManager.MenuFont.MeasureString(Text).X;
        // }
    }
}