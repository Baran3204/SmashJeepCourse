using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardRanking : MonoBehaviour
{
    [SerializeField] private TMP_Text _rankText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Color _ownerColor;

    private FixedString32Bytes _playerName;
    public ulong ClientID { get; private set; }
    public int Score { get; private set; }

    public void SetData(ulong clientId, FixedString32Bytes name, int score)
    {
        ClientID = clientId;
        _playerName = name;

        _nameText.text = name.ToString();

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            _nameText.color = _ownerColor;
            _scoreText.color = _ownerColor;
        }
        UpdateOrder();
        UpdateScore(score);
    }

    public void UpdateScore(int score)
    {
        Score = score;
        _scoreText.text = Score.ToString();
    }

    public void UpdateOrder()
    {
        _rankText.text = $"{transform.GetSiblingIndex() + 1}";
    }

    public string GetPlayerName()
    {
        return _playerName.ToString();
    }
  
}
