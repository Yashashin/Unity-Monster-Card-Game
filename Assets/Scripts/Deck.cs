using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum DeckType
{
    Fire,
    Water,
    Gradss
}

public class Deck : MonoBehaviour
{
    public DeckType deckType;
    public List<GameObject> remainCards;
    public bool isDrawCards;
    public bool lockDeck;
    void Start()
    {
        lockDeck = true;
        Shuffle();
    }

    public void Shuffle()
    {
        for(int i=0;i<remainCards.Count;i++)
        {
            GameObject temp = remainCards[i];
            int randomIndex = Random.Range(i, remainCards.Count);
            remainCards[i] = remainCards[randomIndex];
            remainCards[randomIndex] = temp;
        }
    }

    public List<Card> DrawCards(int number)
    {
        isDrawCards = false;
        lockDeck = true;
        List<Card> cards=new List<Card>();
        for(int i=0;i<number;i++)
        {
            if(remainCards.Count!=0)
            {
                cards.Add(remainCards[remainCards.Count - 1].GetComponent<Card>());
                Destroy(remainCards[remainCards.Count - 1]);
                remainCards.RemoveAt(remainCards.Count - 1);
            }
        }
        return cards;
    }

    public void AddCards(List<Card> cards)
    {
        for(int i=0;i<cards.Count;i++)
        {
            GameObject tmp = Instantiate(transform.GetChild(0).gameObject,transform.position,transform.rotation);
            Card tmpCard=tmp.GetComponent<Card>();
            CopyCard(tmpCard, cards[i]);
            SpriteRenderer sr = tmp.GetComponent<SpriteRenderer>();
            sr.sprite = cards[i].gameObject.GetComponent<Image>().sprite;
            tmp.transform.SetParent(transform, false);
            remainCards.Add(tmp);
           
        }
        Shuffle();
    }

    public void OnMouseDrag()
    {
        if (!lockDeck)
        {
            isDrawCards = true;
        }
    }

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
