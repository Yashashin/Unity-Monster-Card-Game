using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Monster,
    Energy,
    Effect,
    Field,
    None
}

public enum Type
{
    Water,
    Fire,
    Grass,
    None
}

public enum Effect
{
    Burn,
    Heal,
    Penetrate,
    Eruption,
    BurnItDown,
    HealPerRound,
    DrawTwoCardsPerRound,
    PowerUp,
    ShuffleAndDraw,
    RivalShuffleAndDraw,
    None

}
public class Card : MonoBehaviour
{
  //  public GameManager gameManger;
    public CardType cardType;
    public Type type;
    public string name;
    public int requiredEnergyNum;
    public bool isEvo;
    public int damage;
    public int hp;
    public string skill;
    public Effect effect;
}
