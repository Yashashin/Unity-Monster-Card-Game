using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private RoomList roomlistPrefab;

    private List<RoomList> listings = new List<RoomList>();

    public InputField password;

    public override void OnJoinedRoom()
    {
        //Clear all btn
       for(int i=listings.Count-1;i>=0;i--)
        {
            Destroy(listings[i].gameObject);
            listings.RemoveAt(i);
        }
        PhotonNetwork.LoadLevel("Game"); //go to game scene
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomLists)
    {
        foreach(RoomInfo info in roomLists)
        {
            if(info.RemovedFromList)
            {
                int index = listings.FindIndex(x => x.roomInfo.Name == info.Name);
                if(index!=-1)
                {
                    Destroy(listings[index].gameObject);
                    listings.RemoveAt(index);
                }
            }
            else
            {
               
                int index = listings.FindIndex(x => x.roomInfo.Name == info.Name);
                if(index==-1)
                {
                    RoomList roomList = (RoomList)Instantiate(roomlistPrefab, content);
                    if(roomList!=null)
                    {
                        listings.Add(roomList);
                        roomList.SetRoomInfo(info);
                        roomList.inputPassword = password;
                    }
                }
            }
        }
    }

}
