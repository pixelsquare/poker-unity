
using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public struct CardMatchInfo : IComparable<CardMatchInfo>
{
    public int index;
    public List<Card> cards;
    public CardMatchType matchType;

    public Card GetCard(int idx)
    {
        return cards[idx];
    }

    public Card GetCardFirst()
    {
        return cards.FirstOrDefault();
    }

    public Card GetCardLast()
    {
        return cards.LastOrDefault();
    }

    public int CompareTo(CardMatchInfo info)
    {
        if(info.matchType == matchType)
        {
            return 0;
        }

        return matchType.CompareTo(info.matchType);
    }
}
