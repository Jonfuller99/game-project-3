// Adapted from the Game State Management sample from Microsoft XNA Game Studio 4.0
// Archived at https://github.com/SimonDarksideJ/GameStateManagementSample

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Input;

public class InputState : GameComponent, IInputState
{

    // The default maximum number of player devices
    // Typically 4 controllers
    private const int MaxInputs = 4;

    // Current GamePad and Keyboard states
    public readonly GamePadState[] CurrentGamePadStates;
    public KeyboardState CurrentKeyboardState;

    // Prior frame GamePad and Keyboard states
    private readonly GamePadState[] _priorGamePadStates;
    private KeyboardState _priorKeyboardState;

    /// <summary>
    /// Tracks wether a GamePad was ever connected for each player index
    /// </summary>
    public readonly bool[] GamePadWasConnected;

    /// <summary>
    /// Initializes a new instance of the InputState class.
    /// </summary>
    public InputState(Game game) : base(game)
    {
        CurrentGamePadStates = new GamePadState[MaxInputs];
        _priorGamePadStates = new GamePadState[MaxInputs];
        
        GamePadWasConnected = new bool[MaxInputs];


        UpdateOrder = 0;
    }

    /// <summary>
    /// Updates the input state for the current frame.
    /// </summary>
    public override void Update(GameTime gameTime)
    {
        for (var i = 0; i < MaxInputs; i++)
        {
            _priorGamePadStates[i] = CurrentGamePadStates[i];
            CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

            if (CurrentGamePadStates[i].IsConnected)
                GamePadWasConnected[i] = true;
        }

        _priorKeyboardState = CurrentKeyboardState;
        CurrentKeyboardState = Keyboard.GetState();
    }

    /// <summary>
    /// Determines if a key was newly pressed during this frame.
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if pressed, False otherwise</returns>
    public bool IsKeyPressed(Keys key)
    {
        return CurrentKeyboardState.IsKeyDown(key);
    }

    /// <summary>
    /// Determines if a button was newly pressed during this frame.
    /// </summary>
    /// <param name="button">The button to check for</param>
    /// <param name="controllingPlayer">An optional player index to check</param>
    /// <param name="playerIndex">The player index the button was pressed by</param>
    /// <returns>True if pressed, False otherwise</returns>
    public bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
        // If a player index was specified, check if that player pressed the button
        if (controllingPlayer.HasValue)
        {
            playerIndex = controllingPlayer.Value;
            int i = (int)playerIndex;
            return CurrentGamePadStates[i].IsButtonDown(button);
        }

        // Otherwise, check all players for the button press
        return IsButtonPressed(button, PlayerIndex.One, out playerIndex) ||
               IsButtonPressed(button, PlayerIndex.Two, out playerIndex) ||
               IsButtonPressed(button, PlayerIndex.Three, out playerIndex) ||
               IsButtonPressed(button, PlayerIndex.Four, out playerIndex);
    }

    /// <summary>
    /// Determines if a key was newly pressed during this frame.
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if pressed, False otherwise</returns>
    public bool IsNewKeyPress(Keys key)
    {
        return CurrentKeyboardState.IsKeyDown(key) && _priorKeyboardState.IsKeyUp(key);
    }

    /// <summary>
    /// Determines if a button was newly pressed during this frame.
    /// </summary>
    /// <param name="button"></param>
    /// <param name="controllingPlayer"></param>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
        // If a player index was specified, check if that player pressed the button
        if (controllingPlayer.HasValue)
        {
            playerIndex = controllingPlayer.Value;
            int i = (int)playerIndex;
            return CurrentGamePadStates[i].IsButtonDown(button) && _priorGamePadStates[i].IsButtonUp(button);
        }

        // Otherwise, check all players for the button press
        return IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
               IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
               IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
               IsNewButtonPress(button, PlayerIndex.Four, out playerIndex);
    }
}