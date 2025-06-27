using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct LeaderBoardEntitySerilezabe : INetworkSerializeByMemcpy, IEquatable<LeaderBoardEntitySerilezabe>
{

    public ulong ClientID;
    public FixedString32Bytes PlayerName;
    public int Score;

    public LeaderBoardEntitySerilezabe(ulong ClientId, FixedString32Bytes playerName, int score)
    {
        ClientID = ClientId;
        PlayerName = playerName;
        Score = score;
    }
    public bool Equals(LeaderBoardEntitySerilezabe other)
    {
        return ClientID == other.ClientID
             && PlayerName.Equals(other.PlayerName)
             && Score == other.Score;
    }
}
