using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class LeaderBoardUI : NetworkBehaviour
{
    [SerializeField] private LeaderBoardRanking _leaderBoardRankng;
    [SerializeField] private Transform _rankingParent;
    [SerializeField] private TMP_Text _rankText;
    private NetworkList<LeaderBoardEntitySerilezabe> _leaderBoardList = new();
    private List<LeaderBoardRanking> _leaderBoardRankingList = new();
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            Debug.Log("Evente baglandı");
            _leaderBoardList.OnListChanged += HandleLeaderBoardChangedRpc;

            foreach (LeaderBoardEntitySerilezabe entity in _leaderBoardList)
            {
                HandleLeaderBoardChangedRpc(new NetworkListEvent<LeaderBoardEntitySerilezabe>
                {
                    Type = NetworkListEvent<LeaderBoardEntitySerilezabe>.EventType.Add,
                    Value = entity
                });
            }
        }
        if (IsServer)
        {
            PlayerNetworkController[] players = FindObjectsByType<PlayerNetworkController>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                HandlePlayerSpawned(player);
            }


            PlayerNetworkController.OnPlayerSpawned += HandlePlayerSpawned;
            PlayerNetworkController.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }
    private void HandleLeaderBoardChangedRpc(NetworkListEvent<LeaderBoardEntitySerilezabe> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderBoardEntitySerilezabe>.EventType.Add:
                if (!_leaderBoardRankingList.Any(x => x.ClientID == changeEvent.Value.ClientID))
                {
                    var ranking = Instantiate(_leaderBoardRankng, _rankingParent);
                    ranking.SetData(changeEvent.Value.ClientID, changeEvent.Value.PlayerName, changeEvent.Value.Score);
                    _leaderBoardRankingList.Add(ranking);
                }
                UpdatePlayerRankText();
                break;
            case NetworkListEvent<LeaderBoardEntitySerilezabe>.EventType.Value:
                LeaderBoardRanking leaderBoardRankingUpdate =
                _leaderBoardRankingList.FirstOrDefault(x => x.ClientID == changeEvent.Value.ClientID);

                if (leaderBoardRankingUpdate != null)
                {
                    leaderBoardRankingUpdate.UpdateScore(changeEvent.Value.Score);
                }
                break;
            case NetworkListEvent<LeaderBoardEntitySerilezabe>.EventType.Remove:
                LeaderBoardRanking leaderBoardRanking = _leaderBoardRankingList.FirstOrDefault(x => x.ClientID == changeEvent.Value.ClientID);
                if (leaderBoardRanking != null)
                {
                    leaderBoardRanking.transform.SetParent(null);
                    _leaderBoardRankingList.Remove(leaderBoardRanking);
                    Destroy(leaderBoardRanking.gameObject);
                    UpdatePlayerRankText();
                }
                break;
        }

        UpdateSortingOrder();
    }
    private void UpdatePlayerRankText()
    {
        LeaderBoardRanking myRank = _leaderBoardRankingList.FirstOrDefault(
            x => x.ClientID == NetworkManager.Singleton.LocalClientId
        );
        if (myRank == null) return;

        int rank = myRank.transform.GetSiblingIndex() + 1;
        string rankSuffix = GetRankSuffix(rank);

        _rankText.text = $"{rank} <sup>{rankSuffix}</sup>/{_leaderBoardRankingList.Count}";
    }
    private string GetRankSuffix(int rank)
    {
        return rank switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };
    }
    private void HandlePlayerSpawned(PlayerNetworkController controller)
    {
        _leaderBoardList.Add(new LeaderBoardEntitySerilezabe
        {
            ClientID = controller.OwnerClientId,
            PlayerName = controller.PlayerName.Value,
            Score = 0
        });

        controller.GetPlayerScoreController().PlayerScore.OnValueChanged += (oldScore, newScore) =>
        {
            HandleScoreChanged(controller.OwnerClientId, newScore);
        };
    }
    private void HandleScoreChanged(ulong ClientID, int newScore)
    {
        for (int i = 0; i < _leaderBoardList.Count; i++)
        {
            if (_leaderBoardList[i].ClientID != ClientID) continue;

            _leaderBoardList[i] = new LeaderBoardEntitySerilezabe
            {
                ClientID = _leaderBoardList[i].ClientID,
                PlayerName = _leaderBoardList[i].PlayerName,
                Score = newScore
            };
            UpdatePlayerRankText();
            return;
        }


    }

    public List<LeaderBoardEntitySerilezabe> GetLeaderBoard()
    {
        List<LeaderBoardEntitySerilezabe> leaderBoardData = new();

        foreach (var e in _leaderBoardList)
        {
            leaderBoardData.Add(e);
        }

        return leaderBoardData;
    }

    public string GetWinnersName()
    {
        if (_leaderBoardRankingList.Count > 0)
        {
            return _leaderBoardRankingList[0].GetPlayerName();
        }

        return "No Winner";
    }
    private void HandlePlayerDespawned(PlayerNetworkController controller)
    {
        if (_leaderBoardList == null) return;

        foreach (var entity in _leaderBoardList)
        {
            if (entity.ClientID != controller.OwnerClientId) continue;

            _leaderBoardList.Remove(entity);
            break;
        }

        controller.GetPlayerScoreController().PlayerScore.OnValueChanged -= (oldScore, newScore) =>
        {
            HandleScoreChanged(controller.OwnerClientId, newScore);
        };
    }

    private void UpdateSortingOrder()
    {
        _leaderBoardRankingList.Sort((x, y) => y.Score.CompareTo(x.Score));

        for (int i = 0; i < _leaderBoardRankingList.Count; i++)
        {
            _leaderBoardRankingList[i].transform.SetSiblingIndex(i);
            _leaderBoardRankingList[i].UpdateOrder();
        }
        UpdatePlayerRankText();
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _leaderBoardList.OnListChanged -= HandleLeaderBoardChangedRpc;
        }
        if (IsServer)
        {
            PlayerNetworkController.OnPlayerSpawned -= HandlePlayerSpawned;
            PlayerNetworkController.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }
}
