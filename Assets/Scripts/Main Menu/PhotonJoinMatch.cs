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
        //foreach (RoomInfo match in matches)
        //{
        //    Debug.Log("@OnReceivedRoomListUpdate Matchname: " + match.CustomProperties["MatchName"]);
        //}
        if (!_matchList)
        {
            _matchList = FindObjectOfType<PhotonMatchList>();
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
