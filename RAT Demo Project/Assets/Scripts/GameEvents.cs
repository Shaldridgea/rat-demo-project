using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents EventBus { get; private set; }
    
    private GameEvents(){ }

    private void Awake()
    {
        MakeSingleton();
    }

    private void MakeSingleton()
    {
        if (EventBus == null)
            EventBus = this;
        else
            Destroy(gameObject);
    }

    public enum GameEventType
    {   
        [InspectorName("Game Started")]
        GAME_STARTED,
        [InspectorName("Player Low Health")]
        PLAYER_LOW_HEALTH,
        [InspectorName("Player Died")]
        PLAYER_DIED
    }

    public delegate void GameEventDelegate();

    public event GameEventDelegate GameStarted;

    public event GameEventDelegate PlayerLowHealth;

    public event GameEventDelegate PlayerDied;

    public void TriggerGameStarted() => GameStarted?.Invoke();

    public void TriggerPlayerLowHealth() => PlayerLowHealth?.Invoke();

    public void TriggerPlayerDied() => PlayerDied?.Invoke();

    public void RegisterListener(GameEventType eventType, GameEventDelegate listener)
    {
        switch (eventType)
        {
            case GameEventType.GAME_STARTED:
                GameStarted += listener;
                break;

            case GameEventType.PLAYER_LOW_HEALTH:
                PlayerLowHealth += listener;
                break;

            case GameEventType.PLAYER_DIED:
                PlayerDied += listener;
                break;
        }
    }

    public void DeregisterListener(GameEventType eventType, GameEventDelegate listener)
    {
        switch (eventType)
        {
            case GameEventType.GAME_STARTED:
                GameStarted -= listener;
                break;

            case GameEventType.PLAYER_LOW_HEALTH:
                PlayerLowHealth -= listener;
                break;

            case GameEventType.PLAYER_DIED:
                PlayerDied -= listener;
                break;
        }
    }
}
