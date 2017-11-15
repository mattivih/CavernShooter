using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	public GameObject MainMenu, Controls, Credits, HostGame, JoinGame, MatchInProgressError, CreditsBackground;
    private Button _centerButton; //Create Match in Host Game Menu, Select Match in Join Game menu.
	public InputField MatchName;
	public bool MatchInProgress { private get; set; }

	private float _ortoSize;

	//------------------ Main Menu Buttons ------------------

	public void OnClickHostGameButton()
	{
		MainMenu.SetActive(false);
		HostGame.SetActive(true);
        _centerButton = GameObject.Find("Center Button").GetComponent<Button>();
        _centerButton.GetComponentInChildren<Text>().text = "CREATE\nMATCH";
        _centerButton.onClick.RemoveAllListeners();
        _centerButton.onClick.AddListener(OnClickCreateMatchButton);
        _centerButton.interactable = true;
    }

	public void OnClickJoinGameButton()
	{
		MainMenu.SetActive(false);
		JoinGame.SetActive(true);
        _centerButton = GameObject.Find("Center Button").GetComponent<Button>();
        _centerButton.GetComponentInChildren<Text>().text = "SELECT\nMATCH";
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
		AudioManager.Instance.OnEnterCreditsMenu();
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
        PhotonLobbyManager.Instance.CreateMatch(MatchName.text, PlayerCountSelector.PlayersSelected);
		MatchInProgress = true;
		MatchName.interactable = false;
        AddReadyListener();
	}

    //------------------ Join Game Buttons ------------------

    public void OnClickJoinMatchButton() {
        AddReadyListener();
    }


    public void AddReadyListener() {
        _centerButton.GetComponentInChildren<Text>().text = "I'M\nREADY";
        _centerButton.onClick.RemoveAllListeners();
        _centerButton.onClick.AddListener(OnClickReadyButton);
    }

    public void AddNotReadyListener()
    {
        _centerButton.GetComponent<Button>().onClick.AddListener(OnClickNotReadyButton);
    }

    public void OnClickReadyButton() {
        _centerButton.GetComponentInChildren<Text>().text = "I'M\nNOT\nREADY";
        PhotonLobbyManager.Instance.PlayerReady(PhotonNetwork.player);
        _centerButton.onClick.RemoveAllListeners();
        _centerButton.onClick.AddListener(OnClickNotReadyButton);
    }

    public void OnClickNotReadyButton() {
        _centerButton.GetComponentInChildren<Text>().text = "I'M\nREADY";
        PhotonLobbyManager.Instance.PlayerNotReady(PhotonNetwork.player);
        _centerButton.onClick.RemoveAllListeners();
        _centerButton.onClick.AddListener(OnClickReadyButton);
    }



    //------------------ Helper Functions ------------------

    public void OnClickBackButton()
	{
		foreach (var canvas in FindObjectsOfType<Canvas>())
		{
			string name = canvas.gameObject.name;
			if (name == "Credits")
			{
				AudioManager.Instance.OnEnterMainMenu();
				Camera.main.orthographic = true;
				Camera.main.orthographicSize = _ortoSize;

			}
			else if (name == "HostGame" || name == "JoinGame")
			{
				if (MatchInProgress)
				{
					//Can't exit, show error
					MatchInProgressError.SetActive(true);
					Invoke("HideError", 1f);
				}
			}
			if (!MatchInProgress && name != "Background")
			{
				canvas.gameObject.SetActive(false);
				MainMenu.SetActive(true);
			}
		}
	}

	void HideError()
	{
		MatchInProgressError.SetActive(false);
	}

}
