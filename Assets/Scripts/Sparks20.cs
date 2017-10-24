using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparks20 : MonoBehaviour {

    ParticleSystem sparks20;

    void Awake()
    {
        sparks20 = GetComponentInChildren<ParticleSystem>();
    }

    public void Sparks20On()
    {
        if (!sparks20.isPlaying)
        {
            sparks20.Play();
        }
    }

    public void Sparks20Off()
    {
        if (sparks20.isPlaying)
        {
            sparks20.Stop();
        }
    }
}
