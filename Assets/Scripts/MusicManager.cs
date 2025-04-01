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
            {
                instance = FindObjectOfType<MusicManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

   void Start()
{
    if (backgroundMusic != null)
    {
        PlayBackgroundMusic(false, backgroundMusic);
    }

    musicSlider.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value); });
}


    public static void SetVolume(float volume)
    {
        Instance.audioSource.volume = volume;
    }

  public void PlayBackgroundMusic(bool resetSong, AudioClip audioClip = null)
{
    if (audioClip != null)
    {
        audioSource.clip = audioClip;
    }

    if (audioSource.clip != null)
    {
        if (resetSong)
        {
            audioSource.Stop();
        }
        audioSource.Play();
    }

}


    public void PauseBackgroundMusic()
    {
        audioSource.Pause();
    }
}
