using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparks40 : MonoBehaviour {

    ParticleSystem sparks40;

    void Awake()
    {
        sparks40 = GetComponentInChildren<ParticleSystem>();
    }

    public void Sparks40On()
    {
        if (!sparks40.isPlaying)
        {
            sparks40.Play();
        }
    }

    public void Sparks40Off()
    {
        if (sparks40.isPlaying)
        {
            sparks40.Stop();
        }
    }
}
