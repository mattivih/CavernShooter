using UnityEngine;

public class ButtonSoundPlayer : MonoBehaviour {

    public static ButtonSoundPlayer Instance;
    public AudioClip PressNext, PressPrev, PressStart;
    private AudioSource _nextAudio, _prevAudio, _startAudio;
   
    public void Start()
    {
        if (Instance == null) {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        _nextAudio = gameObject.AddComponent<AudioSource>();
        _nextAudio.clip = PressNext;
        _prevAudio = gameObject.AddComponent<AudioSource>();
        _prevAudio.clip = PressPrev;
        _startAudio = gameObject.AddComponent<AudioSource>();
        _startAudio.clip = PressStart;
    }

    public void PlayNextSound()
    {
        if (_nextAudio)
        {
            _nextAudio.Play();
        }
    }

    public void PlayPrevSound()
    {
        if (_prevAudio)
        {
            _prevAudio.Play();
        }
    }

    public void PlayStartSound()
    {
        if (_startAudio)
        {
            _startAudio.Play();
        }
    }
}
