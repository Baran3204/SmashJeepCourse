using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelectorUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _enterNameText;
    [SerializeField] private Button _connectButton;

    private void Awake()
    {
        _connectButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString("PlayerName", _enterNameText.text);
            SceneManager.LoadScene("LoadingScene");
        });
    }

    private void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene("LoadingScene");
            return;
        }

        _enterNameText.text = PlayerPrefs.GetString("PlayerName", string.Empty);    
    }
}
