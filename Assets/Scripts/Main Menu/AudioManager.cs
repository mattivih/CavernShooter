using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

	public static AudioManager Instance = null;

	public AudioClip TitleScreen, MenuMusic;
	public AudioClip[] LevelSongs;

	private AudioSource _audioSource;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		_audioSource = GetComponent<AudioSource>();
		_audioSource.loop = true;
		_audioSource.clip = TitleScreen;
        _audioSource.Play();
        SceneManager.sceneLoaded += OnLevelLoaded;
	}

	public void OnEnterMainMenu()
	{
        //TODO: Don't change clip if it's already playing.
		_audioSource.clip = MenuMusic;
		_audioSource.Play();
	}

	public void OnEnterCreditsMenu()
	{
        //Not used currently
	}

	public void OnLevelLoaded(Scene scene, LoadSceneMode mode)
	{
		_audioSource.clip = LevelSongs[scene.buildIndex];
		_audioSource.Play();
	}
}