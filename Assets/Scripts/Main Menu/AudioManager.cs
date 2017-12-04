using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Random = System.Random;

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
        if (_audioSource.clip != MenuMusic) {
            _audioSource.clip = MenuMusic;
            _audioSource.Play();
        }
	}

	public void OnEnterCreditsMenu()
	{
        //Not used currently
	}

	public void OnLevelLoaded(Scene scene, LoadSceneMode mode)
	{
	    if (scene.name != "0_Title_Screen" && scene.name != "1_Main_Menu")
	    {
            Random random = new Random();
	        int song = random.Next(0, LevelSongs.Length);
            _audioSource.clip = LevelSongs[song];
            _audioSource.Play();
        }
	}
}