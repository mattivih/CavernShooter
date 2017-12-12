using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject MainMenu, Controls, Credits, HostGame, JoinGame;
    public GameObject ConnectingToServer, CreditsBackground;
    public InputField HostMatchNameInput;
    private Button _centerButton; //Create Match in Host Game Menu, Select Match in Join Game menu.
    private bool _hosting;
    private bool _customMatchName;

	private float _ortoSize;

	//------------------ Main Menu Buttons ------------------

	public void OnClickHostGameButton()
	{
        MainMenu.SetActive(false);
        if (PhotonNetwork.connectedAndReady)
        {
            OpenHostGame();
        }
        else {
            ConnectingToServer.SetActive(true);
            _hosting = true;
        }
    }

    public void OpenHostGame() {
        HostMatchNameInput.text = "";
        HostGame.SetActive(true);
        _centerButton = HostGame.GetComponent<ButtonManager>().GetCenterButton();
        _centerButton.GetComponentInChildren<Text>().text = "CREATE\nMATCH";
        _centerButton.onClick.RemoveAllListeners();
        _centerButton.onClick.AddListener(OnClickCreateMatchButton);
        _centerButton.interactable = true;
    }

    public void OnClickJoinGameButton()
	{
		MainMenu.SetActive(false);
        if (PhotonNetwork.connectedAndReady)
        {
            OpenJoinGame();
        }
        else {
            ConnectingToServer.SetActive(true);
            _hosting = false;
        }
    }

    public void OpenJoinGame() {
        JoinGame.SetActive(true);
        _centerButton = JoinGame.GetComponent<ButtonManager>().GetCenterButton();
        ShowSelectMatchTextOnCenterButton();
    }

    public void ShowSelectMatchTextOnCenterButton() {
        _centerButton = GameObject.Find("Center Button").GetComponent<Button>();
        _centerButton.onClick.RemoveAllListeners();
        _centerButton.GetComponentInChildren<Text>().text = "PLEASE\nSELECT\nMATCH";
        _centerButton.interactable = false;
    }

	public void OnClickControlsButton()
	{
		MainMenu.SetActive(false);
		Controls.SetActive(true);
	}

	public void OnClickCreditsButton()
	{
		MainMenu.SetActive(false);
		Credits.SetActive(true);
        CreditsBackground.SetActive(true);
        _ortoSize = Camera.main.orthographicSize;
		Camera.main.orthographic = false;
	}

	public void OnClickQuitButton()
	{
		Application.Quit();
	}


	//------------------ Host Game Buttons ------------------

	public void OnClickCreateMatchButton()
	{
	    string matchName = "";
	    if (_customMatchName)
	    {
	        matchName = HostMatchNameInput.text;
	    }
        PlayerCountSelector[] playerCounts = FindObjectsOfType<PlayerCountSelector>();
        foreach (var icon in playerCounts) {
            icon.SetInteractable(false);
        }
	    PhotonLobbyManager.Instance.CreateMatch(matchName, PlayerCountSelector.PlayersSelected);
        HostMatchNameInput.interactable = false;
        AddReadyListener();
	}

    public void OnMatchNameEdit()
    {
        _customMatchName = true;
    }

    public void OnMatchCreate(string name)
    {
        HostMatchNameInput.text = name;
    }

    //------------------ Join Game Buttons ------------------

    public void OnClickJoinMatchButton(string name) {
        PhotonLobbyManager.Instance.JoinMatch(name);
        AddReadyListener();
    }

    public void OnClickLeaveMatchButton() {
        PhotonLobbyManager.Instance.LeaveMatch();
        ShowSelectMatchTextOnCenterButton();
    }

    //------------------ Helper Functions ------------------


    public void AddReadyListener() {
        _centerButton.GetComponentInChildren<Text>().text = "I'M\nREADY";
        _centerButton.onClick.RemoveAllListeners();
        _centerButton.onClick.AddListener(OnClickReadyButton);
        _centerButton.interactable = true;
    }

    public void AddNotReadyListener()
    {
        _centerButton.GetComponent<Button>().onClick.AddListener(OnClickNotReadyButton);
    }

    public void OnClickReadyButton() {
        _centerButton.GetComponentInChildren<Text>().text = "I'M\nNOT\nREADY";
        PhotonLobbyManager.Instance.PlayerReady();
        _centerButton.onClick.RemoveAllListeners();
        _centerButton.onClick.AddListener(OnClickNotReadyButton);
    }

    public void OnClickNotReadyButton() {
        _centerButton.GetComponentInChildren<Text>().text = "I'M\nREADY";
        PhotonLobbyManager.Instance.PlayerNotReady();
        _centerButton.onClick.RemoveAllListeners();
        _centerButton.onClick.AddListener(OnClickReadyButton);
    }

    public void OnClickBackButton()
	{
		foreach (var canvas in FindObjectsOfType<Canvas>())
		{
			string name = canvas.gameObject.name;
			if (name == "Credits")
			{
				Camera.main.orthographic = true;
				Camera.main.orthographicSize = _ortoSize;

			}
			else if (name == "HostGame" || name == "JoinGame")
			{
				if (PhotonNetwork.inRoom)
				{
                    PhotonLobbyManager.Instance.LeaveMatch();
                }
			}
			if (name != "Background")
			{
				canvas.gameObject.SetActive(false);
				MainMenu.SetActive(true);
			}
		}
	}

    public void OnConnectedToServer()
    {
        ConnectingToServer.SetActive(false);
        if (_hosting)
        {
            OpenHostGame();
        }
        else {
            OpenJoinGame();
        }
    }
}
