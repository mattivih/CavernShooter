using UnityEngine;

public class PhotonJoinMatch : Photon.PunBehaviour
{
    public int MaxMatchesToList = 4; 

    private PhotonMatchList _matchList;

    /// <summary>
    /// Updates the matchlist (on Join Game Menu)
    /// </summary>
    public override void OnReceivedRoomListUpdate()
    {
        RoomInfo[] matches = PhotonNetwork.GetRoomList();
        if (!_matchList)
        {
            _matchList = FindObjectOfType<PhotonMatchList>();
        }

        if (matches.Length > 0)
        {
            _matchList.HideLoadingIcon();
            for (int i = 0; i < matches.Length && i < MaxMatchesToList; i++)
            {
                RoomInfo match = matches[i];
                _matchList.AddOrUpdateMatch(match.Name, match.CustomProperties["MatchName"].ToString(), match.PlayerCount, match.MaxPlayers);
            }
        }
    }
}
