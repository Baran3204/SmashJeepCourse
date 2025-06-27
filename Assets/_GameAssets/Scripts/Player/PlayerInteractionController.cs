using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractionController : NetworkBehaviour
{
    private PlayerSkillController _playerSkillController;
    private PlayerVehicleController _playerVehicleController;
    private PlayerHealthController _playerHealthController;
    private PlayerNetworkController _playerNetworkController;
    private bool _isCrashed;
    private bool _isSpikeActive;
    private bool _isShieldActive;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerVehicleController = GetComponent<PlayerVehicleController>();
        _playerHealthController = GetComponent<PlayerHealthController>();
        _playerNetworkController = GetComponent<PlayerNetworkController>();

        _playerVehicleController.OnVehicleCrashed += () =>
        {
            enabled = false;
            _isCrashed = true;
        };
    }
    private void OnTriggerEnter(Collider other)
    {
        CheckCollision(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckCollision(other);
    }

    private void CheckCollision(Collider other)
    {
        if (!IsOwner) return;
        if (_isCrashed) return;
        if (GameManager.Instance.GetCurrentGameState() != GameState.Playing) return;
        if (other.gameObject.TryGetComponent<ICollectible>(out ICollectible C))
        {
            C.Collect(_playerSkillController);
        }
        if (other.gameObject.TryGetComponent<IDamagables>(out IDamagables D))
        {
            if (_isShieldActive)
            {
                Debug.Log("SHİELD ACTİVE HAHAHHA");
                return;
            }
            var playerName = _playerNetworkController.PlayerName.Value;
            D.Damage(_playerVehicleController, D.GetKillerName());
            _playerHealthController.TakeDamage(D.GetDamageAmount());
            SetKillerUIRpc(D.GetKillerClientİd(), playerName.ToString(), RpcTarget.Single(D.GetKillerClientİd(), RpcTargetUse.Temp));
            SpawnerManager.Instance.RespwanPlayer(D.GetRespawnTimer(), OwnerClientId);
        }
    }


    [Rpc(SendTo.SpecifiedInParams)]
    private void SetKillerUIRpc(ulong killerClientID, FixedString32Bytes playerName, RpcParams rpcParams)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientID, out var killerClient))
        {
            KillScreenUI.Instance.SetSmashUI(playerName.ToString());
            killerClient.PlayerObject.GetComponent<PlayerScoreController>().AddScore(1);
        }
    }
    public void SetShieldActive(bool active) => _isShieldActive = active;
    public void SetSpikeActive(bool active) => _isSpikeActive = active;
    public void OnPlayerRespawned()
    {
        enabled = true;
        _isCrashed = false;
        _playerHealthController.RestartHelth();
    }
}
