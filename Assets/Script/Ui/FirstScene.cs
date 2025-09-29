using UnityEngine;

public class FirstScene : MonoBehaviour
{
        public void LaunchScene1Player()
        {
                SceneTransitor.Instance.LoadCharacterSelection1Player();
        }

        public void LaunchScene2Player()
        {
                SceneTransitor.Instance.LoadCharacterSelection2Players();
        }
}