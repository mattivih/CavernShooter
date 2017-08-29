using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyLobbyPlayer : NetworkLobbyPlayer
{
	[SyncVar(hook = "OnLobbyPlayerOnClientNameChange")]
	public string PlayerName;
	public int ConnectionId { get; set; }
	public Button ReadyButton;
	public InputField NameInput;
	public static int PlayerNum { get; set; }

	/// <summary>
	/// When a player enters match's lobby give a name to the player and add the players name and status to the UI's Player List.
	/// </summary>
	public override void OnClientEnterLobby()
	{
		base.OnClientEnterLobby();
		GetComponent<Canvas>().worldCamera = Camera.main;

		ReadyButton.gameObject.SetActive(false);
		NameInput.gameObject.SetActive(false);
		PlayerNum++;

		// Need to wait a frame so that isLocalPlayer is set...
		StartCoroutine("InitPlayer");
	}

	/// <summary>
	/// Waits for one frame so the isLocalPlayer is set.
	/// </summary>
	public IEnumerator InitPlayer()
	{
		yield return new WaitForEndOfFrame();

		//Setup local player
		if (isLocalPlayer)
		{
			SetupLocalPlayer();
		}
		else
		{
			SetupRemotePlayer();
		}
	}

	/// <summary>
	/// Sets the local player's ready button interactable.
	/// </summary>
	private void SetupLocalPlayer()
	{
		//Send the name to the server
		PlayerName = "Player " + PlayerNum;
		NameInput.text = PlayerName;
		OnLobbyServerOnClientNameChange(PlayerName);

		//Add listener for name change
		NameInput.onEndEdit.RemoveAllListeners();
		NameInput.onEndEdit.AddListener(OnLobbyServerOnClientNameChange);
		NameInput.gameObject.SetActive(true);

		ReadyButton.gameObject.SetActive(true);
		ReadyButton.onClick.RemoveAllListeners();
		ReadyButton.onClick.AddListener(OnReadyClicked);
		ReadyButton.interactable = true;

		//TODO: Callback function instead of direct call to MyLobbymanager?
		MyLobbyManager.Instance.AddPlayerToPlayerList(this);

	}

	/// <summary>
	/// Setups a remote player.
	/// </summary>
	private void SetupRemotePlayer()
	{
		NameInput.interactable = false;
		MyLobbyManager.Instance.AddPlayerToPlayerList(this);
	}

	/// <summary>
	/// Listener for Lobby player' Ready button
	/// </summary>
	public void OnReadyClicked()
	{
		//Forward the selected ship information
		ReadyButton.transform.GetChild(0).GetComponent<Text>().fontSize -= 5;
		ReadyButton.transform.GetChild(0).GetComponent<Text>().text = "WAITING...";

		//Inform server
		CmdSaveSelectedShip(FindObjectOfType<ShipManager>().SelectedShip);
		SendReadyToBeginMessage();
	}

	/// <summary>
	/// Informs the server which ship the player has chosen.
	/// </summary>
	[Command]
	public void CmdSaveSelectedShip(int ship)
	{
		MyLobbyManager.Instance.SaveShipForPlayer(ConnectionId, ship);
	}

	[ClientRpc]
	public void RpcUpdatePlayerStatus()
	{
		UpdateUIStatus();
	}
	//------------------ UI management ------------------

	public override void OnClientReady(bool readyState)
	{
		base.OnClientReady(readyState);
		UpdateUIStatus();
	}

	public void UpdateUIStatus()
	{
		if (MyUIPlayerList.Instance)
		{
			MyUIPlayerList.Instance.UpdatePlayerStatus(this);
		}
	}

	/// <summary>
	/// When the player gets destroyed it's removed from any UI player lits.
	/// </summary>
	//public void OnDestroy()
	//{
	//	MyUIPlayerList.Instance.DeletePlayer(this);
	//}

	/// <summary>
	/// Callback for SyncVar Name.
	/// Input field listener for PlayerName.
	/// Informs the client of the host's name change.
	/// </summary>
	public void OnLobbyPlayerOnClientNameChange(string newName)
	{
		PlayerName = newName;
		MyUIPlayerList.Instance.UpdatePlayerName(this);
	}

	/// <summary>
	/// Informs the server of the client's name change.
	/// </summary>
	/// <param name="name"></param>
	public void OnLobbyServerOnClientNameChange(string name)
	{
		CmdNameChanged(name);
	}

	[Command]
	public void CmdNameChanged(string name)
	{
		PlayerName = name;
        MyUIPlayerList.Instance.UpdatePlayerName(this);
	}

	//TODO: I'm not ready - SendNotReadyToBeginMessage
}
