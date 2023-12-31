using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders.Assets;
using MysticEchoes.Core.Loaders.Prefabs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MysticEchoes.Core.Loaders.Implementation;

public class JsonLoader : IDataLoader
{
    public Dictionary<AssetType, string> LoadTexturePaths()
    {
        return Load<Dictionary<AssetType, string>>(Environment.CurrentDirectory + "\\Config\\Json\\texture-paths.json");
    }

    public Dictionary<PrefabType, Prefab> LoadPrefabs()
    {
        return Load<Dictionary<PrefabType, Prefab>>(Environment.CurrentDirectory + "\\Config\\Json\\prefabs.json");
    }

    public Settings LoadSettings()
    {
        return Load<Settings>(Environment.CurrentDirectory + "\\Config\\Json\\game-settings.json");
    }
    
    public Dictionary<string, AnimationFrame[]> LoadAnimations()
    {
        return Load<Dictionary<string, AnimationFrame[]>>(Environment.CurrentDirectory + "\\Config\\Json\\animations.json");
    }

    public object LoadObject(object objectValue, Type objectType)
    {
        object result;
        if (objectValue is JToken token)
        {
            string json = token.ToString();
            result = JsonConvert.DeserializeObject(json, objectType);
        }
        else
        {
            throw new ArgumentException("Unknown object type");
        }

        if (result == null)
        {
            throw new InvalidOperationException("Can't deserialize json");
        }

        return result;
    }

    private T Load<T>(string jsonPath)
    {
        T result = default(T);

        if (string.IsNullOrEmpty(jsonPath))
            throw new ArgumentException("File path can't be empty.", nameof(jsonPath));

        if (!File.Exists(jsonPath))
            throw new FileNotFoundException("Json file is not found.", jsonPath);

        string json = File.ReadAllText(jsonPath);
        result = JsonConvert.DeserializeObject<T>(json);
        if (result == null)
        {
            throw new InvalidOperationException("Can't deserialize json .");
        }

        return result;
    }
}