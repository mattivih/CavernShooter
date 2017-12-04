using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineWarning : MonoBehaviour
{
    public GameObject source = null;
    public AudioClip Warning;

    private int _shipsInside = 0;
    private AudioSource _audioSource;

    public void Start()
    {
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
                if (_audioSource.isPlaying)
                {
                    _audioSource.Stop();
                }
            }
        }
    }

    public void MineLight()
    {
        Color color = new Color();
        var LightColor = GetComponentInChildren<Light>().color;
        var shipmats = GetComponentInParent<UseMine>().source.GetComponent<Ship>().GetComponentInChildren<MeshRenderer>().materials;
        foreach (var mat in shipmats)
        {
            if (mat.name == "_Ship_Colour (Instance)" || mat.name == "_Ship_Colour")
                color = mat.color;
        }
        LightColor = color;
    }
}
