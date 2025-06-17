using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class LoadFirstScene
{
    static LoadFirstScene()
    {
        EditorApplication.playModeStateChanged += (value) =>
        {
            if (value == PlayModeStateChange.ExitingEditMode)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }

            if (value == PlayModeStateChange.EnteredPlayMode)
            {
                if (EditorSceneManager.GetActiveScene().buildIndex != 0)
                {
                    EditorSceneManager.LoadScene(0);
                }
            }
        };
    }
}
