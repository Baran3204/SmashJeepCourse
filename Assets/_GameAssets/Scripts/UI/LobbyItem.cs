using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _lobbyNameText;
    [SerializeField] private TMP_Text _lobbyPlayersText;
    [SerializeField] private Button _joinButton;

    private LobbiesParentUI _lobbiesParentUI;
    private Lobby _lobby;

    private void Awake()
    {
        _joinButton.onClick.AddListener(() =>
        {
            _lobbiesParentUI.JoinAsync(_lobby);
        });  
    }
    public void SetupLobbyItem(LobbiesParentUI lobbiesParentUI, Lobby lobby)
    {
        _lobbiesParentUI = lobbiesParentUI;
        _lobby = lobby;

        _lobbyNameText.text = lobby.Name;
        _lobbyPlayersText.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";
    }
}
