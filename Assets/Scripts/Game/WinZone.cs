using UnityEngine;

public class WinZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance == null) return;

        bool collected = GameManager.Instance.AllCoinsCollected();
        if (GameManager.Instance.State == GameState.Playing && collected)
        {
            GameManager.Instance.Win();
        }
    }

}

