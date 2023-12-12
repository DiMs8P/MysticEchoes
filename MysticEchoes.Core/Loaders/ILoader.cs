using MysticEchoes.Core.Configuration;

namespace MysticEchoes.Core.Loaders;

public interface ILoader
{
    Dictionary<string, string> LoadTexturePaths();
    Settings LoadSettings();
}