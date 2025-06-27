using System;
using JetBrains.Annotations;
using TMPro;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    public static event Action<PlayerNetworkController> OnPlayerSpawned;
    public static event Action<PlayerNetworkController> OnPlayerDespawned;
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private TMP_Text _playerNameText;
    private PlayerVehicleController _playerVehicleController;
    private PlayerSkillController _playerSkillController;
    private PlayerInteractionController _playerInteractionController;
    [SerializeField] private PlayerScoreController _playerScoreController;
    public NetworkVariable<FixedString32Bytes> PlayerName = new();
    public override void OnNetworkSpawn()
    {
        _camera.gameObject.SetActive(IsOwner);

        if (IsServer)
        {
            UserData userData = HostSingleton.Instance._hostGameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            PlayerName.Value = userData.UserName;
            SetPlayerNameRpc();

            OnPlayerSpawned?.Invoke(this);
        }
        if (!IsOwner) return;

        _playerVehicleController = GetComponent<PlayerVehicleController>();
        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerInteractionController = GetComponent<PlayerInteractionController>();
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void SetPlayerNameRpc()
    {
        _playerNameText.text = PlayerName.Value.ToString();
    }
    public PlayerScoreController GetPlayerScoreController()
    {
        return _playerScoreController;
    }
    public void OnPlayerRespawned()
    {
        _playerVehicleController.OnPlayerRespawned();
        _playerSkillController.OnPlayerRespawned();
        _playerInteractionController.OnPlayerRespawned();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}
