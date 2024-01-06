namespace MysticEchoes.Core.AI.Factories;

public interface IEnemyFactory
{
    public int Create(EnemyInitializationInfo enemyInitializationInfo);
}