#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace InfinityGame.ToolEditor
{
    public class SceneLauncherWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        [MenuItem("Infinity Game/Scene Launcher")]
        public static void ShowWindow()
        {
            GetWindow<SceneLauncherWindow>("Scene Launcher");
        }

        private void OnGUI()
        {
            GUILayout.Label("Scenes in Build Settings", EditorStyles.boldLabel);
            GUILayout.Space(2);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            var scenes = EditorBuildSettings.scenes;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.fontSize = 12;
            buttonStyle.fixedHeight = 25;
            buttonStyle.margin = new RectOffset(5, 5, 5, 5);

            foreach (var scene in scenes)
            {
                if (scene.enabled)
                {
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);

                    if (GUILayout.Button(sceneName, buttonStyle))
                    {
                        OpenScene(scene.path);
                    }
                }
            }

            if (scenes.Length == 0)
            {
                GUILayout.Label("No scenes found in Build Settings.");
                if (GUILayout.Button("Open Build Settings"))
                {
                    EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                }
            }

            GUILayout.EndScrollView();
        }

        private void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }
    }
}
#endif