using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public enum TableInfo
{
    MyDeck,
    RivalDeck,
    MyMainMonster,
    MyBackMonster1,
    MyBackMonster2,
    MyBackMonster3,
    RivalMainMonster,
    RivalBackMonster1,
    RivalBackMonster2,
    RivalBackMonster3
}
public class GameManager : MonoBehaviour
{
    public GameObject camera;

    //Prefab
    public GameObject deckPrefab;
    public GameObject cardInHandPrefab;

    //Hand
    public GameObject hand;
    public List<GameObject> cardsInHand;

    //Battle Manager
    public BattleManager battleManager;

    //Text
    public Text info;
    public Text log;
    public Text myDeckInfo;
    public Text rivalDeckInfo;
    public List<Text> myMonsterInfo;
    public List<Text> rivalMonsterInfo;
    public GameObject gameOverText;

    //Deck position
    public Transform p1_deck, p2_deck;

    //Button
    public GameObject attackBtn;
    public GameObject endTurnBtn;

    //Life token
    public List<GameObject> myLifeToken;
    public List<GameObject> rivalLifeToken;

    public int life;

    //Bool
    public bool isGameStart;
    public bool isWaitForDrawing;
    public bool hasLaunchEffect;
    public bool isFirstTurn;
    public bool isPlayStage;
    public bool isGameOver;

    //Private 
    private GameObject deck;
    private PhotonView view;
    private int drawCardNum; //number of cards you draw every turn, may be affected by card effect

    void Start()
    {
        //Set initial
        isGameStart = false;
        isGameOver = false;
        isWaitForDrawing = false;
        drawCardNum = 1;
        myDeckInfo.text = "30";
        rivalDeckInfo.text = "30";
        life = 3;
        for (int i = 0; i < 3; i++)
        {
            myLifeToken[i].SetActive(true);
            rivalLifeToken[i].SetActive(true);
        }
        view = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient) //p1
        {
            isFirstTurn = true;
            deck = PhotonNetwork.Instantiate(deckPrefab.name, p1_deck.position, p1_deck.rotation, 0); //create deck
        }
        else //p2
        {
            deck = PhotonNetwork.Instantiate(deckPrefab.name, p2_deck.position, p2_deck.rotation, 0); //create deck
            camera.transform.Rotate(new Vector3(0, 0, 180));
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom &&PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient && !isGameStart)
        {
            isGameStart = true;
            LogMessage(true, true, "Game start!");
            view.RPC("LogMessage", RpcTarget.Others, true, false, "Game start!");
            DrawInitialCard();
            view.RPC("DrawInitialCard", RpcTarget.Others);
            StartTurn();
        }
     
        if (PhotonNetwork.InRoom &&¡@isWaitForDrawing)
        {
            if (deck.GetComponent<Deck>().isDrawCards && !isGameOver)
            {
                DrawCard();
                isWaitForDrawing = false;
                PlayStage();
            }
        }
    }

    [PunRPC]
    public void RestartGame()
    {
        //Set initial
        isGameStart = false;
        isGameOver = false;
        isWaitForDrawing = false;
        drawCardNum = 1;
        myDeckInfo.text = "30";
        rivalDeckInfo.text = "30";
        life = 3;
        for (int i = 0; i < 3; i++)
        {
            myLifeToken[i].SetActive(true);
            rivalLifeToken[i].SetActive(true);
        }
        if (PhotonNetwork.IsMasterClient) //p1
        {
            isFirstTurn = true;
            PhotonNetwork.Destroy(deck);
            deck = PhotonNetwork.Instantiate(deckPrefab.name, p1_deck.position, p1_deck.rotation, 0); //create deck
        }
        else //p2
        {
            PhotonNetwork.Destroy(deck);
            deck = PhotonNetwork.Instantiate(deckPrefab.name, p2_deck.position, p2_deck.rotation, 0); //create deck
        }
    }

    [PunRPC]
    public void StartTurn()
    {
        hasLaunchEffect = false;
        info.text = "Your Turn";
        view.RPC("SetInfo", RpcTarget.Others, "Opponent's Turn");
        isWaitForDrawing = true; //enable drawing cards
        deck.GetComponent<Deck>().lockDeck = false; //unlock deck to draw
    }

    public void DrawCard()
    {
        LogMessage(false, true, "Draw card.");
        view.RPC("LogMessage", RpcTarget.Others, false, false, "Draw card.");

        List<Card> cards = deck.GetComponent<Deck>().DrawCards(drawCardNum); //draw card
        if (deck.GetComponent<Deck>().remainCards.Count < drawCardNum) //out of deck
        {
            LogMessage(false, true, "Out of deck!");
            view.RPC("LogMessage", RpcTarget.Others, false, false, "Out of deck!");
        }

        //Instantiate cards to hand
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject tmp = Instantiate(cardInHandPrefab, hand.transform);
            Card card = tmp.GetComponent<Card>();
            CopyCard(card, cards[i]);
            Image sr = tmp.GetComponent<Image>();
            sr.sprite = cards[i].gameObject.GetComponent<SpriteRenderer>().sprite;
            cardsInHand.Add(tmp);
            tmp.GetComponent<CardClickEvent>().gameManager = this;
        }


        myDeckInfo.text = deck.GetComponent<Deck>().remainCards.Count.ToString();
        view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalDeck, deck.GetComponent<Deck>().remainCards.Count.ToString(),true);
    }


    //Draw cards when game begins
    [PunRPC]
    public void DrawInitialCard()
    {
        List<Card> cards = deck.GetComponent<Deck>().DrawCards(4);
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject tmp = Instantiate(cardInHandPrefab, hand.transform);
            Card card = tmp.GetComponent<Card>();
            CopyCard(card, cards[i]);
            Image sr = tmp.GetComponent<Image>();
            sr.sprite = cards[i].gameObject.GetComponent<SpriteRenderer>().sprite;
            cardsInHand.Add(tmp);
            tmp.GetComponent<CardClickEvent>().gameManager = this;
        }
        myDeckInfo.text = "26";
        view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalDeck, "26",true);
    }

    public void PlayStage()
    {
        isPlayStage = true;
        attackBtn.SetActive(true); //enable attack
        endTurnBtn.SetActive(true); //enable end turn
    }

    //Play a card form hand
    public void PlayCard(Card card)
    {
        if (isPlayStage)
        {
            if (card.cardType == CardType.Monster) //play monster card
            {
                if (battleManager.monsterNum != 4)
                {
                    switch (battleManager.monsterNum)
                    {
                        case 0:
                            myMonsterInfo[0].enabled = true;
                            myMonsterInfo[0].text = card.hp.ToString() + "/" + card.damage.ToString();
                            view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalMainMonster, card.hp.ToString() + "/" + card.damage.ToString(),true);
                            break;
                        case 1:
                            myMonsterInfo[1].enabled = true;
                            myMonsterInfo[1].text = card.hp.ToString() + "/" + card.damage.ToString();
                            view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalBackMonster1, card.hp.ToString() + "/" + card.damage.ToString(),true);
                            break;
                        case 2:
                            myMonsterInfo[2].enabled = true;
                            myMonsterInfo[2].text = card.hp.ToString() + "/" + card.damage.ToString();
                            view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalBackMonster2, card.hp.ToString() + "/" + card.damage.ToString(),true);
                            break;
                        case 3:
                            myMonsterInfo[3].enabled = true;
                            myMonsterInfo[3].text = card.hp.ToString() + "/" + card.damage.ToString();
                            view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalBackMonster3, card.hp.ToString() + "/" + card.damage.ToString(),true);
                            break;
                    }
                    battleManager.SetMonsterCard(card);
                    LogMessage(false, true, "Played " + card.name + ".");
                    view.RPC("LogMessage", RpcTarget.Others, false, false, "Played " + card.name + ".");
                    cardsInHand.RemoveAt(cardsInHand.FindIndex(x => x == card.gameObject));
                    Destroy(card.gameObject);
                }
                else
                {
                    LogMessage(true, true, "Monster zones are full");
                }
            }
            else if (card.cardType == CardType.Energy) //play energy card
            {
                battleManager.SetEnergy();
                LogMessage(false, true, "Played " + card.name + ".");
                view.RPC("LogMessage", RpcTarget.Others, false, false, "Played " + card.name + ".");
                cardsInHand.RemoveAt(cardsInHand.FindIndex(x => x == card.gameObject));
                Destroy(card.gameObject);
            }
            else if (card.cardType == CardType.Effect && !hasLaunchEffect) //play effect card
            {
                battleManager.SetEffectCard(card);
                LogMessage(false, true, "Played " + card.name + ".");
                view.RPC("LogMessage", RpcTarget.Others, false, false, "Played " + card.name + ".");
                Effect effect = card.effect;
                cardsInHand.RemoveAt(cardsInHand.FindIndex(x => x == card.gameObject));
                Destroy(card.gameObject);
                LaunchEffect(effect);
                hasLaunchEffect = true;
            }
            else if (card.cardType == CardType.Field) //play filed card
            {
                battleManager.SetFieldCard(card);
                LogMessage(false, true, "Played " + card.name + ".");
                view.RPC("LogMessage", RpcTarget.Others, false, false, "Played " + card.name + ".");
                view.RPC("RemoveFieldCard", RpcTarget.Others);
                Effect effect = card.effect;
                cardsInHand.RemoveAt(cardsInHand.FindIndex(x => x == card.gameObject));
                Destroy(card.gameObject);
                LaunchEffect(effect);
            }
            else
            {
                LogMessage(true, true, "You had launched effect card in this turn");
            }
        }
    }

    public void EndTurn()
    {
        isFirstTurn = false;
        endTurnBtn.SetActive(false);
        attackBtn.SetActive(false);
        isPlayStage = false;
        info.text = "Opponent's Turn";
        view.RPC("StartTurn", RpcTarget.Others);
    }

    //Modify text of information
    [PunRPC]
    public void SetInfo(string text)
    {
        info.text = text;
    }

    //Copy values of Card 
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

    //Log messages to Game Log
    [PunRPC]
    public void LogMessage(bool isSystemMsg, bool isSelfMsg, string msg)
    {
        if (isSystemMsg)
        {
            log.text += "[System]: ";
        }
        else if (isSelfMsg)
        {
            log.text += "[You]: ";
        }
        else
        {
            log.text += "[Opponent]: ";
        }
        log.text += (msg + "\n");
    }


    public void LaunchEffect(Effect effect)
    {
        switch (effect)
        {
            case Effect.PowerUp:
                if (battleManager.mainZoneCard != null)
                {
                    battleManager.mainZoneCard.GetComponent<Card>().damage += 20;
                    myMonsterInfo[0].text = battleManager.mainZoneCard.GetComponent<Card>().hp.ToString() + "/" + battleManager.mainZoneCard.GetComponent<Card>().damage.ToString();
                    view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalMainMonster, battleManager.mainZoneCard.GetComponent<Card>().hp.ToString() + "/" + battleManager.mainZoneCard.GetComponent<Card>().damage.ToString(),true);
                    LogMessage(false, true, battleManager.mainZoneCard.GetComponent<Card>().name + " increased damage to " + battleManager.mainZoneCard.GetComponent<Card>().damage + ".");
                    view.RPC("LogMessage", RpcTarget.Others, false, false, battleManager.mainZoneCard.GetComponent<Card>().name + " increased damage to " + battleManager.mainZoneCard.GetComponent<Card>().damage + ".");
                }
                else
                {
                    LogMessage(true, false, "There's no monster exist.");
                }
                break;
            case Effect.ShuffleAndDraw:
                int rand = Random.Range(1, 3);
                LogMessage(false, true, "Dice was rolled to " + rand.ToString() + ".");
                view.RPC("LogMessage", RpcTarget.Others, false, false, "Dice was rolled to " + rand.ToString() + ".");
                if (deck.GetComponent<Deck>().remainCards.Count >= rand)
                {
                    List<Card> cards = new List<Card>();
                    for (int i = 0; i < cardsInHand.Count; i++)
                    {
                        cards.Add(cardsInHand[i].GetComponent<Card>());
                        Destroy(cardsInHand[i]);
                    }
                    deck.GetComponent<Deck>().AddCards(cards);
                    cardsInHand.RemoveRange(0, cardsInHand.Count);
                    int tmp = drawCardNum;
                    drawCardNum = rand;
                    DrawCard();
                    drawCardNum = tmp;
                }
                else
                {
                    LogMessage(false, true, "Deck had insufficient cards to draw.");
                    view.RPC("LogMessage", RpcTarget.Others, false, false, "Deck had insufficient cards to draw.");
                }
                break;
            case Effect.RivalShuffleAndDraw:
                view.RPC("ShuffleAndDrawOne", RpcTarget.Others);
                break;
            case Effect.Eruption:
                drawCardNum = 2;
                break;
        }
    }

    //Shuffle cards to deck and draw one card
    [PunRPC]
    public void ShuffleAndDrawOne()
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cards.Add(cardsInHand[i].GetComponent<Card>());
            Destroy(cardsInHand[i]);
        }
        cardsInHand.Clear();
        deck.GetComponent<Deck>().AddCards(cards);

        cards = deck.GetComponent<Deck>().DrawCards(1);
        if (deck.GetComponent<Deck>().remainCards.Count < drawCardNum) //out of deck
        {
            LogMessage(false, true, "Out of deck!");
            view.RPC("LogMessage", RpcTarget.Others, false, false, "Out of deck!");
        }
        //Instantiate cards to hand
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject tmp = Instantiate(cardInHandPrefab, hand.transform);
            Card card = tmp.GetComponent<Card>();
            CopyCard(card, cards[i]);
            Image sr = tmp.GetComponent<Image>();
            sr.sprite = cards[i].gameObject.GetComponent<SpriteRenderer>().sprite;
            cardsInHand.Add(tmp);
            tmp.GetComponent<CardClickEvent>().gameManager = this;
        }


        myDeckInfo.text = deck.GetComponent<Deck>().remainCards.Count.ToString();
        view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalDeck, deck.GetComponent<Deck>().remainCards.Count.ToString(), true);
    }


    [PunRPC]
    public void UpdateTableInfo(TableInfo tableInfo, string text,bool isActive)
    {
        switch (tableInfo)
        {
            case TableInfo.MyDeck:
                myDeckInfo.text = text;
                break;
            case TableInfo.RivalDeck:
                rivalDeckInfo.text = text;
                break;
            case TableInfo.MyMainMonster:
                myMonsterInfo[0].text = text;
                myMonsterInfo[0].enabled = isActive;
                break;
            case TableInfo.MyBackMonster1:
                myMonsterInfo[1].text = text;
                myMonsterInfo[1].enabled = isActive;
                break;
            case TableInfo.MyBackMonster2:
                myMonsterInfo[2].text = text;
                myMonsterInfo[2].enabled = isActive;
                break;
            case TableInfo.MyBackMonster3:
                myMonsterInfo[3].text = text;
                myMonsterInfo[3].enabled = isActive;
                break;
            case TableInfo.RivalMainMonster:
                rivalMonsterInfo[0].text = text;
                rivalMonsterInfo[0].enabled = isActive;
                break;
            case TableInfo.RivalBackMonster1:
                rivalMonsterInfo[1].text = text;
                rivalMonsterInfo[1].enabled = isActive;
                break;
            case TableInfo.RivalBackMonster2:
                rivalMonsterInfo[2].text = text;
                rivalMonsterInfo[2].enabled = isActive;
                break;
            case TableInfo.RivalBackMonster3:
                rivalMonsterInfo[3].text = text;
                rivalMonsterInfo[3].enabled = isActive;
                break;
        }
    }


    [PunRPC]
    public void BeAttacked(int damage)
    {
        if (battleManager.monsterNum == 0) //if no monster, lose 1 hp
        {
            life--;
            LostLife(life, true);
            view.RPC("LostLife", RpcTarget.Others, life, false);
            LogMessage(false, true, "Lost 1 life.");
            view.RPC("LogMessage", RpcTarget.Others, false, false, "Lost 1 life.");

        }
        else
        {
            battleManager.mainZoneCard.GetComponent<Card>().hp -= damage;
            myMonsterInfo[0].text = battleManager.mainZoneCard.GetComponent<Card>().hp + "/" + battleManager.mainZoneCard.GetComponent<Card>().damage;
            view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalMainMonster, battleManager.mainZoneCard.GetComponent<Card>().hp + "/" + battleManager.mainZoneCard.GetComponent<Card>().damage,true);
            if (battleManager.mainZoneCard.GetComponent<Card>().hp <= 0) //monster died
            {
                LogMessage(false, true, battleManager.mainZoneCard.GetComponent<Card>().name + " was killed.");
                view.RPC("LogMessage", RpcTarget.Others, false, false, battleManager.mainZoneCard.GetComponent<Card>().name + " was killed.");
                battleManager.MonsterDied();

                myMonsterInfo[0].text = battleManager.mainZoneCard.GetComponent<Card>().hp + "/" + battleManager.mainZoneCard.GetComponent<Card>().damage;
                view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalMainMonster, battleManager.mainZoneCard.GetComponent<Card>().hp.ToString() + "/" + battleManager.mainZoneCard.GetComponent<Card>().damage.ToString(),true);
                if(battleManager.monsterNum==0)
                {
                    myMonsterInfo[0].enabled = false;
                    view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalMainMonster, "", false);
                }
                if (battleManager.monsterNum == 1)
                {
                    myMonsterInfo[1].enabled = false;
                    view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalBackMonster1,"", false);
                }
                else if (battleManager.monsterNum == 2)
                {
                    myMonsterInfo[1].text = battleManager.backZoneLeft.GetComponent<Card>().hp + "/" + battleManager.backZoneLeft.GetComponent<Card>().damage;
                    view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalBackMonster1, battleManager.backZoneLeft.GetComponent<Card>().hp.ToString() + "/" + battleManager.backZoneLeft.GetComponent<Card>().damage.ToString(),true);
                    myMonsterInfo[2].enabled = false;
                    view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalBackMonster2,"", false);
                }
                else if (battleManager.monsterNum == 3)
                {
                    myMonsterInfo[1].text = battleManager.backZoneLeft.GetComponent<Card>().hp + "/" + battleManager.backZoneLeft.GetComponent<Card>().damage;
                    view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalBackMonster1, battleManager.backZoneLeft.GetComponent<Card>().hp.ToString() + "/" + battleManager.backZoneLeft.GetComponent<Card>().damage.ToString(),true);
                    myMonsterInfo[2].text = battleManager.backZoneMiddle.GetComponent<Card>().hp + "/" + battleManager.backZoneMiddle.GetComponent<Card>().damage;
                    view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalBackMonster2, battleManager.backZoneMiddle.GetComponent<Card>().hp.ToString() + "/" + battleManager.backZoneMiddle.GetComponent<Card>().damage.ToString(),true);
                    myMonsterInfo[3].enabled = false;
                    view.RPC("UpdateTableInfo", RpcTarget.Others, TableInfo.RivalBackMonster3, "", false);
                }
                life--;
                LostLife(life, true);
                view.RPC("LostLife", RpcTarget.Others, life, false);
            }
        }
        if (life <= 0)
        {
            LoseGame();
            view.RPC("WinGame", RpcTarget.Others);
        }
    }

    public void LoseGame()
    {
        gameOverText.SetActive(true);
        gameOverText.GetComponent<Text>().text = "You Lose!";
        info.enabled = false;
        isPlayStage = false;
        LogMessage(true, false, "You Lose!");
        isGameOver = true;
    }

    [PunRPC]
    public void WinGame()
    {
        gameOverText.SetActive(true);
        gameOverText.GetComponent<Text>().text = "You Win!";
        info.enabled = false;
        isPlayStage = false;
        LogMessage(true, false, "You win!");
        isGameOver = true;
    }

    public void Attack()
    {
        if (battleManager.monsterNum > 0)
        {
            if (!isFirstTurn)
            {
                if (battleManager.energyCount > 0)
                {
                    battleManager.ConsumeEnergy();
                    LogMessage(false, true, "Attacked.");
                    view.RPC("LogMessage", RpcTarget.Others, false, false, "Attacked.");
                    view.RPC("BeAttacked", RpcTarget.Others, battleManager.mainZoneCard.GetComponent<Card>().damage);
                    if (!isGameOver)
                    {
                        EndTurn();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    LogMessage(true, true, "You had insufficent energy to attack.");
                }
            }
            else
            {
                LogMessage(true, true, "First turn cannot attack.");
            }

        }
    }



    [PunRPC]
    public void RemoveFieldCard()
    {
        battleManager.RemoveFieldCard();
    }

    [PunRPC]
    public void LostLife(int life, bool isMine)
    {
        if (isMine)
        {
            switch (life)
            {
                case 2:
                    myLifeToken[2].SetActive(false);
                    break;
                case 1:
                    myLifeToken[1].SetActive(false);
                    break;
                case 0:
                    myLifeToken[0].SetActive(false);
                    break;
            }
        }
        else
        {
            switch (life)
            {
                case 2:
                    rivalLifeToken[2].SetActive(false);
                    break;
                case 1:
                    rivalLifeToken[1].SetActive(false);
                    break;
                case 0:
                    rivalLifeToken[0].SetActive(false);
                    break;
            }
        }

    }
}
