using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class HelpMenuPUManager : MonoBehaviour {

	public GameObject _namePos, _pickupPos, _iconPos, _bottom;
	public GameObject[] Names, Pickups, Icons;
	private GameObject[] _temp = new GameObject[8];

	void Start () {
		InstantiatePUs(Names, _namePos, 0f);
		InstantiatePUs(Icons, _iconPos, -0.2f);
		InstantiatePUs(Pickups, _pickupPos, 30f);
	}
	
	private void InstantiatePUs(GameObject[] prefablist, GameObject topSpawnPos, float scale){
		GameObject[] _array = prefablist;
		GameObject _position = topSpawnPos;
		float _scale = scale;

		// Reset prefab array's transforms
		for(int i = 0; i < _array.Length; i++){
			_array[i].transform.rotation = Quaternion.identity;
			_array[i].transform.localScale = new Vector3(1f, 1f, 1f);
		}
		Vector3 distance = new Vector3(0f, 0f, 0f);
		Vector3 spawnPoint = new Vector3(
		_position.GetComponent<Transform>().position.x, 
		_position.GetComponent<Transform>().position.y, 
		_position.GetComponent<Transform>().position.z);
		for(int j = 0; j < _array.Length; j++){
			distance += new Vector3(0f, (_bottom.transform.position.y - _position.transform.position.y) / _array.Length * (j+1), 0f);
			_temp[j] = Instantiate(_array[j], spawnPoint + distance, Quaternion.identity, _position.transform);
            _temp[j].transform.localScale += new Vector3(_scale, _scale, _scale);
            distance = new Vector3(0f, 0f, 0f);
		} 
	}

	void Update(){
		for(int i = 0; i < _temp.Length; i++){
			_temp[i].transform.Rotate(20f * Time.deltaTime, 30f * Time.deltaTime, 20f * Time.deltaTime);
		}	
	}
	
}
