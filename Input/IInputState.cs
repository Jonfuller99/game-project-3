using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Input;


public interface IInputState{
    bool IsKeyPressed(Keys key);

    bool IsNewKeyPress(Keys key);

    bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex);

    bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex);
}