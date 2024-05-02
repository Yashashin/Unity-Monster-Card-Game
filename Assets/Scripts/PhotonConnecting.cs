using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class PhotonConnecting : MonoBehaviourPunCallbacks
{
    public string version = "0.1";
    public void ConnectToPhoton()
    {
        //Connect to server
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        //Set connecting information
        PhotonNetwork.GameVersion = version;
        //Set all players 
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 10;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = "Test" + Random.Range(0, 999);
    }

    //Call back for connecting to server
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to "+PhotonNetwork.CloudRegion+" server successfully");
        PhotonNetwork.JoinLobby();
        SceneManager.LoadScene(1); //go to loading scene
    }

    //Call back for disconnecting
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnect reason:" + cause.ToString());
    }
}
