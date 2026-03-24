using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace InfinityGame.DesignPattern.ObjectPooling.Editor
{
    public class ObjectPoolWindow : EditorWindow
    {
        private const string ENUM_FILE_PATH = "Assets/InfinityGame/DesignPattern/ObjectPooling/Enum/ItemPoolType.cs";
        private List<string> _enumMembers = new List<string>();
        private string _newName = "";
        private Vector2 _scrollPos;

        [MenuItem("Infinity Game/Object Pool/Manage Item Pool Type")]
        public static void ShowWindow()
        {
            GetWindow<ObjectPoolWindow>("Item Pool Type");
        }

        [MenuItem("Infinity Game/Object Pool/Add Pool Manager")]
        public static void AddPoolManager()
        {
            var existingManager = Object.FindAnyObjectByType<PoolManager>();

            if (existingManager != null) return;

            GameObject go = new GameObject("Pool Manager");
            Undo.RegisterCreatedObjectUndo(go, "Create PoolManager");

            go.AddComponent<PoolManager>();
            go.AddComponent<ObjectPool>();

            Selection.activeGameObject = go;
        }

        private void OnEnable()
        {
            LoadEnum();
        }

        private void OnGUI()
        {
            GUILayout.Label("Manage Item Pool Type Enum", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Adding or removing items type.", MessageType.Info);

            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                _newName = EditorGUILayout.TextField("New Member Name", _newName);
                if (GUILayout.Button("Add", GUILayout.Width(60)))
                {
                    AddMember(_newName);
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("Existing Members:", EditorStyles.label);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, (GUIStyle)"helpBox");

            for (int i = 0; i < _enumMembers.Count; i++)
            {
                string memberName = _enumMembers[i];
                bool isNone = memberName.Equals("None", System.StringComparison.OrdinalIgnoreCase);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (isNone)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.TextField(memberName);
                        GUI.enabled = true;
                        GUILayout.Space(65);
                    }
                    else
                    {
                        string updatedName = EditorGUILayout.DelayedTextField(memberName);
                        if (updatedName != memberName)
                        {
                            _enumMembers[i] = updatedName;
                            SaveEnum();
                            return;
                        }

                        if (GUILayout.Button("Remove", GUILayout.Width(60)))
                        {
                            _enumMembers.RemoveAt(i);
                            SaveEnum();
                            return;
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void LoadEnum()
        {
            _enumMembers.Clear();
            if (!File.Exists(ENUM_FILE_PATH)) return;

            string content = File.ReadAllText(ENUM_FILE_PATH);

            var enumBlockMatch = Regex.Match(content, @"enum\s+ItemPoolType\s*\{([\s\S]*?)\}");
            if (!enumBlockMatch.Success) return;

            string enumBody = enumBlockMatch.Groups[1].Value;

            var matches = Regex.Matches(enumBody, @"(\w+)\s*(?:=\s*\d+)?\s*,?");

            foreach (Match match in matches)
            {
                string name = match.Groups[1].Value;
                if (!string.IsNullOrEmpty(name) && !IsCSharpKeyword(name))
                {
                    _enumMembers.Add(name);
                }
            }
        }

        private bool IsCSharpKeyword(string word)
        {
            string[] keywords =
                { "public", "private", "protected", "internal", "class", "enum", "namespace", "using", "static" };
            return keywords.Contains(word);
        }

        private void AddMember(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            name = Regex.Replace(name, @"\s+", "_");

            if (_enumMembers.Contains(name))
            {
                EditorUtility.DisplayDialog("Error", "Member already exists!", "OK");
                return;
            }

            _enumMembers.Add(name);
            _newName = "";
            SaveEnum();
        }

        private void SaveEnum()
        {
            _enumMembers = _enumMembers.Distinct().ToList();
            if (!_enumMembers.Contains("None")) _enumMembers.Insert(0, "None");

            string content = "namespace InfinityGame.DesignPattern.ObjectPooling\n{\n";
            content += "    public enum ItemPoolType\n    {\n";

            for (int i = 0; i < _enumMembers.Count; i++)
            {
                content += $"        {_enumMembers[i]} = {i},\n";
            }

            content += "    }\n}\n";

            File.WriteAllText(ENUM_FILE_PATH, content);
            AssetDatabase.Refresh();
            Debug.Log("Complete");
        }
    }
}