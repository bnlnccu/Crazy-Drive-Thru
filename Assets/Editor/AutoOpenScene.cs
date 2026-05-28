using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class AutoOpenScene
{
    static AutoOpenScene()
    {
        EditorApplication.delayCall += () =>
        {
            var scene = SceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(scene.path))
            {
                string targetScene = "Assets/Scenes/DriveThru.unity";
                if (System.IO.File.Exists(targetScene))
                {
                    EditorSceneManager.OpenScene(targetScene);
                }
            }
        };
    }
}
