using MysticEchoes.Core.Input;
using MysticEchoes.Core.Loaders;
using SharpGL;

namespace MysticEchoes.Core.Assets;

public class AssetManager
{
    private OpenGL _gl;
    private Dictionary<string, string> _data;
    
    public AssetManager(ILoader dataLoader)
    {
        _data = dataLoader.LoadTexturePaths();
    }
    
    public void InitializeGl(OpenGL gl)
    {
        _gl = gl;
    }
}