using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : NetworkBehaviour
{
    [SerializeField] private int _playerIndex;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private GameObject _readyGameObject;
    [SerializeField] private Button _kickbutton;
    [SerializeField] private CharacterSelectVisual _visual;
    public NetworkVariable<FixedString32Bytes> PlayerName = new();

    private void Awake()
    {
        _kickbutton.onClick.AddListener(() =>
        {
            PlayerDataSerilezabe playerData = MultiplayerGameManager.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);

            MultiplayerGameManager.Instance.KickPlayer(playerData.ClientID);
        });
    }
    private void Start()
    {
        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerGameManager_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        UpdatePlayer();
        HandlePlayerNameChanged(string.Empty, PlayerName.Value);
        PlayerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        _playerNameText.text = newValue.ToString();
    }

    private void CharacterSelectReady_OnReadyChanged()
    {
        UpdatePlayer();
    }

    private void MultiplayerGameManager_OnPlayerDataNetworkListChanged()
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (MultiplayerGameManager.Instance.IsPlayerIndexConnected(_playerIndex))
        {
            Show();

            PlayerDataSerilezabe playerData = MultiplayerGameManager.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);

            _visual.SetPlayerColor(MultiplayerGameManager.Instance.GetPlayerColor(playerData.ColorID));
            _readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.ClientID));

            HideKickButton(playerData);
            SetOwner(playerData.ClientID);
            UpdatePlayerNameRpc();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void HideKickButton(PlayerDataSerilezabe playerData)
    {
        _kickbutton.gameObject.SetActive(NetworkManager.Singleton.IsServer && playerData.ClientID != NetworkManager.Singleton.LocalClientId);
    }

    private void SetOwner(ulong ClientID)
    {
        if (IsServer)
        {
            var netObj = GetComponent<NetworkObject>();

            if (netObj.OwnerClientId != ClientID)
            {
                netObj.ChangeOwnership(ClientID);
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdatePlayerNameRpc()
    {
        if (IsServer)
        {

            UserData userData = HostSingleton.Instance._hostGameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

            PlayerName.Value = userData.UserName;
        }
    }

    public override void OnDestroy()
    {
         MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged -= MultiplayerGameManager_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged -= CharacterSelectReady_OnReadyChanged;
        PlayerName.OnValueChanged -= HandlePlayerNameChanged;
    }
}