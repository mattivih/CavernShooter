using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchResultList : MonoBehaviour {

    private List<Transform> _positions;
    private int _playerCount = 0;
    private Text[] _entries;
    void Awake () {
        _entries = GetComponentsInChildren<Text>();
    }

    public void FillPlayerInfo(string playerName) {
        _playerCount++;
        _entries[_playerCount-1].text = _playerCount + ". " + playerName;
    }
}
