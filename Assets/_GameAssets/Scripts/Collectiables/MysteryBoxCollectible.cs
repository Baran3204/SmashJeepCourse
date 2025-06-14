using Unity.Netcode;
using UnityEngine;

public class MysteryBoxCollectible : NetworkBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private Animator _mysteryBoxAnimator;
    [SerializeField] private Collider _mysteryBoxCollider;
    [Header("Settings")]
    [SerializeField] private float _respawnTimer;
    public void Collect()
    {
        CollectRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void CollectRpc()
    {
        AnimateCollection();
        Invoke(nameof(Respawn), _respawnTimer);
    }

    private void AnimateCollection()
    {
        _mysteryBoxCollider.enabled = false;
        _mysteryBoxAnimator.SetTrigger("IsCollected");
    }

    private void Respawn()
    {
        _mysteryBoxAnimator.SetTrigger("IsRespawned");
        _mysteryBoxCollider.enabled = true;
    }
}
