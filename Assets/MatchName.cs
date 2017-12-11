using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchName : MonoBehaviour {

    void OnEnable()
    {
        GetComponent<InputField>().interactable = true;
    }
}
