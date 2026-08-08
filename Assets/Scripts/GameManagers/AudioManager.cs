using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource timerSource;

    [Header("SFX")]
    [SerializeField] private AudioClip diamondPickup;
    [SerializeField] private AudioClip lavaDamage;
    [SerializeField] private AudioClip wrongClick;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip winMusic;
    [SerializeField] private AudioClip loseMusic;

    [Header("Timer")]
    [SerializeField] private AudioClip timerRunningOut;

    private bool timerWarningPlaying;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();

        timerSource.clip = timerRunningOut;
        timerSource.loop = true;
    }

    public void PlayDiamondPickup()
    {
        sfxSource.PlayOneShot(diamondPickup);
    }

    public void PlayWrongClick()
    {
        sfxSource.PlayOneShot(wrongClick);
    }

    public void PlayLavaDamage()
    {
        sfxSource.PlayOneShot(lavaDamage);
    }

    public void StartTimerWarning()
    {
        if (timerWarningPlaying)
            return;

        timerWarningPlaying = true;
        timerSource.Play();
    }

    public void StopTimerWarning()
    {
        timerWarningPlaying = false;
        timerSource.Stop();
    }

    public void PlayWin()
    {
        StopTimerWarning();

        musicSource.Stop();
        musicSource.PlayOneShot(winMusic);
    }

    public void PlayLose()
    {
        StopTimerWarning();

        musicSource.Stop();
        musicSource.PlayOneShot(loseMusic);
    }
}