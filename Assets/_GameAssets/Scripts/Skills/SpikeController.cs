using Unity.Netcode;
using UnityEngine;

public class SpikeController : NetworkBehaviour, IDamagables
{
    [SerializeField] private Collider _spikeCollider;
    [SerializeField] private MysteryBoxSkillsSO mysteryBoxSkillsSO;
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

    public void Damage(PlayerVehicleController playerVehicleController)
    {
        playerVehicleController.CrashVehicle();
        KillScreenUI.Instance.SetSmashedUI("Baran3204", mysteryBoxSkillsSO.SkillData.RespawnTimer);
        DestroyRpc();
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
