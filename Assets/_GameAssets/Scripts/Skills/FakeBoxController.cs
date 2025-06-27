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
    [SerializeField] private GameObject _explosionParticlesPrefab;

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
        DestroyRpc(false);
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
            DestroyRpc(true);
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

    public void Damage(PlayerVehicleController playerVehicleController, string playerName)
    {
        playerVehicleController.CrashVehicle();
        KillScreenUI.Instance.SetSmashedUI(playerName, mysteryBoxSkillsSO.SkillData.RespawnTimer);
        DestroyRpc(true);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyRpc(bool isExp)
    {
        if (IsServer)
        {
            if (isExp)
            {
                GameObject exp = Instantiate(_explosionParticlesPrefab, transform.position, Quaternion.identity);
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
