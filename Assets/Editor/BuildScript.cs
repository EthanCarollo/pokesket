using UnityEditor;
using UnityEngine;

public class BuildScript
{
    public static void PerformBuild()
    {
        PlayerSettings.WebGL.template = "PROJECT:Pokesket";
        
        string buildPath = "Build/WebGL";
        string[] scenes = new string[] {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/CharacterSelection.unity",
            "Assets/Scenes/GameScene.unity"
        };

        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.WebGL, BuildOptions.None);
        Debug.Log("Build WebGL terminé dans " + buildPath);
    }
}
