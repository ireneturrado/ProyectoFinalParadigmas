using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HitFlashUI : MonoBehaviour
{
    public static HitFlashUI Instance;

    public Image flashImage;
    public float flashDuration = 0.15f;

    private void Awake()
    {
        Instance = this;
        flashImage.color = new Color(1, 0, 0, 0);
    }

    public void PlayFlash()
    {
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        // Aparece
        flashImage.color = new Color(1, 0, 0, 0.6f);
        yield return new WaitForSecondsRealtime(flashDuration);

        // Desaparece
        flashImage.color = new Color(1, 0, 0, 0);
    }
}
