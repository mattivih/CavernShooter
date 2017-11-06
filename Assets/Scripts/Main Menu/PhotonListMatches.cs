using UnityEngine;

public class PhotonListMatches : MonoBehaviour
{

	void OnEnable()
	{
		PhotonLobbyManager.Instance.ListMatches();
	}
}
