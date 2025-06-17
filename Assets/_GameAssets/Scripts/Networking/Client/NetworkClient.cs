using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager _networkManager;

    public NetworkClient(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId != 0 && clientId != _networkManager.LocalClientId) return;

        Disconnet();
    }

    private void Disconnet()
    {
        if (SceneManager.GetActiveScene().name != "MenuScene")
        {
            SceneManager.LoadScene("MenuScene");
        }

        if (_networkManager.IsConnectedClient)
        {
            _networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if (_networkManager == null) return;

        _networkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;

        if (_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }
}
