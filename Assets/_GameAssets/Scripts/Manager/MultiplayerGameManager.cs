    using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class MultiplayerGameManager : NetworkBehaviour
{
    public static MultiplayerGameManager Instance;
    public event Action OnPlayerDataNetworkListChanged;
    [SerializeField] private List<Color> _colorLists = new();
    private NetworkList<PlayerDataSerilezabe> _playerDataNetworkList = new();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerDataNetworkList.OnListChanged += (value) =>
        {
            OnPlayerDataNetworkListChanged?.Invoke();
        };
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _playerDataNetworkList.Clear();

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        }
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        foreach (var data in _playerDataNetworkList)
        {
            if (data.ClientID == clientId)
            {
                _playerDataNetworkList.Remove(data);
            }
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        foreach (var data in _playerDataNetworkList)
        {
            if (data.ClientID == clientId)
            {
                _playerDataNetworkList.Remove(data);
            }
        }

        _playerDataNetworkList.Add(new PlayerDataSerilezabe
        {
            ClientID = clientId,
            ColorID = GetFirsUnusedColorId()
        });
    }
    [Rpc(SendTo.Server)]
    private void ChangePlayerColorRpc(int colorId, RpcParams rpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            // COLOR NOT AVAİLABLE
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientID(rpcParams.Receive.SenderClientId);

        PlayerDataSerilezabe playerData = _playerDataNetworkList[playerDataIndex];

        playerData.ColorID = colorId;

        _playerDataNetworkList[playerDataIndex] = playerData;
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorRpc(colorId);
    }

    private int GetPlayerDataIndexFromClientID(ulong clientİd)
    {
        for (int i = 0; i < _playerDataNetworkList.Count; i++)
        {
            if (_playerDataNetworkList[i].ClientID == clientİd)
            {
                return i;
            }
        }

        return -1;
    }
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < _playerDataNetworkList.Count;
    }

    public PlayerDataSerilezabe GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return _playerDataNetworkList[playerIndex];
    }

    public Color GetPlayerColor(int colorIndex)
    {
        return _colorLists[colorIndex];
    }

    public int GetFirsUnusedColorId()
    {
        for (int i = 0; i < _colorLists.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }

        return -1;
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach (var e in _playerDataNetworkList)
        {
            if (e.ColorID == colorId)
            {
                // COLOR ALREADY USİNG
                return false;
            }
        }

        return true;
    }

    public PlayerDataSerilezabe GetPlayerDataFromClientId(ulong clientİd)
    {
        foreach (var e in _playerDataNetworkList)
        {
            if (e.ClientID == clientİd)
            {
                return e;
            }

        }

        return default;
    }
    public PlayerDataSerilezabe GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        OnClientDisconnectCallback(clientId);
    }

    public override void OnNetworkDespawn()
    {
       NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
       NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    }
}
