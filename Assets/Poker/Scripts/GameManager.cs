
using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;

using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject m_CardUIRef = null;

    [SerializeField] private Transform[] m_PlayerParent = null;
    [SerializeField] private Transform m_CommunityCardsParent = null;

    [SerializeField] private TextMeshProUGUI m_Player1CardMatchText = null;
    [SerializeField] private TextMeshProUGUI m_Player2CardMatchText = null;

    [SerializeField] private TextMeshProUGUI m_Player1BannerText = null;
    [SerializeField] private TextMeshProUGUI m_Player2BannerText = null;

    [SerializeField] private GameObject m_PlayerOneWinBanner = null;
    [SerializeField] private GameObject m_PlayerTwoWinBanner = null;

    private CardInfo[] m_CommunityCardUI = new CardInfo[COMMUNITY_MAX_CARDS];
    private List<CardInfo[]> m_PlayerCardUI = new List<CardInfo[]>()
    {
        new CardInfo[PLAYER_MAX_CARDS],
        new CardInfo[PLAYER_MAX_CARDS]
    };

    private List<Player> m_Players = new List<Player>();
    private Card[] m_CommunityCards = new Card[COMMUNITY_MAX_CARDS];

    private const int PLAYER_MAX_COUNT = 2;
    private const int PLAYER_MAX_CARDS = 2;
    private const int COMMUNITY_MAX_CARDS = 5;

    private const string PLAYER_WIN_BANNER_FORMAT = "Player {0}\nWins!\n({1})";

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        SetPlayerOneWinBannerActive(false);
        SetPlayerTwoWinBannerActive(false);
        
        DeckManager.Instance.Initialize();
        GameManager.Instance.Initialize();

        List<Card> communityCards = new List<Card>();
        communityCards.Add(DeckManager.Instance.GetCard(11, CardType.SPADES));
        communityCards.Add(DeckManager.Instance.GetCard(8, CardType.DIAMOND));
        communityCards.Add(DeckManager.Instance.GetCard(11, CardType.DIAMOND));
        communityCards.Add(DeckManager.Instance.GetCard(12, CardType.CLUBS));
        communityCards.Add(DeckManager.Instance.GetCard(5, CardType.SPADES));

        List<Card> p1Cards = new List<Card>();
        p1Cards.Add(DeckManager.Instance.GetCard(6, CardType.SPADES));
        p1Cards.Add(DeckManager.Instance.GetCard(7, CardType.SPADES));
        
        List<Card> p2Cards = new List<Card>();
        p2Cards.Add(DeckManager.Instance.GetCard(7, CardType.CLUBS));
        p2Cards.Add(DeckManager.Instance.GetCard(3, CardType.CLUBS));

        m_Players[0].cards.Clear();
        m_Players[0].cards.AddRange(p1Cards);

        m_Players[1].cards.Clear();
        m_Players[1].cards.AddRange(p2Cards);

        m_CommunityCards = communityCards.ToArray();

        SetUIDirty();
        EvaluateMatch();

        // p1Cards.AddRange(communityCards);
        // p2Cards.AddRange(communityCards);

        // p1Cards.PrintWithMatchType();
        // p2Cards.PrintWithMatchType();

        // Debug.Log(MatchInterpreter.EvaluateCardMatch(p1Cards, p2Cards, communityCards));

        // p1Cards.PrintWithMatchType();
        // p2Cards.PrintWithMatchType();
    }

    public void Initialize()
    {
        for(int i = 0; i < PLAYER_MAX_COUNT; i++)
        {
            Player player = new Player();
            player.cards = new List<Card>();
            
            for(int j = 0; j < PLAYER_MAX_CARDS; j++)
            {
                player.cards.Add(DeckManager.Instance.DrawCard());
                CardInfo cardUI = CreateCardUIObject(m_PlayerParent[i]);
                cardUI.Initialize(player.cards[j]);
                m_PlayerCardUI[i][j] = cardUI;
            }

            m_Players.Add(player);
        }

        for(int i = 0; i < COMMUNITY_MAX_CARDS; i++)
        {
            Card card = DeckManager.Instance.DrawCard();
            CardInfo cardUI = CreateCardUIObject(m_CommunityCardsParent);
            cardUI.Initialize(card);

            m_CommunityCardUI[i] = cardUI;
            m_CommunityCards[i] = card;
        }

        EvaluateMatch();
    }

    public void Restart()
    {
        // NOTE: This will generate new sets of card
        DeckManager.Instance.ShuffleDeck();

        for(int i = 0; i < m_Players.Count; i++)
        {   
            m_Players[i].cards.Clear();

            for(int j = 0; j < PLAYER_MAX_CARDS; j++)
            {
                m_Players[i].cards.Add(DeckManager.Instance.DrawCard());
            }
        }

        for(int i = 0; i < COMMUNITY_MAX_CARDS; i++)
        {
            m_CommunityCards[i] = DeckManager.Instance.DrawCard();
        }

        SetUIDirty();
        EvaluateMatch();
    }

    public void EvaluateMatch()
    {
        int idx = MatchInterpreter.EvaluateCardMatch(m_Players[0].cards, m_Players[1].cards, m_CommunityCards.ToList());

        m_Players[0].cards.AddRange(m_CommunityCards);
        m_Players[1].cards.AddRange(m_CommunityCards);

        SetPlayerOneWinBannerActive(idx <= 0);
        SetPlayerTwoWinBannerActive(idx >= 0);

        SetPlayer1BannerText(idx != 0 ? "Player 1\nWins!" : "DRAW");
        SetPlayer2BannerText(idx != 0 ? "Player 2\nWins!" : "DRAW");

        SetPlayer1CardMatchText(string.Format("({0})", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m_Players[0].cards.GetCardMatchType().ToString().Replace("_", " "))));
        SetPlayer2CardMatchText(string.Format("({0})", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m_Players[1].cards.GetCardMatchType().ToString().Replace("_", " " ))));
    }

    public void SetUIDirty()
    {
        for(int i = 0; i < m_Players.Count; i++)
        {   
            for(int j = 0; j < PLAYER_MAX_CARDS; j++)
            {
                m_PlayerCardUI[i][j].Initialize(m_Players[i].cards[j]);
            }
        }

        for(int i = 0; i < COMMUNITY_MAX_CARDS; i++)
        {
            m_CommunityCardUI[i].Initialize(m_CommunityCards[i]);
        }
    }

    public CardInfo CreateCardUIObject(Transform parent)
    {
        return Instantiate(m_CardUIRef, parent).GetComponent<CardInfo>();
    }

    public void SetPlayerOneWinBannerActive(bool active)
    {
        if(m_PlayerOneWinBanner != null)
        {
            m_PlayerOneWinBanner.SetActive(active);
        }
    }

    public void SetPlayerTwoWinBannerActive(bool active)
    {
        if(m_PlayerTwoWinBanner != null)
        {
            m_PlayerTwoWinBanner.SetActive(active);
        }
    }

    public void SetPlayer1BannerText(string message)
    {
        if(m_Player1BannerText != null)
        {
            m_Player1BannerText.text = message;
        }
    }

    public void SetPlayer2BannerText(string message)
    {
        if(m_Player2BannerText != null)
        {
            m_Player2BannerText.text = message;
        }
    }

    public void SetPlayer1CardMatchText(string message)
    {
        if(m_Player1CardMatchText != null)
        {
            m_Player1CardMatchText.text = message;
        }
    }

    public void SetPlayer2CardMatchText(string message)
    {
        if(m_Player2CardMatchText != null)
        {
            m_Player2CardMatchText.text = message;
        }
    }
}
