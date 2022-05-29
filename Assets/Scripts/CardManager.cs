using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Transform hand;
    [SerializeField] private Card cardPrefab;

    private float totalTwist = 20;
    private float positionStep = 30f;
    private bool buffWorkInProgress;
    
    private List<Card> cards = new();

    private void Start()
    {
        var random = new Random();
        var cardsCount = random.Next(4, 7);
        
        for (int i = 0; i < cardsCount; i++) {
            var card = Instantiate(cardPrefab, hand);
            cards.Add(card);
            
            CardTransformInHand(cardsCount, i, card);

            card.OnCardRemoveFromHand += RepositionCards;
        }
    }

    public async void BuffCards()
    {
        if (buffWorkInProgress) {
            return;
        }
        
        buffWorkInProgress = true;
        
        while (cards.Count > 0) {
            for (int i = cards.Count - 1; i >= 0; i--) {
                await cards[i].ChangeAndCheckRandomParameter();
            }
        }

        buffWorkInProgress = false;
    }
    
    private void RepositionCards(Card card)
    {
        var repositionStep = positionStep / 2;
        int indexOfRemovedCard = cards.IndexOf(card);
        cards.Remove(card);

        for (int i = 0; i < cards.Count; i++)
        {
            var cardRectTransform = cards[i].GetComponent<RectTransform>();
            if (i < indexOfRemovedCard) {
                cardRectTransform.DOMoveX(cardRectTransform.position.x - repositionStep, 0.5f);
            } else {
                cardRectTransform.DOMoveX(cardRectTransform.position.x + repositionStep, 0.5f);
            }
        }
    }

    private void CardTransformInHand(int cardsCount, int i, Card card)
    {
        var twistPerCard = totalTwist / cardsCount;
        var startTwist = totalTwist / 2f;
        
        var twistForThisCard = startTwist - i * twistPerCard;
        card.transform.Rotate(0f, 0f, -twistForThisCard);

        var scalingFactor = 0.01f;
        var nudgeThisCard = Mathf.Abs(twistForThisCard);
        nudgeThisCard *= scalingFactor;
        
        card.transform.position -= new Vector3(i * positionStep, nudgeThisCard, 0);
        card.transform.SetAsFirstSibling();
    }

}
