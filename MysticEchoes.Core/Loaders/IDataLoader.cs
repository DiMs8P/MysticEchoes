using MysticEchoes.Core.Configuration;

namespace MysticEchoes.Core.Loaders;

public interface IDataLoader
{
    Dictionary<string, string> LoadTexturePaths();
    Settings LoadSettings();
}