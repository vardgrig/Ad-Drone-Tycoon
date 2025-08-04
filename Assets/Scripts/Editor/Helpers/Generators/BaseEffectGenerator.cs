using System.IO;
using GameFlow.Upgrade.Base;
using UnityEditor;
using UnityEngine;

namespace Editor.Helpers.Generators
{
    public class BaseEffectGenerator : EditorWindow
    {
        private string _effectName = "New Effect";
        private string _savePath = "Assets/Data/EffectData"; // Default path
        private UpgradeEffectType _effectType = UpgradeEffectType.IncreaseDroneSpeed;
        private float _effectValue = 1f; // Default value for the effect
        private int _startingLevel = 1;
        private int _endingLevel = 10;
        private Vector2 _scrollPosition;

        [MenuItem("Tools/Effect Data Generator")]
        public static void ShowWindow()
        {
            GetWindow<BaseEffectGenerator>("Effect Data Generator");
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GUILayout.Label("Effect Data Generator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use this tool to quickly create new EffectData assets.", MessageType.Info);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("New Upgrade Details", EditorStyles.miniBoldLabel);

            _effectName = EditorGUILayout.TextField("Effect Name", _effectName);
            _effectType = (UpgradeEffectType)EditorGUILayout.EnumPopup("Effect Type", _effectType);
            
            _effectValue = EditorGUILayout.FloatField("Effect Value", _effectValue);

            GUILayout.Space(10);
            GUILayout.Label("Level Range", EditorStyles.miniBoldLabel);
            _startingLevel = EditorGUILayout.IntField("Starting Level", _startingLevel);
            _endingLevel = EditorGUILayout.IntField("Ending Level", _endingLevel);

            GUILayout.Space(10);
            GUILayout.Label("Save Location", EditorStyles.miniBoldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path", _savePath, EditorStyles.textField);
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                var selectedPath = EditorUtility.OpenFolderPanel("Select Save Folder", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    _savePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f); // A nice green color
            if (GUILayout.Button("Create Effect Data Assets", GUILayout.Height(30)))
            {
                CreateEffectAsset();
            }

            GUI.backgroundColor = Color.white; // Reset color

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void CreateEffectAsset()
        {
            if (string.IsNullOrWhiteSpace(_effectName))
            {
                EditorUtility.DisplayDialog("Error", "Effect Data Name cannot be empty.", "OK");
                return;
            }

            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }

            for (var i = _startingLevel; i <= _endingLevel; i++)
            {
                var newUpgrade = CreateInstance<BaseEffectData>();

                var sanitizedName = _effectName.Replace(" ", "_");
                var assetPath = Path.Combine(_savePath, $"Eff_{sanitizedName}{i}.asset");
                var uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

                newUpgrade.effectType = _effectType;
                newUpgrade.effectValue = _effectValue;

                AssetDatabase.CreateAsset(newUpgrade, uniqueAssetPath);
                AssetDatabase.SaveAssets();
            }

            EditorUtility.FocusProjectWindow();

            Debug.Log($"<color=green>Successfully created effect data at: {_savePath}</color>");
        }
    }
}