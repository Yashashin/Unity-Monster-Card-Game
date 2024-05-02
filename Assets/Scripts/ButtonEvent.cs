using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class ButtonEvent : MonoBehaviourPunCallbacks
{
    public GameManager gameManager;
    
    public void Attack()
    {
        gameManager.Attack();
    }

    public void EndTurn()
    {
        gameManager.EndTurn();
    }
    public void Disconnect()
    {
        StartCoroutine(DoSwitchScene());
    }

    IEnumerator DoSwitchScene()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene(0);
    }
}
