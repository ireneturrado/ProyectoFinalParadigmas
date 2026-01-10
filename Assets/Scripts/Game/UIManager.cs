using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI messageText;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (messageText) messageText.gameObject.SetActive(false);
    }

    public void SetCoins(int collected, int total)
    {
        if (coinsText) coinsText.text = $"Coins: {collected}/{total}";
    }

    public void ShowMessage(string msg, float seconds = 2f)
    {
        if (!messageText) return;
        messageText.gameObject.SetActive(true);
        messageText.text = msg;
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), seconds);
    }

    void HideMessage()
    {
        if (messageText) messageText.gameObject.SetActive(false);
    }
    public void HideCoins()
    {
        coinsText.gameObject.SetActive(false);
    }

    public void ShowCoins()
    {
        coinsText.gameObject.SetActive(true);
    }

}
