using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioAutoDestroy : MonoBehaviour
{
    private AudioSource a;

    void Awake()
    {
        a = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (a.clip == null)
        {
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, a.clip.length + 0.1f);
    }
}
