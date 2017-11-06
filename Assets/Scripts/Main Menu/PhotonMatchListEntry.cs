using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

/// <summary>
/// UI script: handles viewing the information of one match in the match list.
/// </summary>
public class PhotonMatchListEntry : MonoBehaviour
{

	public Text MatchName, Players;
	public Button JoinButton;

	private RoomInfo _match;

	/// <summary>
	/// Add a match's information to the UI's match list.
	/// </summary>
	public void FillMatchListEntry(RoomInfo match)
	{
		_match = match;
		MatchName.text = _match.Name;
		Players.text = _match.PlayerCount + "/" + _match.MaxPlayers;
		JoinButton.onClick.RemoveAllListeners();
		JoinButton.onClick.AddListener(JoinButtonListener);
	}

	/// <summary>
	/// Button listener for Join Button.
	/// </summary>
	void JoinButtonListener()
	{
		JoinButton.GetComponentInChildren<Text>().text = "JOINED";
		JoinButton.interactable = false;
		JoinButton.onClick.RemoveListener(JoinButtonListener);
		GameObject.Find("Select Match").SetActive(false);
		PhotonLobbyManager.Instance.JoinMatch(_match.Name);
	}
}
