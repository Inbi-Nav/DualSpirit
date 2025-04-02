using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;
    public AudioClip backgroundMusic;
    [SerializeField] private Slider musicSlider;

    public static MusicManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MusicManager>();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Asegura que el audio source existe
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (backgroundMusic != null)
            PlayBackgroundMusic(false, backgroundMusic);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value); });
    }

    public static void SetVolume(float volume)
    {
        if (Instance.audioSource != null)
            Instance.audioSource.volume = volume;
    }

    public void PlayBackgroundMusic(bool resetSong, AudioClip audioClip = null)
    {
        if (audioClip != null)
            audioSource.clip = audioClip;

        if (audioSource.clip != null)
        {
            if (resetSong)
                audioSource.Stop();

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }

    public void PauseBackgroundMusic()
    {
        if (audioSource != null)
            audioSource.Pause();
    }
}
