using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke20 : MonoBehaviour {

    ParticleSystem smoke20;

    void Awake()
    {
        smoke20 = GetComponentInChildren<ParticleSystem>();
    }

    public void Smoke20On()
    {
        if (!smoke20.isPlaying)
        {
            smoke20.Play();
        }
    }

    public void Smoke20Off()
    {
        if (smoke20.isPlaying)
        {
            smoke20.Stop();
        }
    }
}
