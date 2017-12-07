using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineWarning : MonoBehaviour
{
    public GameObject source = null;
    public AudioClip Warning;
    public Light MineLight;

    private int _shipsInside = 0;
    private AudioSource _audioSource;

    public void Start()
    {
        MineLight.color = new Color(0, 0, 0);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.clip = Warning;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Ship>() && collider.gameObject != source)
        {
            _shipsInside = _shipsInside + 1;
            if (_shipsInside > 0)
            {
                if (GetComponentInParent<SpriteRenderer>().sprite.name == "mine2-blue")
                    MineLight.color = new Color(0, 44, 255);
                else if (GetComponentInParent<SpriteRenderer>().sprite.name == "mine2-purple")
                    MineLight.color = new Color(255, 44, 237);
                else if (GetComponentInParent<SpriteRenderer>().sprite.name == "mine2-red")
                    MineLight.color = new Color(255, 0, 0);
                else if (GetComponentInParent<SpriteRenderer>().sprite.name == "mine2-turquise")
                    MineLight.color = new Color(0, 255, 255);
                    
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Ship>() && collider.gameObject != source)
        {
            _shipsInside = _shipsInside - 1;
            if (_shipsInside == 0)
            {
                MineLight.color = new Color(0, 0, 0);

                if (_audioSource.isPlaying)
                {
                    _audioSource.Stop();
                }
            }
        }
    }
}
