using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class SkillManager : NetworkBehaviour
{
    public event Action OnMineCountReduced;
    public static SkillManager Instance;
    [SerializeField] private MysteryBoxSkillsSO[] _mysteryBoxSkills;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _hillLayer;
    private Dictionary<SkillType, MysteryBoxSkillsSO> _skillsDic;

    private void Awake()
    {
        Instance = this;
        _skillsDic = new();

        foreach (var skill in _mysteryBoxSkills)
        {
            _skillsDic[skill.SkillType] = skill;
        }
    }

    public void ActivateSkill(SkillType skillType, Transform playerTrasnform, ulong spawnerClientId)
    {
        SkillTransformDataSerilazable skillTransform
        = new SkillTransformDataSerilazable(playerTrasnform.position,
        playerTrasnform.rotation, skillType, playerTrasnform.GetComponent<NetworkObject>());

        if (!IsServer)
        {
            RequestSpawnRpc(skillTransform, spawnerClientId);
            return;
        }

        SpawnSkill(skillTransform, spawnerClientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RequestSpawnRpc(SkillTransformDataSerilazable skillTransformDataSerilazable, ulong spawnerClientId)
    {
        SpawnSkill(skillTransformDataSerilazable, spawnerClientId);
    }
    private async void SpawnSkill(SkillTransformDataSerilazable skillTransformDataSerilazable, ulong spawnerClientId)
    {
        if (!_skillsDic.TryGetValue(skillTransformDataSerilazable.SkillType, out MysteryBoxSkillsSO skillData))
        {
            Debug.Log($"Spawn Kill: {skillTransformDataSerilazable.SkillType} not found!");
            return;
        }

        if (skillTransformDataSerilazable.SkillType == SkillType.Mine)
        {
            Vector3 spawnPos = skillTransformDataSerilazable.Position;
            Vector3 spawnDir = skillTransformDataSerilazable.Rotation * Vector3.forward;

            for (int i = 0; i < skillData.SkillData.SpawnAmountOrTimer; i++)
            {
                Vector3 offset = spawnDir * (i * 3f);
                skillTransformDataSerilazable.Position = spawnPos + offset;

                Spawn(skillTransformDataSerilazable, spawnerClientId, skillData);
                await UniTask.Delay(200);
                OnMineCountReduced?.Invoke();
            }   
        }
        else
        {
            Spawn(skillTransformDataSerilazable, spawnerClientId, skillData);
        }
    }

    private void Spawn(SkillTransformDataSerilazable skillTransformDataSerilazable, ulong spawnerClientId, MysteryBoxSkillsSO skillData)
    {
        if (IsServer)
        {
            Transform skill = Instantiate(skillData.SkillData.SkillPrefab);
            skill.SetPositionAndRotation(skillTransformDataSerilazable.Position, skillTransformDataSerilazable.Rotation);
            var netObj = skill.GetComponent<NetworkObject>();
            netObj.SpawnWithOwnership(spawnerClientId);

            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(spawnerClientId, out var client))
            {
                if (skillData.SkillType != SkillType.Rocket)
                {
                    netObj.TrySetParent(client.PlayerObject);
                }
                else
                {
                    PlayerSkillController playerSkillController = client.PlayerObject.GetComponent<PlayerSkillController>();
                    netObj.transform.localPosition = playerSkillController.GetRocketLaunchPos();
                    return;
                }

                if (skillData.SkillData.ShouldToParent)
                {
                    netObj.transform.localPosition = Vector3.zero;
                }

                PositionDataSerilezabe positionData = new(skill.transform.localPosition + skillData.SkillData.SkillOffset);
                UpdateSkillPositionRpc(netObj.NetworkObjectId, positionData, false);

                if (!skillData.SkillData.ShouldToParent)
                {
                    netObj.TryRemoveParent();
                }

                if (skillData.SkillType == SkillType.FakeBox)
                {
                    float groundHeigth = GetGroundHeight(skillData, skill.position);

                    positionData = new(new Vector3(skill.transform.position.x, groundHeigth, skill.transform.position.z));
                    UpdateSkillPositionRpc(netObj.NetworkObjectId, positionData, true);   
                }
            }
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateSkillPositionRpc(ulong objectId, PositionDataSerilezabe positionDataSerilezabe, bool specialPos)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var netObj))
        {
            if (specialPos) netObj.transform.position = positionDataSerilezabe.Position;
            else netObj.transform.localPosition = positionDataSerilezabe.Position;
            
        }
    }

    private float GetGroundHeight(MysteryBoxSkillsSO skill, Vector3 Pos)
    {
        if (Physics.Raycast(new Vector3(Pos.x, Pos.y, Pos.z), Vector3.down, out RaycastHit hit2, 10f, _hillLayer))
        {
            return 3f;
        }
        if (Physics.Raycast(new Vector3(Pos.x, Pos.y, Pos.z), Vector3.down, out RaycastHit hit, 10f, _groundLayer))
        {
            return skill.SkillData.SkillOffset.y;
        }

        return skill.SkillData.SkillOffset.y;
    }
}
