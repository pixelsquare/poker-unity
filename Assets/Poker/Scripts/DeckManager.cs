
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    private List<Card> m_CardDeck = new List<Card>();
    private Stack<int> m_CardDeckIdx = new Stack<int>();
    
    private const int SUIT_MAX_COUNT = 14;

    public void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        string[] suitNames = Enum.GetNames(typeof(CardType));

        foreach(string suitName in suitNames)
        {
            for(int i = 2; i <= SUIT_MAX_COUNT; i++)
            {
                m_CardDeck.Add(new Card()
                {
                    rank = i,
                    suit = (CardType)Enum.Parse(typeof(CardType), suitName, true)
                });
            }
        }
    }

    public Card GetCard(int rank, CardType suit)
    {
        return m_CardDeck.Find(a => a.rank == rank && a.suit == suit);
    }

    public void ShuffleDeck()
    {
        List<int> indices = new List<int>();

        for(int i = 0; i < m_CardDeck.Count; i++)
        {
            indices.Add(i);
        }

        System.Random rand = new System.Random();

        while(indices.Count > 0)
        {
            int idx = rand.Next(0, indices.Count);
            m_CardDeckIdx.Push(indices[idx]);
            indices.RemoveAt(idx);
        }
    }

    public Card DrawCard()
    {
        int randIdx = GetRandomCardIndex();
        return m_CardDeck[randIdx];
    }

    private int GetRandomCardIndex()
    {
        if(m_CardDeckIdx.Count <= 0)
        {
            ShuffleDeck();
        }

        return m_CardDeckIdx.Pop();
    }
}
