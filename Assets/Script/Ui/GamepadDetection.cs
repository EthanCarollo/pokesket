using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamepadDetection : MonoBehaviour
{
    [Header("UI Feedback")]
    public TextMeshProUGUI statusText;
    
    private int connectedGamepads = 0;
    private string[] gamepadNames;
    
    private void Start()
    {
        gamepadNames = new string[2]; // Maximum 2 joueurs
        DetectGamepads();
    }
    
    private void Update()
    {
        if (Time.time % 1f < Time.deltaTime)
        {
            DetectGamepads();
        }
    }
    
    private void DetectGamepads()
    {
        DetectWithOldInputSystem();
        UpdateUI();
    }
    
    private void DetectWithOldInputSystem()
    {
        connectedGamepads = 0;
        string[] joystickNames = Input.GetJoystickNames();
        
        Debug.Log($"Joysticks détectés : {joystickNames.Length}");
        
        for (int i = 0; i < joystickNames.Length && i < 2; i++)
        {
            if (!string.IsNullOrEmpty(joystickNames[i]))
            {
                connectedGamepads++;
                gamepadNames[i] = joystickNames[i];
                Debug.Log($"Manette {i + 1}: {joystickNames[i]}");
            }
            else
            {
                gamepadNames[i] = "Non connectée";
            }
        }
    }
    
    private void UpdateUI()
    {
        if (statusText != null)
        {
            statusText.text = $"Manettes connectées : {connectedGamepads}";
        }
    }
}