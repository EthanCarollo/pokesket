using UnityEngine;

public class DetectRemoteInput : MonoBehaviour
{
    void Update()
    {
        for (int i = 0; i <= 19; i++) // Tu peux augmenter jusqu'à 30
        {
            if (Input.GetKeyDown((KeyCode)(KeyCode.Joystick1Button0 + i)))
            {
                Debug.Log("JoystickButton" + i + " pressed");
            }
        }
    }
}
