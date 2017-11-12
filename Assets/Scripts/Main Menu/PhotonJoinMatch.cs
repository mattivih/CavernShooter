using UnityEngine;

public class PhotonJoinMatch : Photon.PunBehaviour
{
    public int MaxMatchesToList = 4;

    private PhotonMatchlist _matchList;

    public void OnEnable()
	{
		PhotonLobbyManager.Instance.JoinLobby();
	}

    public void OnDisable()
    {
        PhotonLobbyManager.Instance.ExitLobby();
    }

    /// <summary>
    /// Updates the matchlist (on Join Game Menu)
    /// </summary>
    public override void OnReceivedRoomListUpdate()
    {
        RoomInfo[] matches = PhotonNetwork.GetRoomList();
        foreach (RoomInfo match in matches)
        {
            Debug.Log(match.Name);
        }
        if (!_matchList)
        {
            _matchList = FindObjectOfType<PhotonMatchlist>();
        }

        if (matches.Length > 0)
        {
            _matchList.HideLoadingIcon();
            for (int i = 0; i < matches.Length && i < MaxMatchesToList; i++)
            {
                _matchList.AddMatchToList(matches[i]);
            }
        }
    }
}
