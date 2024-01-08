namespace MysticEchoes.Core.Events;

public class OnEnemyDeadInfo
{
    public int EnemyId { get; set; }
    public int EntityId { get; set; }
    public int RoomId { get; set; }
}