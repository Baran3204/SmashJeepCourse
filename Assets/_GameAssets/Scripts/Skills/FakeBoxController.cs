using System;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class FakeBoxController : NetworkBehaviour, IDamagables
{
    [Header("References")]
    [SerializeField] private Canvas _fakeBoxCanvas;
    [SerializeField] private Collider _fakeBoxCollider;
    [SerializeField] private RectTransform _arrowTransform;
    [SerializeField] private float _animationDuration;
    [SerializeField] private MysteryBoxSkillsSO mysteryBoxSkillsSO;

    private Tween _arrowTween;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        SetOwnerVisualRpc();

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

    [Rpc(SendTo.Owner)]
    private void SetOwnerVisualRpc()
    {
        _fakeBoxCanvas.gameObject.SetActive(true);
        _fakeBoxCanvas.worldCamera = Camera.main;
        _fakeBoxCollider.enabled = false;
        _arrowTween = _arrowTransform.DOAnchorPosY(-1, _animationDuration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ShieldControlle s))
        {
            DestroyRpc();
        }
    }
    public override void OnNetworkDespawn()
    {
        _arrowTween.Kill();

        if (!IsOwner) return;
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var client))
        {
            NetworkObject ownerNetObj = client.PlayerObject;
            PlayerVehicleController playerVehicleController = ownerNetObj.GetComponent<PlayerVehicleController>();
            playerVehicleController.OnVehicleCrashed -= OnVehicleCrashed;
        }
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
