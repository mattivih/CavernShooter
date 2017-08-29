using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

/// <summary>
/// UI script: handles viewing the information of one match in the match list.
/// </summary>
public class MyMatchListEntry : MonoBehaviour
{

	public Text MatchName, Players;
	public Button JoinButton;

	private MatchInfoSnapshot _match;

	/// <summary>
	/// Add a match's information to the UI's match list.
	/// </summary>
	public void FillMatchListEntry(MatchInfoSnapshot match)
	{
		_match = match;
		MatchName.text = match.name;
		Players.text = match.currentSize + "/" + match.maxSize;
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
		MyLobbyManager.Instance.JoinMatch(_match);
	}
}
