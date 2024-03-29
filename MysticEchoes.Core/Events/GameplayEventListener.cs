﻿namespace MysticEchoes.Core.Events;

public class GameplayEventListener
{
    public delegate void OnPlayerDead(OnPlayerDeadInfo info);
    public event OnPlayerDead OnPlayerDeadEvent;

    public delegate void OnEnemyDead(OnEnemyDeadInfo info);
    public event OnEnemyDead OnEnemyDeadEvent;

    public delegate void OnLastEnemyDead();
    public event OnLastEnemyDead OnLastEnemyDeadEvent;

    public void InvokeOnPlayerDead(OnPlayerDeadInfo info)
    {
        OnPlayerDead handler = OnPlayerDeadEvent;
        handler?.Invoke(info);
    }
    
    public void InvokeOnEnemyDead(OnEnemyDeadInfo info)
    {
        OnEnemyDead handler = OnEnemyDeadEvent;
        handler?.Invoke(info);
    }

    public void InvokeLastEnemyDead()
    {
        OnLastEnemyDeadEvent?.Invoke();
    }
}