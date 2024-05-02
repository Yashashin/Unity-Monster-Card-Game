using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class RoomList : MonoBehaviour
{
    public Text roomName;
    public RoomInfo roomInfo;
    public InputField inputPassword;
    public string passowrd;
   
    public void SetRoomInfo(RoomInfo info)
    {
        roomInfo = info;
        string displayName = info.Name.Substring(0, info.Name.IndexOf("-"));
        passowrd = info.Name.Substring(info.Name.IndexOf("-")+1);
        roomName.text = displayName + " " + info.PlayerCount+"/" + info.MaxPlayers;
    }

    public void onClickJoinRoom()
    {
        if (inputPassword.text == passowrd)
        {
            PhotonNetwork.JoinRoom(roomInfo.Name);
        }
        else
        {
            Debug.Log("Wrong Password!");
        }
    }
}
