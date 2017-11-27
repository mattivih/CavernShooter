using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

/// <summary>
/// UI script: handles viewing the information of one match in the match list.
/// </summary>
public class PhotonMatchListEntry : MonoBehaviour
{

	public Text MatchNameText, Players;
	public Button JoinButton;

    public string MatchName { get; private set; }
    public string ViewName { get; private set; }

    private RoomInfo _match;

	/// <summary>
	/// Add a match's information to the UI's match list.
	/// </summary>
	public void FillMatchListEntry(RoomInfo match)
	{
		_match = match;
        MatchName = match.Name;
        if (match.CustomProperties.ContainsKey("MatchName"))
        {
            ViewName = match.CustomProperties["MatchName"].ToString();
        }
        else {
            ViewName = "Match";
        }
	    MatchNameText.text = ViewName;
        UpdatePlayerCount(match.PlayerCount);
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
        FindObjectOfType<MenuManager>().OnClickJoinMatchButton();
        PhotonLobbyManager.Instance.JoinMatch(_match.Name);
	}

    public void UpdatePlayerCount(int playerCount) {
        Players.text = playerCount + "/" + _match.MaxPlayers;
    }

    public void UpdateMatchName(string name)
    {
        ViewName = name;
        MatchNameText.text = name;
    }
}
