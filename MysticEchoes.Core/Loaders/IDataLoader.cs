﻿using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders.Prefabs;

namespace MysticEchoes.Core.Loaders;

public interface IDataLoader
{
    Dictionary<string, string> LoadTexturePaths();
    Dictionary<PrefabType, Prefab> LoadPrefabs();
    Settings LoadSettings();
    Dictionary<string, AnimationFrame[]> LoadAnimations();
    
    object LoadObject(object objectValue, Type objectType);
}