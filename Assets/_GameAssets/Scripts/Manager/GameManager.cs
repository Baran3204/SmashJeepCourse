using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public event Action<GameState> OnGameStateChanged;
    [SerializeField] private GameDataSO _gameData;
    [SerializeField] private GameState _currentGameState;
    private NetworkVariable<int> _gameTimer = new NetworkVariable<int>(0);


    private void Awake()
    {
        Instance = this;    
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _gameTimer.Value = _gameData.GameTimer;
            SetTimerUIRpc();
            InvokeRepeating(nameof(DecreaseTimer), 1f, 1f);
        }

        _gameTimer.OnValueChanged += OnValueChanged;
    }

    private void OnValueChanged(int previousValue, int newValue)
    {
        TimerUI.Instance.SetTimerText(newValue);

        if (IsServer && newValue <= 0)
        {
            ChangeGameState(GameState.GameOver);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetTimerUIRpc()
    {
        TimerUI.Instance.SetTimerText(_gameTimer.Value);
    }

    private void DecreaseTimer()
    {
        if (IsServer && _currentGameState == GameState.Playing)
        {
            _gameTimer.Value--;

            if (_gameTimer.Value <= 0)
            {
                CancelInvoke(nameof(DecreaseTimer));
            }
        }
    }

    public void ChangeGameState(GameState gameState)
    {
        if (IsServer)
        {
            _currentGameState = gameState;
            ChangeGameStateRpc(gameState);
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeGameStateRpc(GameState gameState)
    {
        _currentGameState = gameState;
        OnGameStateChanged?.Invoke(gameState);

    }

    public GameState GetCurrentGameState()
    {
        return _currentGameState;
    }
} 
