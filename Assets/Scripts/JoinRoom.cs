using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class JoinRoom : MonoBehaviourPunCallbacks
{
   public void JoinTheRoom()
    {
        PhotonNetwork.JoinRoom(transform.GetChild(0).GetComponent<Text>().text);
    }
}
