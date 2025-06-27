using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _settingButton;
    [Header("Setting Menu")]
    [SerializeField] private RectTransform _menuTransform;
    [SerializeField] private Image _blackBackgroundImage;
    [SerializeField] private Button _vsyncButton;
    [SerializeField] private GameObject _vsyncTick;
    [SerializeField] private Button _leaveGameButton;
    [SerializeField] private Button _keepPlayButton;
    [SerializeField] private Button _copyCodeButton;
    [SerializeField] private Image _copiedImage;
    [SerializeField] private TMP_Text _joinCodeText;

    [Header("Sprites")]
    [SerializeField] private Sprite _tickSprite;
    [SerializeField] private Sprite _crossSprite;
    [Header("Settings")]
    [SerializeField] private float _animDur;
    private bool _isAnim;
    private bool _isVsyncActive;
    private bool _isCopied;

    private void Awake()
    {
        _settingButton.onClick.AddListener(OnSettingsButtonClicked);

        _vsyncButton.onClick.AddListener(() =>
        {
            _isVsyncActive = !_isVsyncActive;

            QualitySettings.vSyncCount = _isVsyncActive ? 1 : 0;
            _vsyncTick.SetActive(_isVsyncActive);
        });

        _leaveGameButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.IsServer) HostSingleton.Instance._hostGameManager.Shutdown();
            ClientSingleton.Instance.GetClientGameManager().Disconnect();
        });

        _keepPlayButton.onClick.AddListener(() =>
        {
            if (_isAnim) return;
            _isAnim = true;
            _blackBackgroundImage.DOFade(0f, _animDur);
            _menuTransform.DOScale(0f, _animDur).SetEase(Ease.InBack).OnComplete(() =>
            {
                _isAnim = false;
                _menuTransform.gameObject.SetActive(false);
                _isCopied = false;
                _copiedImage.sprite = _crossSprite;
            });
        });

        _copyCodeButton.onClick.AddListener(() =>
        {
            if (_isCopied) return;
            _isCopied = true;

            _copiedImage.sprite = _tickSprite;

            GUIUtility.systemCopyBuffer = _joinCodeText.text;
        });
    }

    private void Start()
    {
        _menuTransform.localScale = Vector3.zero;
        _menuTransform.gameObject.SetActive(false);
        _vsyncTick.SetActive(false);
    }

    private void OnSettingsButtonClicked()
    {
        if (_isAnim) return;
        SetJoinCode();
        _isAnim = true;
        _menuTransform.gameObject.SetActive(true);
        _blackBackgroundImage.DOFade(0.8f, _animDur);
        _menuTransform.DOScale(1f, _animDur).SetEase(Ease.OutBack).OnComplete(() =>
        {
            _isAnim = false;
        });
    }

    private void SetJoinCode()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            _joinCodeText.text = HostSingleton.Instance._hostGameManager.GetJoinCode();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            _joinCodeText.text = ClientSingleton.Instance.GetClientGameManager().GetJoinCode();
        }
    }
}
