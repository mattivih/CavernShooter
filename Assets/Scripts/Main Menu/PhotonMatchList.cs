using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.Match;

/// <summary>
/// UI script: Matchlist that shows available matches, max players and currently joined players per match.
/// </summary>
public class PhotonMatchList : MonoBehaviour
{

	public GameObject MatchListEntryPrefab, NoMatchesFoundPrefab, LoadingPrefab;

	public MatchInfoSnapshot SelectedMatch { get; set; }

	private GameObject _errorNoMatchesFound, _loading;
	private List<Transform> _positions;
	private int _matchCount = 0;

	void Awake()
	{
		_loading = Instantiate(LoadingPrefab);
		_loading.SetActive(true);
		_loading.transform.SetParent(transform, false);

		_errorNoMatchesFound = Instantiate(NoMatchesFoundPrefab);
		_errorNoMatchesFound.SetActive(false);
		_errorNoMatchesFound.transform.SetParent(transform, false);

		//Get the matchlist entry transform positions
		_positions = GetComponentsInChildren<Transform>(true).ToList();

		//Delete parent object from the list
		Transform parent = null;
		foreach (var pos in _positions)
		{
			if (pos.gameObject.GetComponent<PhotonMatchList>())
			{
				parent = pos;
			}
		}
		_positions.Remove(parent);
	}

	public void AddOrUpdateMatch(string name, string viewName, int playerCount, int maxPlayers)
	{
	    PhotonMatchListEntry[] matchlist = GetComponentsInChildren<PhotonMatchListEntry>();
	    bool nameChanged = false;
	    PhotonMatchListEntry matchInList = null;
	    foreach (var entry in matchlist)
	    {
	        if (entry.MatchName == name)
	        {
	            matchInList = entry;
                if (!string.IsNullOrEmpty(viewName) && entry.ViewName != viewName)
	            {
	                nameChanged = true;
	            }
	        }
	    }
        if (!matchInList)
        {
            //Add match
            GameObject matchListEntry = Instantiate(MatchListEntryPrefab, _positions[_matchCount].transform.position, Quaternion.identity, _positions[_matchCount]);
            matchListEntry.GetComponent<PhotonMatchListEntry>().FillMatchListEntry(name, viewName, playerCount, maxPlayers);
            _matchCount++;
        }
        else if (nameChanged)
        {
            //Update match details
            matchInList.UpdateMatchName(viewName);
        }
	}

    public void UpdatePlayerCount(string matchName, int playerCount) {
        PhotonMatchListEntry[] matchlist = GetComponentsInChildren<PhotonMatchListEntry>();
        foreach (var entry in matchlist) {
            if (entry.MatchName == matchName) {
                entry.UpdatePlayerCount(playerCount);
            }
        }
    }

    /// <summary>
	/// Show message "No matches found" if no matches are found.
	/// </summary>
	public void ShowNoMatchesFound()
	{
		_errorNoMatchesFound.SetActive(true);
	}

	/// <summary>
	/// Hide message "No matches found".
	/// </summary>
	public void HideNoMatchesFound()
	{
		_errorNoMatchesFound.SetActive(false);
	}

	/// <summary>
	/// Show loading icon.
	/// </summary>
	public void ShowLoadingIcon()
	{
		_loading.SetActive(true);
	}

	/// <summary>
	/// Hide message loading icon.
	/// </summary>
	public void HideLoadingIcon()
	{
		_loading.SetActive(false);
	}
}
