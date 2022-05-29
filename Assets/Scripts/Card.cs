using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = System.Random;

public class Card : MonoBehaviour, IDragHandler, IDropHandler, IBeginDragHandler
{
    public Action<Card> OnCardRemoveFromHand;
    
    [SerializeField] private Image cardImage;
    [SerializeField] private List<TextValueController> cardParameters;

    private Vector3 startPositionForDragging;
    private Quaternion startRotationForDragging;
    private int startSiblingIndexForDragging;
    private PointerEventData pointerEventData;

    private int valueMinRange = -2;
    private int valueMaxRange = 10;
    private int minParameterValue = 1;
    private int dropAreaLayer = 6;
    private float tweenDurationForReturnToHand = 0.5f;
    
    private static readonly int Shine = Animator.StringToHash("Shine");

    private void Start()
    {
        SetupImage();
    }
    
    public async Task ChangeAndCheckRandomParameter()
    {
        var random = new Random();
        var parameterIndex = random.Next(cardParameters.Count);
        var newParameterValue = random.Next(valueMinRange, valueMaxRange);
        cardParameters[parameterIndex].SetValue(newParameterValue);
        await Task.Delay(TimeSpan.FromSeconds(cardParameters[parameterIndex].GetAnimationDuration()));

        if (!cardParameters[parameterIndex]) {
            return;
        }

        if (cardParameters[parameterIndex].CheckParameterForRemoveCard && newParameterValue < minParameterValue) {
            RemoveCard();
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (LayerIsDropArea()) {
            return;
        }
        
        GetComponent<RectTransform>().position = eventData.position;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (LayerIsDropArea()) {
            return;
        }
        
        if (pointerEventData != eventData) {
            return;              
        }

        pointerEventData = null;

        var hit = Physics2D.Raycast(eventData.position, Vector2.down, float.MaxValue, 1 << dropAreaLayer);
        if (hit.collider != null) {
            LayOnDropArea(hit.transform);
        } else {
            ReturnToHand();
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (LayerIsDropArea()) {
            return;
        }
        
        pointerEventData = eventData;
        startPositionForDragging = transform.position;
        startRotationForDragging = transform.rotation;
        startSiblingIndexForDragging = transform.GetSiblingIndex();
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        transform.SetAsLastSibling();
    }
    
    private async void SetupImage()
    {
        var image = await FileDownloader.DownloadImageAsync();
        var sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f));
        cardImage.sprite = sprite;
    }

    private void RemoveCard()
    {
        OnCardRemoveFromHand?.Invoke(this);
        Destroy(gameObject);
    }

    private void ReturnToHand()
    {
        transform.DOMove(startPositionForDragging, tweenDurationForReturnToHand);
        transform.DORotateQuaternion(startRotationForDragging, tweenDurationForReturnToHand);
        transform.SetSiblingIndex(startSiblingIndexForDragging);
    }

    private void LayOnDropArea(Transform dropAreaTransform)
    {
        transform.SetParent(dropAreaTransform);
        GetComponent<Animator>().SetTrigger(Shine);
        transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        OnCardRemoveFromHand?.Invoke(this);
        gameObject.layer = dropAreaLayer;
    }

    private bool LayerIsDropArea() => gameObject.layer == dropAreaLayer;
}