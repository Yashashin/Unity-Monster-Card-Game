# Monster Card Game
遊戲程式設計Project3

`Executable file in /Build/HW3.exe`

`Read Chinese introduction in Introduction.pdf`

![image](https://github.com/Yashashin/Unity-Monster-Card-Game/blob/main/monster_card_preview.png)
## How to Play
![image](https://github.com/Yashashin/Unity-Monster-Card-Game/blob/main/monster_card_rule.png
)
### Rule:

1. Each player starts with 3 life points. Whenever a player is hit directly by a monster or their own monster is defeated, they lose one life point. A player loses the game if their life points reach zero.
2.  At the beginning of their turn, the player must click on the deck to draw a card.
3.  Player can only activate one effect card per turn.
4.  The host is the first-attacking player, who cannot attack on the first turn.
5.  If a player launches an attack, their turn ends.
6.  Only the frontmost monster (battle monster) can participate in battle.
7.  If the battle monster is defeated, the monsters behind it will take its place in order (left -> middle -> right).

### Deck
A deck consists of 30 cards, including:
- Flame Slime *5 (Monster Card)
- Flame Genie *5 (Monster Card)
- Flame Energy *10 (Energy Card)
- Tornado *2 (Effect Card)
- Dice of Destiny *4 (Effect Card)
- Power Potion *2 (Effect Card)
- Volcano *2 (Field Card)

Effect Explanation:
- Tornado
  - Shuffle the opponent's hand into their deck and draw one card.
- Dice of Destiny
  - Shuffle your own hand into the deck and randomly draw one to three cards.
- Power Potion
  - Increases the attack power of the battle monster by 20. (If there is no battle monster on the field, this card can still be activated, but it will have no effect.)
- Volcano
  - Provides a permanent effect where the player draws two cards per turn.

### Room:
Create Room:
- Enter a room name and password, then click "Create".
  
Join Room:
- Enter the password in the top right corner and select the desired room.select the room.
  
**ATTENTION!! Both players must be in the lobby at the same time for one to see the room created by the other.**

*IDE: Unity 2020.3*
