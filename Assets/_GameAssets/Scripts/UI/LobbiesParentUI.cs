using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbiesParentUI : MonoBehaviour
{
    [SerializeField] private Transform _lobbyItemParent;
    [SerializeField] private LobbyItem _lobbyItemPrefab;
    [SerializeField] private Button _refreshButton;
    private bool _isJoining;
    private bool _isRefreshing;
    public async void JoinAsync(Lobby lobby)
    {
        if (_isJoining) return;
        _isJoining = true;
        try
        {
            Lobby joiningLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GetClientGameManager().StartClientAsycn(joinCode);
        }
        catch (LobbyServiceException exc)
        {
            Debug.LogError(exc);
            RefreshList();
        }

        _isJoining = false;
    }
    
    private void Awake()
    {
        _refreshButton.onClick.AddListener(RefreshList);    
    }

    public async void RefreshList()
    {
        if (_isRefreshing) return;
        _isRefreshing = true;

        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new();
            queryLobbiesOptions.Count = 20;
            queryLobbiesOptions.Filters = new List<QueryFilter>()
            {
                new QueryFilter
                (
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"
                ),
                 new QueryFilter
                (
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"
                )
            };
            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            foreach (Transform e in _lobbyItemParent)
            {
                Destroy(e.gameObject);
            }

            foreach (Lobby e in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(_lobbyItemPrefab, _lobbyItemParent);
                lobbyItem.SetupLobbyItem(this, e);
            }
        }
        catch (LobbyServiceException exc)
        {
            Debug.LogError(exc);

        }
        _isRefreshing = false;
    }
}
