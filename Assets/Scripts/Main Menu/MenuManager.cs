using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	public GameObject MainMenu, Controls, Credits, HostGame, JoinGame, MatchInProgressError, CreditsBackground;
	public Button CreateMatch;
	public InputField MatchName;
	public bool matchInProgress { private get; set; }
	private float _ortoSize;

	//------------------ Main Menu Buttons ------------------

	public void OnClickHostButton()
	{
		MainMenu.SetActive(false);
		HostGame.SetActive(true);
	}

	public void OnClickJoinButton()
	{
		MainMenu.SetActive(false);
		JoinGame.SetActive(true);
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
		MyLobbyManager.Instance.CreateMatch(MatchName.text, (uint)PlayerCountSelector.PlayersSelected);
		matchInProgress = true;
		MatchName.interactable = false;
		ToggleCreateMatchButton();
	}

	public void ToggleCreateMatchButton()
	{
		if (CreateMatch.gameObject.activeInHierarchy)
		{
			CreateMatch.gameObject.SetActive(false);
		}
		else
		{
			CreateMatch.gameObject.SetActive(true);
		}
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
				if (matchInProgress)
				{
					//Can't exit, show error
					MatchInProgressError.SetActive(true);
					Invoke("HideError", 1f);
				}
			}
			if (!matchInProgress && name != "Background")
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
