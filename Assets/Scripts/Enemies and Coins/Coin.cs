using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioClip coinSfx;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(coinSfx, transform.position, 1f);
            GameManager.Instance.CollectCoin();
            Destroy(gameObject);
        }
    }

}
