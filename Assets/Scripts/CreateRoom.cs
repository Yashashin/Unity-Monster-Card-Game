using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
public class CreateRoom : MonoBehaviourPunCallbacks
{
    public InputField createRoomName;
    public InputField enterPassword;

    public void CreateNewRoom()
    {
        if (createRoomName.text != "" && enterPassword.text != "")
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            roomOptions.EmptyRoomTtl = 10;
            roomOptions.PlayerTtl = 30000; //30 sec
            PhotonNetwork.JoinOrCreateRoom(createRoomName.text+"-"+enterPassword.text, roomOptions, TypedLobby.Default);
        }
    }
    
    public override void OnCreatedRoom()
    {
        Debug.Log("Create room successfully");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create room failed " + message);
    }

}
