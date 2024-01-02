using MysticEchoes.Core.Loaders;

namespace MysticEchoes.Core.Configuration;

public class Settings
{
    public PlayerSettings PlayerSettings { get; set; }
    public WeaponsSettings WeaponsSettings { get; set; }
    public ItemsSettings ItemsSettings { get; set; }
}