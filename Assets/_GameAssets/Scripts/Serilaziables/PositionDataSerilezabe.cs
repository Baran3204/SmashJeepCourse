using Unity.Netcode;
using UnityEngine;

public struct PositionDataSerilezabe : INetworkSerializeByMemcpy
{
    public Vector3 Position;

    public PositionDataSerilezabe(Vector3 pos)
    {
        Position = pos;
    }
}
