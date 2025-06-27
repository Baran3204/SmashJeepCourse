using Unity.Netcode;
using UnityEngine;
public class RocketController : NetworkBehaviour, IDamagables
{
    [SerializeField] private Collider _rocketCollider;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private MysteryBoxSkillsSO mysteryBoxSkillsSO;
    [SerializeField] private GameObject _explosionParticlesPrefab;
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
            DestroyRpc(true, s.transform.position);
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
        DestroyRpc(false);
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

    public void Damage(PlayerVehicleController playerVehicleController, string playerName)
    {
        playerVehicleController.CrashVehicle();
        KillScreenUI.Instance.SetSmashedUI(playerName, mysteryBoxSkillsSO.SkillData.RespawnTimer);
        DestroyRpc(true, playerVehicleController.transform.position);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyRpc(bool isExp, Vector3 vehiclePos = default)
    {
        if (IsServer)
        {
            if (isExp)
            {
                GameObject exp = Instantiate(_explosionParticlesPrefab, vehiclePos, Quaternion.identity);
                exp.GetComponent<NetworkObject>().Spawn();
            }
            Destroy(gameObject);
        }
        
    }
    public ulong GetKillerClientİd()
    {
        return OwnerClientId;
    }

    public int GetRespawnTimer()
    {
        return mysteryBoxSkillsSO.SkillData.RespawnTimer;
    }

    public int GetDamageAmount()
    {
        return mysteryBoxSkillsSO.SkillData.DamageAmount;
    }

    public string GetKillerName()
    {
        ulong killerClientId = GetKillerClientİd();

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientId, out var killerClient))
        {
            string playerName = killerClient.PlayerObject.GetComponent<PlayerNetworkController>().PlayerName.Value.ToString();
            return playerName;
        }

        return string.Empty;
    }
}
