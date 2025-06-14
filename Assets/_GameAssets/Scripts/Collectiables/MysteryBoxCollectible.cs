using Unity.Netcode;
using UnityEngine;

public class MysteryBoxCollectible : NetworkBehaviour, ICollectible
{

    [Header("References")]
    [SerializeField] private MysteryBoxSkillsSO[] _skills;
    [SerializeField] private Animator _mysteryBoxAnimator;
    [SerializeField] private Collider _mysteryBoxCollider;
    [Header("Settings")]
    [SerializeField] private float _respawnTimer;
    public void Collect(PlayerSkillController playerSkillController)
    {
        if (playerSkillController.HasSkillAlready()) return;
        MysteryBoxSkillsSO skill = GetRandomSkill();
        SkillsUI.Instance.SetSkillUI(skill);
        playerSkillController.SetupSkill(skill);
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

    private MysteryBoxSkillsSO GetRandomSkill()
    {
        return _skills[Random.Range(0, _skills.Length)];
    }
}
