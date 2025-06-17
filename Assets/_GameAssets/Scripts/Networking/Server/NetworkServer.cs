using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private Dictionary<ulong, string> _clientIdAuthDic = new();
    private Dictionary<string, UserData> _authIdUserDataDic = new();
    private NetworkManager _networkManager;
    public NetworkServer(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnServerReady;
    }

    private void OnServerReady()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        if (_clientIdAuthDic.TryGetValue(clientId, out string authId))
        {
            _clientIdAuthDic.Remove(clientId);
            _authIdUserDataDic.Remove(authId);
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        _clientIdAuthDic[request.ClientNetworkId] = userData.UserAuthId;
        _authIdUserDataDic[userData.UserAuthId] = userData;
        response.Approved = true;
        response.CreatePlayerObject = true;
    }

    public void Dispose()
    {
        if (_networkManager == null) return;

        _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        _networkManager.OnServerStarted -= OnServerReady;
        _networkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;

        if (_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }
}

[Serializable]
public class UserData
{
    public string UserName;
    public string UserAuthId;
}
