using MysticEchoes.Core.Input;
using MysticEchoes.Core.MapModule;
using SharpGL;

namespace MysticEchoes.Core.Configuration;

public class Environment
{
    public Environment(OpenGL openGl, IInputManager inputManager, IMazeGenerator mazeGenerator)
    {
        OpenGl = openGl;
        InputManager = inputManager;
        MazeGenerator = mazeGenerator;
    }
    
    public readonly OpenGL OpenGl;
    public readonly IInputManager InputManager;
    public readonly IMazeGenerator MazeGenerator;
}