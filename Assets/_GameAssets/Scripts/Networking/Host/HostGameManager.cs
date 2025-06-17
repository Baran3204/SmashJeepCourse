using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class HostGameManager : IDisposable
{
    public NetworkServer NetworkServer { get; private set; }
    private Allocation _allocation;
    private string _joinCode;
    private string _lobbyId;
    public async UniTask StartHostAsync()
    {
        try
        {
            _allocation = await RelayService.Instance.CreateAllocationAsync(4);
        }
        catch (Exception Exception)
        {
            Debug.LogError(Exception);
            return;
        }

        try
        {
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log(_joinCode);
        }
        catch (Exception exc)
        {
            Debug.LogError(exc);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(AllocationUtils.ToRelayServerData(_allocation, "dtls"));

        try
        {

            CreateLobbyOptions createLobbyOptions = new();
            createLobbyOptions.IsPrivate = false;
            createLobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject
                    (
                        visibility: DataObject.VisibilityOptions.Member,
                        value: _joinCode
                    )
                }
            };

            string PlayerName = PlayerPrefs.GetString("PlayerName", "NoName");
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync($"{PlayerName}'s Lobby", 4, createLobbyOptions);
            _lobbyId = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15));
        }
        catch (LobbyServiceException exc)
        {
            Debug.LogError(exc);
            return;
        }

        NetworkServer networkServer = new(NetworkManager.Singleton);

        UserData userData = new UserData
        {
            UserName = PlayerPrefs.GetString("PlayerName", "NoName"),
            UserAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private IEnumerator HeartBeatLobby(float time)
    {
        WaitForSecondsRealtime delay = new(time);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(_lobbyId);
            yield return delay;
        }
    }

    public async void Shutdown()
    {
        HostSingleton.Instance.StopCoroutine(nameof(HeartBeatLobby));

        if (!string.IsNullOrEmpty(_lobbyId))
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(_lobbyId);
            }
            catch (LobbyServiceException exc)
            {
                Debug.Log(exc);
            }

            _lobbyId = string.Empty;
        }

        NetworkServer?.Dispose();
    }
    public void Dispose()
    {
        Shutdown();
    }
}
