using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int _colorId;
    [SerializeField] private Image _colorImage;
    [SerializeField] private GameObject _selectedGameObject;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(() =>
        {
            MultiplayerGameManager.Instance.ChangePlayerColor(_colorId);
        });
    }
    private void Start()
    {

        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged += OnPlayerDataNetworkListChanged;
        _colorImage.color = MultiplayerGameManager.Instance.GetPlayerColor(_colorId);
        UpdateIsSelected();
    }

    private void OnPlayerDataNetworkListChanged()
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (MultiplayerGameManager.Instance.GetPlayerData().ColorID == _colorId)
        {
            _selectedGameObject.SetActive(true);
        }
        else
        {
            _selectedGameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged += OnPlayerDataNetworkListChanged;
    }
}
