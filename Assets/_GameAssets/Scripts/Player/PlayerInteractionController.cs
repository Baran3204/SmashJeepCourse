using Unity.Netcode;
using UnityEngine;

public class PlayerInteractionController : NetworkBehaviour
{
    private PlayerSkillController _playerSkillController;
    private PlayerVehicleController _playerVehicleController;
    private bool _isCrashed;
    private bool _isSpikeActive;
    private bool _isShieldActive;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerVehicleController = GetComponent<PlayerVehicleController>();

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
            D.Damage(_playerVehicleController);
            SetKillerUIRpc(D.GetKillerClientİd(), RpcTarget.Single(D.GetKillerClientİd(), RpcTargetUse.Temp));
            SpawnerManager.Instance.RespwanPlayer(D.GetRespawnTimer(), OwnerClientId);
        }
    }


    [Rpc(SendTo.SpecifiedInParams)]
    private void SetKillerUIRpc(ulong killerClientID, RpcParams rpcParams)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientID, out var killerClient))
        {
            KillScreenUI.Instance.SetSmashUI("SkinnyDev");
        }
    }
    public void SetShieldActive(bool active) => _isShieldActive = active;
    public void SetSpikeActive(bool active) => _isSpikeActive = active;
    public void OnPlayerRespawned()
    {
        enabled = true;
        _isCrashed = false;
    }
}
