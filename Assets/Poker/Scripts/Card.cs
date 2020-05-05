
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Card : IComparable<Card>
{
    public int rank;

    public CardType suit;

    private Sprite suitIcon;

    public string RankString
    {
        get { return Card.GetRankString(this.rank); }
    }

    public Sprite SuitIcon
    {
        get
        {
            if(suitIcon == null)
            {
                suitIcon = Card.GetSuitIcon(suit);
            }

            return suitIcon;
        }
    }

    public Color SuitColor
    {
        get { return Card.GetSuitColor(suit); }
    }

    public int CompareTo(Card card)
    {
        if(card.rank == rank)
        {
            return 0;
        }

        return rank.CompareTo(card.rank);
    }

    public static Card Empty
    {
        get 
        { 
            return new Card()
            {
                rank = 0,
                suit = CardType.DIAMOND
            };
        }
    }

    public static string GetRankString(int rank)
    {
        switch(rank)
        {
            case 14:
                return "A";
            case 11:
                return "J";
            case 12:
                return "Q";
            case 13:
                return "K";
            default:
                return rank.ToString();
        }
    }

    public static Sprite GetSuitIcon(CardType suit)
    {
        switch(suit)
        {
            case CardType.DIAMOND:
                return Resources.LoadAll<Sprite>("icons")[1];
            case CardType.HEARTS:
                return Resources.LoadAll<Sprite>("icons")[2];
            case CardType.SPADES:
                return Resources.LoadAll<Sprite>("icons")[3];
            case CardType.CLUBS:
                return Resources.LoadAll<Sprite>("icons")[0];
        }

        return null;
    }

    public static Color GetSuitColor(CardType suit)
    {
        switch(suit)
        {
            case CardType.DIAMOND:
            case CardType.HEARTS:
                Color outColor = Color.red;
                ColorUtility.TryParseHtmlString("#9E141D", out outColor);
                return outColor;
            case CardType.SPADES:
            case CardType.CLUBS:
                return Color.black;
            default:
                return Color.black;
        }
    }
}