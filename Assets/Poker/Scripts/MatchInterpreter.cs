
using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Array = System.Array;

// Reference:
// https://www.pokerstars.com/poker/games/rules/hand-rankings/

public static class MatchInterpreter
{
    public static int EvaluateCardMatch(List<Card> card1, List<Card> card2, List<Card> communityCards)
    {
        List<Card> combinedCards1 = new List<Card>(card1);
        combinedCards1.AddRange(communityCards);

        List<Card> combinedCards2 = new List<Card>(card2);
        combinedCards2.AddRange(communityCards);
        
        combinedCards1.Sort((a, b) => b.rank.CompareTo(a.rank));
        combinedCards2.Sort((a, b) => b.rank.CompareTo(a.rank));

        CardMatchType p1MatchType = combinedCards1.GetCardMatchType();
        CardMatchType p2MatchType = combinedCards2.GetCardMatchType();

        // NOTE: Evaluate DRAW card matches
        if(p1MatchType == p2MatchType)
        {
            CardMatchType matchType = p1MatchType;
            Debug.Log($"DRAW matchType = {matchType}");

            // NOTE: We will not compare for Royal Flush since it is the highest.
            // It automatically results into a draw and will fall under default
            switch(matchType)
            {
                case CardMatchType.ONE_PAIR:
                case CardMatchType.TWO_PAIR:
                case CardMatchType.THREE_OF_A_KIND:
                case CardMatchType.FOUR_OF_A_KIND:
                    return ComparePairCards(card1, card2, communityCards);
                case CardMatchType.FLUSH:
                    return CompareFlushCards(card1, card2, communityCards);
                case CardMatchType.STRAIGHT:
                case CardMatchType.STRAIGHT_FLUSH:
                    return CompareStraightCards(card1, card2, communityCards);
                case CardMatchType.HIGH_CARDS:
                    return CompareHighCards(combinedCards1, combinedCards2);
                default:
                    return 0;
            }
        }

        return -1 * p1MatchType.CompareTo(p2MatchType);
    }

    private static int ComparePairCards(List<Card> card1, List<Card> card2, List<Card> communityCards)
    {
        List<Card> combinedCards1 = new List<Card>(card1);
        combinedCards1.AddRange(communityCards);

        List<Card> combinedCards2 = new List<Card>(card2);
        combinedCards2.AddRange(communityCards);
        
        combinedCards1.Sort((a, b) => b.rank.CompareTo(a.rank));
        combinedCards2.Sort((a, b) => b.rank.CompareTo(a.rank));

        IEnumerable<IGrouping<int, Card>> c1 = combinedCards1.GroupBy(a => a.rank).Where(b => b.Count() == 2);
        IEnumerable<IGrouping<int, Card>> c2 = combinedCards2.GroupBy(a => a.rank).Where(b => b.Count() == 2);

        int c1Count = c1.Count();
        int c2Count = c2.Count();

        // NOTE: Evaluate ONE PAIR card match
        if(combinedCards1.GetCardMatchType() == CardMatchType.ONE_PAIR 
            && combinedCards2.GetCardMatchType() == CardMatchType.ONE_PAIR
            && c1Count >= 1 && c2Count >= 1)
        {
            List<Card> c1List = c1.ElementAt(0).ToList();
            List<Card> c2List = c2.ElementAt(0).ToList();
            
            c1List.Sort((a, b) => b.rank.CompareTo(a.rank));
            c2List.Sort((a, b) => b.rank.CompareTo(a.rank));

            if(c1List[0].rank == c2List[0].rank)
            {
                List<Card> kickerCard1 = combinedCards1.Except(c1List).ToList();
                List<Card> kickerCard2 = combinedCards2.Except(c2List).ToList();

                kickerCard1.Sort((a, b) => b.rank.CompareTo(a.rank));
                kickerCard2.Sort((a, b) => b.rank.CompareTo(a.rank));

                List<Card> bestCard1 = new List<Card>(c1List);
                List<Card> bestCard2 = new List<Card>(c2List);

                bestCard1.Add(kickerCard1[0]);
                bestCard1.Add(kickerCard1[1]);
                bestCard1.Add(kickerCard1[2]);

                bestCard2.Add(kickerCard2[0]);
                bestCard2.Add(kickerCard2[1]);
                bestCard2.Add(kickerCard2[2]);

                return CompareHighCards(bestCard1, bestCard2);
            }

            // NOTE: Compare the pair cards
            return -1 * c1List[0].rank.CompareTo(c2List[0].rank);
        }
        
        // NOTE: Evaluate TWO PAIR card match
        if(combinedCards1.GetCardMatchType() == CardMatchType.TWO_PAIR 
            && combinedCards2.GetCardMatchType() == CardMatchType.TWO_PAIR
            && c1Count >= 2 && c2Count >= 2)
        {
            List<Card> c1List = new List<Card>();
            List<Card> c2List = new List<Card>();

            c1.Select(a => a.ToList()).ToList().ForEach(b => c1List.AddRange(b));
            c2.Select(a => a.ToList()).ToList().ForEach(b => c2List.AddRange(b));

            c1List.Sort((a, b) => b.rank.CompareTo(a.rank));
            c2List.Sort((a, b) => b.rank.CompareTo(a.rank));

            if(c1List[0].rank == c2List[0].rank)
            {
                if(c1List[2].rank == c2List[2].rank)
                {
                    List<Card> kickerCard1 = combinedCards1.Except(c1List).ToList();
                    List<Card> kickerCard2 = combinedCards2.Except(c2List).ToList();

                    kickerCard1.Sort((a, b) => b.rank.CompareTo(a.rank));
                    kickerCard2.Sort((a, b) => b.rank.CompareTo(a.rank));

                    List<Card> bestCard1 = new List<Card>(c1List);
                    List<Card> bestCard2 = new List<Card>(c2List);

                    bestCard1.Add(kickerCard1[0]);
                    bestCard2.Add(kickerCard2[0]);

                    return CompareHighCards(bestCard1, bestCard2);
                }

                // NOTE: Compare the second pair of cards
                return -1 * c1List[2].rank.CompareTo(c2List[2].rank);
            }

            // NOTE: Compare the first pair of cards
            return -1 * c1List[0].rank.CompareTo(c2List[0].rank);
        }

        c1 = combinedCards1.GroupBy(a => a.rank).Where(b => b.Count() == 3);
        c2 = combinedCards2.GroupBy(a => a.rank).Where(b => b.Count() == 3);

        c1Count = c1.Count();
        c2Count = c2.Count();

        // NOTE: Evaluate THREE OF A KIND card match
        if(combinedCards1.GetCardMatchType() == CardMatchType.THREE_OF_A_KIND 
            && combinedCards2.GetCardMatchType() == CardMatchType.THREE_OF_A_KIND
            && c1Count >= 1 && c2Count >= 1)
        {
            List<Card> c1List = c1.ElementAt(0).ToList();
            List<Card> c2List = c2.ElementAt(0).ToList();
            
            c1List.Sort((a, b) => b.rank.CompareTo(a.rank));
            c2List.Sort((a, b) => b.rank.CompareTo(a.rank));

            if(c1List[0].rank == c2List[0].rank)
            {
                List<Card> kickerCard1 = combinedCards1.Except(c1List).ToList();
                List<Card> kickerCard2 = combinedCards2.Except(c2List).ToList();

                kickerCard1.Sort((a, b) => b.rank.CompareTo(a.rank));
                kickerCard2.Sort((a, b) => b.rank.CompareTo(a.rank));

                List<Card> bestCard1 = new List<Card>(c1List);
                List<Card> bestCard2 = new List<Card>(c2List);

                bestCard1.Add(kickerCard1[0]);
                bestCard1.Add(kickerCard1[1]);

                bestCard2.Add(kickerCard2[0]);
                bestCard2.Add(kickerCard2[1]);

                return CompareHighCards(bestCard1, bestCard2);
            }

            // NOTE: Compare the three card matches
            return -1 * c1List[0].rank.CompareTo(c2List[0].rank);
        }

        c1 = combinedCards1.GroupBy(a => a.rank).Where(b => b.Count() == 4);
        c2 = combinedCards2.GroupBy(a => a.rank).Where(b => b.Count() == 4);
        
        c1Count = c1.Count();
        c2Count = c2.Count();

        // NOTE: Evaluate FOUR OF A KIND card match
        if(combinedCards1.GetCardMatchType() == CardMatchType.FOUR_OF_A_KIND 
            && combinedCards2.GetCardMatchType() == CardMatchType.FOUR_OF_A_KIND
            && c1Count >= 1 && c2Count >= 1)
        {
            List<Card> c1List = c1.ElementAt(0).ToList();
            List<Card> c2List = c2.ElementAt(0).ToList();
            
            c1List.Sort((a, b) => b.rank.CompareTo(a.rank));
            c2List.Sort((a, b) => b.rank.CompareTo(a.rank));

            if(c1List[0].rank == c2List[0].rank)
            {
                List<Card> kickerCard1 = combinedCards1.Except(c1List).ToList();
                List<Card> kickerCard2 = combinedCards2.Except(c2List).ToList();

                kickerCard1.Sort((a, b) => b.rank.CompareTo(a.rank));
                kickerCard2.Sort((a, b) => b.rank.CompareTo(a.rank));

                List<Card> bestCard1 = new List<Card>(c1List);
                List<Card> bestCard2 = new List<Card>(c2List);

                bestCard1.Add(kickerCard1[0]);
                bestCard2.Add(kickerCard2[0]);

                return CompareHighCards(bestCard1, bestCard2);
            }

            // NOTE: Compare the four card matches
            return -1 * c1List[0].rank.CompareTo(c2List[0].rank);
        }

        return 0;
    }

    private static int CompareFlushCards(List<Card> card1, List<Card> card2, List<Card> communityCards)
    {
        List<Card> combinedCards1 = new List<Card>(card1);
        combinedCards1.AddRange(communityCards);

        List<Card> combinedCards2 = new List<Card>(card2);
        combinedCards2.AddRange(communityCards);

        List<Card> card1Flush = combinedCards1.GroupBy(a => a.suit).Where(b => b.Count() == 5).First().ToList();
        List<Card> card2Flush = combinedCards2.GroupBy(a => a.suit).Where(b => b.Count() == 5).First().ToList();

        if(card1Flush.Count == 5 && card2Flush.Count == 5)
        {
            return CompareHighCards(card1Flush, card2Flush);
        }

        return 0;
    }

    private static int CompareStraightCards(List<Card> card1, List<Card> card2, List<Card> communityCards)
    {
        List<Card> combinedCards1 = new List<Card>(card1);
        combinedCards1.AddRange(communityCards);

        List<Card> combinedCards2 = new List<Card>(card2);
        combinedCards2.AddRange(communityCards);
        
        combinedCards1.Sort((a, b) => b.rank.CompareTo(a.rank));
        combinedCards2.Sort((a, b) => b.rank.CompareTo(a.rank));
        
        HashSet<Card> card1Sequence = new HashSet<Card>();
        HashSet<Card> card2Sequence = new HashSet<Card>();

        // NOTE: Get all possible card sequences
        for(int i = 1; i < combinedCards1.Count; i++)
        {
            if(combinedCards1[i].rank == combinedCards1[i - 1].rank - 1)
            {
                card1Sequence.Add(combinedCards1[i]);
                card1Sequence.Add(combinedCards1[i - 1]);
            }
        }

        // NOTE: Get all possible card sequences
        for(int i = 1; i < combinedCards2.Count; i++)
        {
            if(combinedCards2[i].rank == combinedCards2[i - 1].rank - 1)
            {
                card2Sequence.Add(combinedCards2[i]);
                card2Sequence.Add(combinedCards2[i - 1]);
            }
        }

        if(card1Sequence.Count == 5 && card2Sequence.Count == 5)
        {
            // NOTE: Compare the kicker card
            if(card1Sequence.First().rank == card2Sequence.First().rank)
            {
                List<Card> card1Kickers = combinedCards1.Except(card1Sequence).ToList();
                List<Card> card2Kickers = combinedCards2.Except(card2Sequence).ToList();
                return -1 * card1Kickers[0].rank.CompareTo(card2Kickers[0].rank);
            }

            // NOTE: Compare the first card
            return -1 * combinedCards1.First().rank.CompareTo(combinedCards2.First().rank);
        }

        return 0;
    }

    private static int CompareHighCards(List<Card> combinedCards1, List<Card> combinedCards2)
    {
        combinedCards1.Sort((a, b) => b.rank.CompareTo(a.rank));
        combinedCards2.Sort((a, b) => b.rank.CompareTo(a.rank)); 

        return CompareCards(combinedCards1, combinedCards2);
    }

    private static int CompareCards(IEnumerable<Card> combinedCards1, IEnumerable<Card> combinedCards2)
    {
        for(int i = 0; i < combinedCards1.Count(); i++)
        {
            if(combinedCards1.ElementAt(i).rank != combinedCards2.ElementAt(i).rank)
            {
                return -1 * combinedCards1.ElementAt(i).rank.CompareTo(combinedCards2.ElementAt(i).rank);
            }
        }

        return 0;
    }
}
