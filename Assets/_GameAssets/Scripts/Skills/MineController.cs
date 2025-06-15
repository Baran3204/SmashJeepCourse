using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class MineController : NetworkBehaviour, IDamagables
{
    [SerializeField] private Collider _mineCollider;
    [SerializeField] private float _fallsSpeed;
    [SerializeField] private float _rayDistance;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private MysteryBoxSkillsSO mysteryBoxSkillsSO;

    private bool _hasLanded;
    private Vector3 _lastSendPos;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        SetOwnerVisualsRpc();

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var client))
        {
            NetworkObject ownerNetObj = client.PlayerObject;
            PlayerVehicleController playerVehicleController = ownerNetObj.GetComponent<PlayerVehicleController>();
            playerVehicleController.OnVehicleCrashed += OnVehicleCrashed;
        }
    }

    private void OnVehicleCrashed()
    {
        DestroyRpc();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var client))
        {
            NetworkObject ownerNetObj = client.PlayerObject;
            PlayerVehicleController playerVehicleController = ownerNetObj.GetComponent<PlayerVehicleController>();
            playerVehicleController.OnVehicleCrashed -= OnVehicleCrashed;
        }
    }

    private void Update()
    {
        if (!IsServer && _hasLanded) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit ht, _rayDistance, _groundLayer))
        {
            _hasLanded = true;
            transform.position = ht.point + new Vector3(0f, 0.1f, 0f);
            if (_lastSendPos != transform.position)
            {
                SyncPosRpc(transform.position);
                _lastSendPos = transform.position;
            }
        }
        else
        {
            transform.position += _fallsSpeed * Vector3.down * Time.deltaTime;
            if (_lastSendPos != transform.position)
            {
                SyncPosRpc(transform.position);
                _lastSendPos = transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ShieldControlle s))
        {
            DestroyRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SyncPosRpc(Vector3 pos)
    {
        if (IsServer) return;
        transform.position = pos;
    }

    [Rpc(SendTo.Owner)]
    private void SetOwnerVisualsRpc()
    {
        _mineCollider.enabled = false;
    }

    public void Damage(PlayerVehicleController playerVehicleController)
    {
        playerVehicleController.CrashVehicle();
        KillScreenUI.Instance.SetSmashedUI("Baran3204", mysteryBoxSkillsSO.SkillData.RespawnTimer);
        DestroyRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyRpc()
    {
        if (IsServer) Destroy(gameObject);
    }

    public ulong GetKillerClientİd()
    {
        return OwnerClientId;
    }
    
    public int GetRespawnTimer()
    {
        return mysteryBoxSkillsSO.SkillData.RespawnTimer;
    }
}
