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

        float lh = Input.GetAxis("Horizontal");
        float lv = Input.GetAxis("Vertical");
        float rh = Input.GetAxis("RightStickHorizontal"); // Peut nécessiter un Input personnalisé
        float rv = Input.GetAxis("RightStickVertical");

        if (Mathf.Abs(lh) > 0.1f || Mathf.Abs(lv) > 0.1f)
            Debug.Log($"Left Stick: ({lh}, {lv})");

        if (Mathf.Abs(rh) > 0.1f || Mathf.Abs(rv) > 0.1f)
            Debug.Log($"Right Stick: ({rh}, {rv})");
    }
}
