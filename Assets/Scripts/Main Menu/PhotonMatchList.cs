using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.Match;

/// <summary>
/// UI script: Matchlist that shows available matches, max players and currently joined players per match.
/// </summary>
public class PhotonMatchlist : MonoBehaviour
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
			if (pos.gameObject.GetComponent<PhotonMatchlist>())
			{
				parent = pos;
			}
		}
		_positions.Remove(parent);
	}

	public void AddMatchToList(RoomInfo match)
	{
		GameObject matchListEntry = Instantiate(MatchListEntryPrefab, _positions[_matchCount].transform.position, Quaternion.identity, _positions[_matchCount]);
		matchListEntry.GetComponent<PhotonMatchlistEntry>().FillMatchListEntry(match);
		_matchCount++;
	}

    public void UpdatePlayerCount(RoomInfo match) {
        PhotonMatchlistEntry[] matchlist = GetComponentsInChildren<PhotonMatchlistEntry>();
        foreach (var entry in matchlist) {
            if (entry.MatchName.text == match.Name) {
                entry.UpdatePlayerCount(PhotonNetwork.room.PlayerCount);
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
