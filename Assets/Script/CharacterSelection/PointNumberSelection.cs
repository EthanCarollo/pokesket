using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PointNumberSelection : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI valueText;
    public Button increaseButton;
    public Button decreaseButton;
    public SelectablePokemonPanel selectablePokemonPanel;
    
    [Header("Value Settings")]
    public int minValue = 11;
    public int maxValue = 51;
    public int step = 5;

    private void Start()
    {
        UpdateUI();

        increaseButton.onClick.AddListener(IncreaseValue);
        decreaseButton.onClick.AddListener(DecreaseValue);
    }

    private void IncreaseValue()
    {
        selectablePokemonPanel.maxPoint = Mathf.Min(selectablePokemonPanel.maxPoint + step, maxValue);
        UpdateUI();
    }

    private void DecreaseValue()
    {
        selectablePokemonPanel.maxPoint = Mathf.Max(selectablePokemonPanel.maxPoint - step, minValue);
        UpdateUI();
    }

    private void UpdateUI()
    {
        valueText.text = selectablePokemonPanel.maxPoint.ToString();
    }
}
