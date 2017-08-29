using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLobby : MonoBehaviour {

    public void ReturnToLobby() {
        //MyLobbyManager.Instance.SendReturnToLobby();
        SceneManager.LoadScene("1_MainMenu");
    }
}
