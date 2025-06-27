using Unity.Netcode;
using UnityEngine;

public class SpikeController : NetworkBehaviour, IDamagables
{
    [SerializeField] private Collider _spikeCollider;
    [SerializeField] private MysteryBoxSkillsSO mysteryBoxSkillsSO;
    [SerializeField] private GameObject _expPrefab;
    private void PlayerSkillController_OnTimerFinished()
    {
        DestroyRpc();
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyRpc()
    {
        if (IsServer)
        {
            Destroy(gameObject);
        }
    }
    [Rpc(SendTo.Server)]
    private void PlayerParticlesRpc(Vector3 vehiclePos = default)
    {
        GameObject exp = Instantiate(_expPrefab, vehiclePos, Quaternion.identity);
        exp.GetComponent<NetworkObject>().Spawn();
    }
    public override void OnNetworkSpawn()
    {
        PlayerSkillController.OnTimerFinished += PlayerSkillController_OnTimerFinished;
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
        PlayerSkillController.OnTimerFinished -= PlayerSkillController_OnTimerFinished;
        if (!IsOwner) return;
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var client))
        {
            NetworkObject ownerNetObj = client.PlayerObject;
            PlayerVehicleController playerVehicleController = ownerNetObj.GetComponent<PlayerVehicleController>();
            playerVehicleController.OnVehicleCrashed -= OnVehicleCrashed;
        }
    }

    [Rpc(SendTo.Owner)]
    private void SetOwnerVisualsRpc()
    {
        _spikeCollider.enabled = false;
    }

    public void Damage(PlayerVehicleController playerVehicleController, string playerName)
    {
        playerVehicleController.CrashVehicle();
        PlayerParticlesRpc(playerVehicleController.transform.position);
        KillScreenUI.Instance.SetSmashedUI(playerName, mysteryBoxSkillsSO.SkillData.RespawnTimer);
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
