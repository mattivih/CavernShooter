using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI script: Fills in the information of a single player, who has joined a match lobby, to the player list.
/// </summary>
public class MyPlayerListEntry : MonoBehaviour
{

	public Text PlayerName, PlayerStatus;
	public Image ReadyIcon;

	[HideInInspector]
	public MyLobbyPlayer Player { get; set; }

	/// <summary>
	/// Fills the player name and sets status text to "Ready" or "Waiting..."
	/// </summary>
	public void FillPlayerListEntry(MyLobbyPlayer player)
	{
		Player = player;
		SetName(player.PlayerName);
		if (player.readyToBegin)
		{
			PlayerReady();
		}
		else
		{
			PlayerNotReady();
		}
	}

	/// <summary>
	/// Updates the player's name
	/// </summary>
	public void SetName(string name)
	{
		PlayerName.text = name;
	}

	/// <summary>
	/// Sets the player status indicator to READY
	/// </summary>
	public void PlayerReady()
	{
		ReadyIcon.enabled = true;
		PlayerStatus.text = "";
	}

	/// <summary>
	/// Sets the player status indicator to WAITING
	/// </summary>
	public void PlayerNotReady()
	{
		ReadyIcon.enabled = false;
		PlayerStatus.text = "Waiting...";
	}
}
