using Unity.Netcode;
using UnityEngine;

public struct SkillTransformDataSerilazable : INetworkSerializeByMemcpy
{
    public Vector3 Position;
    public Quaternion Rotation;
    public SkillType SkillType;
    public NetworkObject NetworkObject;

    public SkillTransformDataSerilazable(Vector3 pos, Quaternion rotate, SkillType skill, NetworkObject netObj)
    {
        Position = pos;
        Rotation = rotate;
        SkillType = skill;
        NetworkObject = netObj;
    }
}
