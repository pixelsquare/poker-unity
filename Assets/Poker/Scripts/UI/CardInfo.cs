
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using TMPro;

public class CardInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_CardValueLabel = null;
    [SerializeField] private Image m_CardSuitImage = null;

    public void Initialize(Card card)
    {
        SetCardValue(card.RankString);
        SetCardSuitSprite(card.SuitIcon);
        SetCardSuitColor(card.SuitColor);
    }

    public void Initialize(int cardValue, CardType cardSuit)
    {
        SetCardValue(Card.GetRankString(cardValue));
        SetCardSuitSprite(Card.GetSuitIcon(cardSuit));
        SetCardSuitColor(Card.GetSuitColor(cardSuit));
    }

    public void SetCardValue(int cardValue)
    {
        SetCardValue(cardValue.ToString());
    }

    public void SetCardValue(string label)
    {
        if(m_CardValueLabel != null)
        {
            m_CardValueLabel.text = label;
        }
    }

    public void SetCardSuitColor(Color color)
    {
        if(m_CardValueLabel != null)
        {
            m_CardValueLabel.color = color;
        }
    }

    public void SetCardSuitSprite(Sprite suitSpr)
    {
        if(m_CardSuitImage != null)
        {
            m_CardSuitImage.overrideSprite = suitSpr;
        }
    }
}
