using System;
using Unity.Netcode;

public struct PlayerDataSerilezabe : INetworkSerializeByMemcpy, IEquatable<PlayerDataSerilezabe>
{
    public ulong ClientID;
    public int ColorID;

    public PlayerDataSerilezabe(ulong clientid, int colorid)
    {
        ClientID = clientid;
        ColorID = colorid;
    }
    public bool Equals(PlayerDataSerilezabe other)
    {
        return ClientID == other.ClientID && ColorID == other.ColorID;
    }
}
