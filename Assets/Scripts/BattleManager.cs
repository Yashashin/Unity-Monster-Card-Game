using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class BattleManager : MonoBehaviour
{
    public int monsterNum;

    //Cards in zone 
    public GameObject mainZoneCard;
    public GameObject backZoneLeft;
    public GameObject backZoneMiddle;
    public GameObject backZoneRight;
    public GameObject grave;
    public GameObject fieldCard;

    //Display numbers of energies.
    public Text energyText;
    public int energyCount;

    //Card Position
    public Transform p1_mainZone, p2_mainZone;
    public Transform p1_backZone1, p1_backZone2, p1_backZone3, p2_backZone1, p2_backZone2, p2_backZone3;
    public Transform p1_grave, p2_grave;
    public Transform field;


    //Put monster card to zone
    [PunRPC]
    public void SetMonsterCard(Card card)
    {
        if (PhotonNetwork.IsMasterClient) //p1
        {
            if (monsterNum == 0)
            {

                mainZoneCard = PhotonNetwork.Instantiate(card.name, p1_mainZone.position, p1_mainZone.rotation, 0);
                Card tmpCard = mainZoneCard.GetComponent<Card>();
                CopyCard(tmpCard, card);
            }
            else if (monsterNum == 1)
            {
                backZoneLeft = PhotonNetwork.Instantiate(card.name, p1_backZone1.position, p1_backZone1.rotation, 0);
                Card tmpCard = backZoneLeft.GetComponent<Card>();
                CopyCard(tmpCard, card);
            }
            else if (monsterNum == 2)
            {
                backZoneMiddle = PhotonNetwork.Instantiate(card.name, p1_backZone2.position, p1_backZone2.rotation, 0);
                Card tmpCard = backZoneMiddle.GetComponent<Card>();
                CopyCard(tmpCard, card);
            }
            else if (monsterNum == 3)
            {
                backZoneRight = PhotonNetwork.Instantiate(card.name, p1_backZone3.position, p1_backZone3.rotation, 0);
                Card tmpCard = backZoneRight.GetComponent<Card>();
                CopyCard(tmpCard, card);
            }
        }
        else //p2
        {
            if (monsterNum == 0)
            {
                mainZoneCard = PhotonNetwork.Instantiate(card.name, p2_mainZone.position, p2_mainZone.rotation, 0);
                Card tmpCard = mainZoneCard.GetComponent<Card>();
                CopyCard(tmpCard, card);
            }
            else if (monsterNum == 1)
            {
                backZoneLeft = PhotonNetwork.Instantiate(card.name, p2_backZone1.position, p2_backZone1.rotation, 0);
                Card tmpCard = backZoneLeft.GetComponent<Card>();
                CopyCard(tmpCard, card);
            }
            else if (monsterNum == 2)
            {
                backZoneMiddle = PhotonNetwork.Instantiate(card.name, p2_backZone2.position, p2_backZone2.rotation, 0);
                Card tmpCard = backZoneMiddle.GetComponent<Card>();
                CopyCard(tmpCard, card);
            }
            else if (monsterNum == 3)
            {
                backZoneRight = PhotonNetwork.Instantiate(card.name, p2_backZone3.position, p2_backZone3.rotation, 0);
                Card tmpCard = backZoneRight.GetComponent<Card>();
                CopyCard(tmpCard, card);
            }
        }
        monsterNum++;
    }

    //Put effect card to zone
    public void SetEffectCard(Card card)
    {
        if (PhotonNetwork.IsMasterClient) //p1
        {
            if (grave != null)
            {
                PhotonNetwork.Destroy(grave);
            }
            grave = PhotonNetwork.Instantiate(card.name, p1_grave.position, p1_grave.rotation, 0);
            Card tmpCard = grave.GetComponent<Card>();
            CopyCard(tmpCard, card);
        }
        else //p2
        {
            if (grave != null)
            {
                PhotonNetwork.Destroy(grave);
            }
            grave = PhotonNetwork.Instantiate(card.name, p2_grave.position, p2_grave.rotation, 0);
            Card tmpCard = grave.GetComponent<Card>();
            CopyCard(tmpCard, card);
        }
    }

    //Destroy file card
    public void RemoveFieldCard()
    {
        if (fieldCard != null)
        {
            PhotonNetwork.Destroy(fieldCard);
        }
    }

    //Put file card to zone
    public void SetFieldCard(Card card)
    {
        if(fieldCard!=null)
        {
            PhotonNetwork.Destroy(fieldCard);
        }

        fieldCard = PhotonNetwork.Instantiate(card.name, field.position, field.rotation, 0);
        Card tmpCard = fieldCard.GetComponent<Card>();
        CopyCard(tmpCard, card);
    }

    //Energy number increased by 1
    public void SetEnergy()
    {
        energyCount++;
        energyText.text = " X " + energyCount.ToString();
    }

    //Destroy monster in main zone and move monsters in back zone
    public void MonsterDied()
    {
        monsterNum--;
        if (PhotonNetwork.IsMasterClient) //p1
        {
            PhotonNetwork.Destroy(mainZoneCard.gameObject);
            if (backZoneLeft != null)
            {
                mainZoneCard = backZoneLeft;
                mainZoneCard.transform.position = p1_mainZone.position;
            }

            if (backZoneMiddle != null)
            {
                backZoneLeft = backZoneMiddle;
                backZoneLeft.transform.position = p1_backZone1.position;
            }
            else
            {
                backZoneLeft = null;
            }
            if (backZoneRight != null)
            {
                backZoneMiddle = backZoneRight;
                backZoneMiddle.transform.position = p1_backZone2.position;
                backZoneRight = null;
            }
            else
            {
                backZoneMiddle = null;
            }
        }
        else //p2 
        {
            PhotonNetwork.Destroy(mainZoneCard.gameObject);
            if (backZoneLeft != null)
            {
                mainZoneCard = backZoneLeft;
                mainZoneCard.transform.position = p2_mainZone.position;
            }

            if (backZoneMiddle != null)
            {
                backZoneLeft = backZoneMiddle;
                backZoneLeft.transform.position = p2_backZone1.position;
            }
            else
            {
                backZoneLeft = null;
            }
            if (backZoneRight != null)
            {
                backZoneMiddle = backZoneRight;
                backZoneMiddle.transform.position = p2_backZone2.position;
                backZoneRight = null;
            }
            else
            {
                backZoneMiddle = null;
            }
        }
    }

    public void Start()
    {
        energyText.text = " X 0"; //set energy number to 0
    }

    //Energy number decreased by 1
    public void ConsumeEnergy()
    {
        energyCount--;
        energyText.text = " X " + energyCount.ToString();
    }


    //Copy values of Card component to another object
    public void CopyCard(Card target, Card source)
    {
        target.cardType = source.cardType;
        target.type = source.type;
        target.name = source.name;
        target.requiredEnergyNum = source.requiredEnergyNum;
        target.isEvo = source.isEvo;
        target.damage = source.damage;
        target.hp = source.hp;
        target.skill = source.skill;
        target.effect = source.effect;
    }
}
