using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Objects/Skill Data")]
public class SkillDataSO : ScriptableObject
{
    [SerializeField] private Transform _skillPrefab;
    [SerializeField] private Vector3 _skillOffset;
    [SerializeField] private int _spawnAmountOrTimer;
    [SerializeField] private bool _shouldToParent;
    [SerializeField] private int _respawnTimer;
    [SerializeField] private int _damageAmount;

    public Transform SkillPrefab => _skillPrefab;
    public Vector3 SkillOffset => _skillOffset;
    public int SpawnAmountOrTimer => _spawnAmountOrTimer;
    public int RespawnTimer => _respawnTimer;
    public int DamageAmount => _damageAmount;
    public bool ShouldToParent => _shouldToParent;
}
