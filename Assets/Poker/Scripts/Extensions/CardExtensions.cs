
using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class CardExtensions
{
    public static CardMatchType GetCardMatchType(this List<Card> cards)
    {
        CardMatchType result = CardMatchType.HIGH_CARDS;

        if(HasOnePairCards(cards))
        {
            result = CardMatchType.ONE_PAIR;
        }

        if(HasTwoPairCards(cards))
        {
            result = CardMatchType.TWO_PAIR;
        }
        
        if(HasThreeOfAKindCards(cards))
        {
            result = CardMatchType.THREE_OF_A_KIND;
        }

        if(HasStraightCards(cards))
        {
            result = CardMatchType.STRAIGHT;
        }

        if(HasFlushCards(cards))
        {
            result = CardMatchType.FLUSH;
        }

        if(HasFullHouseCards(cards))
        {
            result = CardMatchType.FULL_HOUSE;
        }

        if(HasFourOfAKindCards(cards))
        {
            result = CardMatchType.FOUR_OF_A_KIND;
        }

        if(HasStraightFlushCards(cards))
        {
            result = CardMatchType.STRAIGHT_FLUSH;
        }

        if(HasRoyalFlushCards(cards))
        {
            result = CardMatchType.ROYAL_FLUSH;
        }

        return result;
    }

    public static bool HasOnePairCards(this List<Card> cards)
    {
        return cards.GroupBy(a => a.rank).Where(b => b.Count() == 2).Count() >= 1;
    }

    public static bool HasTwoPairCards(this List<Card> cards)
    {
        return cards.GroupBy(a => a.rank).Where(b => b.Count() == 2).Count() >= 2;
    }

    public static bool HasThreeOfAKindCards(this List<Card> cards)
    {
        return cards.GroupBy(a => a.rank).Where(b => b.Count() == 3).Count() >= 1;
    }
    
    public static bool HasStraightCards(this List<Card> cards)
    {
        bool isStraight = true;
        List<Card> cardListTmp = new List<Card>(cards);

        if(cardListTmp.Count < 5)
        {
            isStraight = false;
            return isStraight;
        }
            
        // NOTE: Regular straight check
        cardListTmp.Sort((a, b) => b.rank.CompareTo(a.rank));
        HashSet<Card> cardList = new HashSet<Card>();

        // NOTE: Check if there are card sequence
        for(int i = 1; i < cardListTmp.Count; i++)
        {
            if(cardListTmp[i].rank == cardListTmp[i - 1].rank - 1)
            {
                cardList.Add(cardListTmp[i - 1]);
                cardList.Add(cardListTmp[i]);
            }
        }
        
        // NOTE: If we got 5 cards sequential then we got a straight
        isStraight = cardList.Count == 5;

        // NOTE: Final check for the correct card sequences
        for(int i = 1; i < cardList.Count; i++)
        {
            if(cardList.ElementAt(i - 1).rank != cardList.ElementAt(i).rank + 1)
            {
                isStraight = false;
            }
        }

        // NOTE: Check for Ace High Straight
        if(cardListTmp.Contains(14) && cardListTmp.Contains(10) && cardListTmp.Contains(11) 
            && cardListTmp.Contains(12) && cardListTmp.Contains(13) && !HasFlushCards(cardListTmp))
        {
            isStraight = true;
        }

        return isStraight;
    }

    public static bool HasFlushCards(this List<Card> cards)
    {
        return cards.GroupBy(a => a.suit).Where(b => b.Count() == 5).Count() == 1;
    }

    public static bool HasFullHouseCards(this List<Card> cards)
    {
        return cards.GroupBy(a => a.rank).Where(b => b.Count() == 3).Count() == 1
            && cards.GroupBy(a => a.rank).Where(b => b.Count() == 2).Count() == 1;
    }

    public static bool HasFourOfAKindCards(this List<Card> cards)
    {
        return cards.GroupBy(a => a.rank).Where(b => b.Count() == 4).Count() == 1;
    }

    public static bool HasStraightFlushCards(this List<Card> cards)
    {
        return HasFlushCards(cards) && HasStraightCards(cards);
    }

    public static bool HasRoyalFlushCards(this List<Card> cards)
    {
        return cards.Contains(10) && cards.Contains(11) && cards.Contains(12) 
            && cards.Contains(13) && cards.Contains(14) && HasFlushCards(cards);
    }

    public static bool Contains(this List<Card> cards, int rank)
    {
        for(int i = 0; i < cards.Count; i++)
        {
            if(cards[i].rank == rank)
            {
                return true;
            }
        }

        return false;
    }

    public static void Print(this List<Card> card)
    {
        Debug.Log(string.Join("|", card.Select(a => a.rank).ToArray()));
    }

    public static void PrintWithMatchType(this List<Card> card)
    {
        Debug.Log(string.Join("|", card.Select(a => a.rank).ToArray()) + " - " + card.GetCardMatchType());
    }
}
