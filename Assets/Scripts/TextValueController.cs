using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class TextValueController : MonoBehaviour
{
    [SerializeField] private bool checkParameterForRemoveCard;
    
    private float targetValue;
    private float initialValue;
    
    private Text textComponent;
    private float textValue;
    
    private float animationTime = 1.5f;
    private int valueMinRange = 1;
    private int valueMaxRange = 10;

    public bool CheckParameterForRemoveCard => checkParameterForRemoveCard;

    private void Start()
    {
        textComponent = GetComponent<Text>();
        
        SetupStartTextValue();
    }
    
    public void SetValue(float value)
    {
        initialValue = textValue;
        targetValue = value;
    }

    public float GetAnimationDuration()
    {
        return animationTime;
    }

    private void SetupStartTextValue()
    {
        var random = new Random();
        textValue = random.Next(valueMinRange, valueMaxRange);
        textComponent.text = textValue.ToString();
    }
    
    private void Update()
    {
        if (Mathf.Abs(initialValue - targetValue) < Mathf.Epsilon) {
            return;
        }
        
        if (initialValue < targetValue) {
            textValue += animationTime * Time.deltaTime * (targetValue - initialValue);
            if (textValue >= targetValue) {
                textValue = targetValue;
            }
        } else {
            textValue -= animationTime * Time.deltaTime * (initialValue - targetValue);
            if (textValue <= targetValue) {
                textValue = targetValue;
            }
        }

        textComponent.text = textValue.ToString("0");
    }
}