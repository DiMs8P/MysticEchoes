using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders.Assets;
using MysticEchoes.Core.Loaders.Prefabs;

namespace MysticEchoes.Core.Loaders;

public interface IDataLoader
{
    Dictionary<AssetType, string> LoadTexturePaths();
    Dictionary<PrefabType, Prefab> LoadPrefabs();
    Settings LoadSettings();

    object LoadObject(object objectValue, Type objectType);
}