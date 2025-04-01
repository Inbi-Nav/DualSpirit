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
        Debug.Log("Background music is set.");
        PlayBackgroundMusic(false, backgroundMusic);
    }
    else
    {
        Debug.Log("No background music assigned.");
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
        Debug.Log("Reproduciendo música de fondo: " + audioSource.clip.name); // Verifica si se asigna correctamente
    }

    if (audioSource.clip != null)
    {
        if (resetSong)
        {
            audioSource.Stop();
        }
        audioSource.Play();
    }
    else
    {
        Debug.LogWarning("No se ha asignado un clip de audio al AudioSource.");
    }
}


    public void PauseBackgroundMusic()
    {
        audioSource.Pause();
    }
}
