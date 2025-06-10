using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The main purpose of this script is just to change scene when we click on touch anything
/// </summary>
public class AnyInputToChangeScene : MonoBehaviour
{
    public int indexSceneToLoad = 1;

    void Update()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetJoystickNames().Length > 0 && DetectJoystickInput())
        {
            SceneTransitor.Instance.LoadScene(indexSceneToLoad);
        }
    }

    bool DetectJoystickInput()
    {
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetButtonDown("Submit") || Input.GetButtonDown("Jump");
    }
}