using UnityEngine;

public class WaitingForPlayersUI : MonoBehaviour
{
    public static WaitingForPlayersUI Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        gameObject.SetActive(true);
        StartingGameUI.Instance.OnAllPlayersConnected += () =>
        {
            Hide();
        };
    }

    public void Hide()
    {
        gameObject.SetActive(false); 
    }

}
