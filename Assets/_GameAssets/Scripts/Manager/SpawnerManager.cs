using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class SpawnerManager : NetworkBehaviour
{
    public static SpawnerManager Instance;
    [SerializeField] private GameObject _playerPrefab;

    [Header("References")]
    [SerializeField] private List<Transform> _spawnPointTransformList;
    [SerializeField] private List<Transform> _respawnPointTransformList;

    private List<int> _availableSpawnIndexList = new();
    private List<int> _availableRespawnIndexList = new();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        for (int i = 0; i < _spawnPointTransformList.Count; i++)
        {
            _availableSpawnIndexList.Add(i);
        }

        for (int i = 0; i < _respawnPointTransformList.Count; i++)
        {
            _availableRespawnIndexList.Add(i);
        }

        NetworkManager.OnClientConnectedCallback += SpawnPlayer;
    }

    private void Awake()
    {
        Instance = this;    
    }

    private void SpawnPlayer(ulong clientID)
    {
        if (_availableSpawnIndexList.Count == 0)
        {
            Debug.Log("SİKEEEEE ");
            return;
        }
        int rndIndex = Random.Range(0, _availableSpawnIndexList.Count);
        int spawnIndex = _availableSpawnIndexList[rndIndex];
        _availableSpawnIndexList.RemoveAt(rndIndex);

        Transform spawnPoint = _spawnPointTransformList[spawnIndex];
        GameObject player = Instantiate(_playerPrefab, spawnPoint.position, spawnPoint.rotation);

        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID);
    }

    public void RespwanPlayer(int timer, ulong clientId)
    {
        StartCoroutine(RespawnPlayerCournite(timer, clientId));
    }

    private IEnumerator RespawnPlayerCournite(int respawnTimer, ulong clientId)
    {
        yield return new WaitForSeconds(respawnTimer);

        if (GameManager.Instance.GetCurrentGameState() != GameState.Playing) yield break;

        if (_respawnPointTransformList.Count == 0)
        {
            Debug.Log("SİKEEEEE ");
            yield break;
        }
        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
        {
            Debug.Log($"Client {clientId} not found braaa");
            yield break;
        }

        if (_availableRespawnIndexList.Count == 0)
        {
            for (int i = 0; i < _respawnPointTransformList.Count; i++)
            {
                _availableRespawnIndexList.Add(i);
            }
        }

        int rndIndex = Random.Range(0, _availableRespawnIndexList.Count);
        int respawnIndex = _availableRespawnIndexList[rndIndex];
        _availableRespawnIndexList.RemoveAt(rndIndex);

        var rPoint = _respawnPointTransformList[respawnIndex];
        NetworkObject netObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        if (netObj == null)
        {
            Debug.LogError("Player Network Object is Null");
            yield break;
        }

        if (netObj.TryGetComponent<Rigidbody>(out var PlayerRb))
        {
            PlayerRb.isKinematic = true;
        }

        if (netObj.TryGetComponent<NetworkTransform>(out var playerNetTr))
        {
            playerNetTr.Interpolate = false;
            netObj.GetComponent<PlayerVehicleVisualController>().SetvehicleActiveCourtine(0.1f);
        }

        netObj.transform.SetPositionAndRotation(rPoint.position, rPoint.rotation);

        yield return new WaitForSeconds(0.1f);

        PlayerRb.isKinematic = false;
        playerNetTr.Interpolate = true;

        if (netObj.TryGetComponent<PlayerNetworkController>(out var playerNetworkController))
        {
            playerNetworkController.OnPlayerRespawned();
        }
    }
}
