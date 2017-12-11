using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Random = System.Random;

public class AudioManager : MonoBehaviour
{

	public static AudioManager Instance = null;

	public AudioClip TitleScreen, MenuMusic;
	public AudioClip[] LevelSongs;

	private AudioSource _levelAudio, _nextAudio, _prevAudio, _startAudio;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		_levelAudio = GetComponent<AudioSource>();
		_levelAudio.loop = true;
        _levelAudio.clip = TitleScreen;
        _levelAudio.Play();
        SceneManager.sceneLoaded += OnLevelLoaded;
	}

	public void OnEnterCreditsMenu()
	{
        //Not used currently
	}

	public void OnLevelLoaded(Scene scene, LoadSceneMode mode)
	{
        if (scene.name != "0_Title_Screen" && scene.name != "1_Main_Menu")
        {
            //Level scene loaded
            Random random = new Random();
            int song = random.Next(0, LevelSongs.Length);
            _levelAudio.clip = LevelSongs[song];
            _levelAudio.Play();
        }
        else if (scene.name == "1_Main_Menu")
        {
            if (_levelAudio.clip != MenuMusic)
            {
                _levelAudio.clip = MenuMusic;
                _levelAudio.Play();
            }
        }
	}
}