using UnityEngine;

public class ListMatches : MonoBehaviour
{

	void OnEnable()
	{
		MyLobbyManager.Instance.ListMatches();
	}
}
