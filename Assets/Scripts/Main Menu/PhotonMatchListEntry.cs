using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

/// <summary>
/// UI script: handles viewing the information of one match in the match list.
/// </summary>
public class PhotonMatchListEntry : MonoBehaviour
{

	public Text MatchNameText, PlayerCountText;
	public Button JoinButton;

    public string MatchName { get; private set; }
    public int PlayerCount { get; private set; }
    public int MaxPlayers { get; private set; }
    public string ViewName { get; private set; }

	/// <summary>
	/// Add a match's information to the UI's match list.
	/// </summary>
	public void FillMatchListEntry(string name, string viewName, int playerCount, int maxPlayers)
	{
        MatchName = name;
        ViewName = viewName;
        MaxPlayers = maxPlayers;
	    MatchNameText.text = ViewName;
        UpdatePlayerCount(playerCount);
		JoinButton.onClick.RemoveAllListeners();
		JoinButton.onClick.AddListener(JoinButtonListener);
	}

    /// <summary>
	/// Button listener for Join Button.
	/// </summary>
	private void JoinButtonListener()
	{
        ButtonSoundPlayer.Instance.PlayNextSound();
        JoinButton.GetComponentInChildren<Text>().text = "LEAVE";
        JoinButton.onClick.RemoveAllListeners();
        JoinButton.onClick.AddListener(LeaveButtonListener);
        FindObjectOfType<MenuManager>().OnClickJoinMatchButton(MatchName);
	}
    private void LeaveButtonListener() {
        ButtonSoundPlayer.Instance.PlayPrevSound();
        JoinButton.GetComponentInChildren<Text>().text = "JOIN";
        JoinButton.onClick.RemoveAllListeners();
        JoinButton.onClick.AddListener(JoinButtonListener);
        FindObjectOfType<MenuManager>().OnClickLeaveMatchButton();
    }


    public void UpdatePlayerCount(int playerCount) {
        PlayerCountText.text = playerCount + "/" + MaxPlayers;
    }

    public void UpdateMatchName(string name)
    {
        ViewName = name;
        MatchNameText.text = name;
    }
}
