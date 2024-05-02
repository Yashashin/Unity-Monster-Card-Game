using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardClickEvent : MonoBehaviour
{
    public GameManager gameManager;

    public void onClickEvent() //click card to play
    {
        if(gameManager!=null)
        {
            gameManager.PlayCard(gameObject.GetComponent<Card>());
        }
    }
}
