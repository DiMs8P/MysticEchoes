using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders.Prefabs;

namespace MysticEchoes.Core.Loaders;

public interface IDataLoader
{
    Dictionary<string, string> LoadTexturePaths();
    Dictionary<string, Prefab> LoadPrefabs();
    Settings LoadSettings();
}