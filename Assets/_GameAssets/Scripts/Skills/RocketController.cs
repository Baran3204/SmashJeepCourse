using Unity.Netcode;
using UnityEngine;
public class RocketController : NetworkBehaviour, IDamagables
{
    [SerializeField] private Collider _rocketCollider;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private MysteryBoxSkillsSO mysteryBoxSkillsSO;
    private bool _isMoving;

    [Rpc(SendTo.Owner)]
    private void SetOwnerVisualsRpc()
    {
        _rocketCollider.enabled = false;
    }
    private void Update()
    {
        if (IsServer && _isMoving)
        {
            MoveRocket();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ShieldControlle s))
        {
            DestroyRpc();
        }
    }
    public override void OnNetworkSpawn()
    {
        RequestForMovementRpc();
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
    [Rpc(SendTo.Server)]
    private void RequestForMovementRpc()
    {
        _isMoving = true;
    }
    private void MoveRocket()
    {
        transform.position += _movementSpeed * transform.forward * Time.deltaTime;
        transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime, Space.Self);
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
