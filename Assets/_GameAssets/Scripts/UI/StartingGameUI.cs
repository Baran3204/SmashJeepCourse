using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class StartingGameUI : NetworkBehaviour
{
    public static StartingGameUI Instance;
    public event Action OnAllPlayersConnected;
    [SerializeField] private TMP_Text _countDownText;
    [SerializeField] private float _animDur;

    private NetworkVariable<int> _playersLoaded = new
    (
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            SetPlayersLoadedRpc();
        }
        if (IsServer)
        {
            OnSinglePlayerConnected();
            _playersLoaded.OnValueChanged = OnPlayersLoad;
        }
    }
    private void Awake()
    {
        Instance = this;    
    }
    private void OnPlayersLoad(int oldCount, int newCount)
    {
        if (IsServer && newCount == NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            StartCountDownRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void SetPlayersLoadedRpc()
    {
        _playersLoaded.Value++;
        Debug.Log("Client scene loaded. Total loaded: " + _playersLoaded.Value);

    }
    [Rpc(SendTo.ClientsAndHost)]
    private void StartCountDownRpc()
    {
        OnAllPlayersConnected?.Invoke();
        StartCoroutine(CountDownCoroutine());
    }
    private void OnSinglePlayerConnected()
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 1)
        {
            Debug.Log("Single Player Connected");
            StartCoroutine(CountDownCoroutine());
            WaitingForPlayersUI.Instance.Hide();
        }
    }
    private IEnumerator CountDownCoroutine()
    {
        _countDownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            _countDownText.text = i.ToString();
            AnimateText();
            yield return new WaitForSeconds(1f);
        }

        GameManager.Instance.ChangeGameState(GameState.Playing);
        _countDownText.text = "GO!";
        AnimateText();
        yield return new WaitForSeconds(1f);

        _countDownText.transform.DOScale(0f, _animDur / 2).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            _countDownText.gameObject.SetActive(false);
        });
    }
    private void AnimateText()
    {
        _countDownText.transform.localScale = Vector3.zero;
        _countDownText.transform.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-30f, 30f));

        _countDownText.transform.DOScale(1f, _animDur).SetEase(Ease.OutBack);
        _countDownText.transform.DORotate(Vector3.zero, _animDur).SetEase(Ease.OutBack);
    }
}
