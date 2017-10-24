using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke40 : MonoBehaviour {

    ParticleSystem smoke40;

    void Awake()
    {
        smoke40 = GetComponentInChildren<ParticleSystem>();
    }

    public void Smoke40On()
    {
        if(!smoke40.isPlaying)
        {
            smoke40.Play();
        }
    }

    public void Smoke40Off()
    {
        if(smoke40.isPlaying)
        {
            smoke40.Stop();
        }
    }
}
