
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Player
{
    public List<Card> cards;

    public static Player Empty { get { return default(Player); } }
}
