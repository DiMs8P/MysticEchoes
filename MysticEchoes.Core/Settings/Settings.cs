using MysticEchoes.Core.Loaders;

namespace MysticEchoes.Core.Configuration;

public class Settings
{
    public WeaponsSettings WeaponsSettings { get; set; }
    public ItemsSettings ItemsSettings { get; set; }
    public EnemySettings EnemySettings { get; set; }
}