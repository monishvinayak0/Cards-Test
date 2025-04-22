using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource fxSource;
    public AudioClip flip, match, mismatch, gameOver;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayFlip() => fxSource.PlayOneShot(flip);
    public void PlayMatch() => fxSource.PlayOneShot(match);
    public void PlayMismatch() => fxSource.PlayOneShot(mismatch);
    public void PlayGameOver() => fxSource.PlayOneShot(gameOver);
}
