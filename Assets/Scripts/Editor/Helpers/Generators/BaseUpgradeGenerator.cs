using System.IO;
using GameFlow.Upgrade.Base;
using UnityEditor;
using UnityEngine;

namespace Editor.Helpers.Generators
{
    public class BaseUpgradeGenerator : EditorWindow
    {
        private string _upgradeName = "New Upgrade";
        private string _savePath = "Assets/Data/UpgradeData"; // Default path
        private UpgradeType _upgradeType = UpgradeType.Drone;
        private Vector2 _scrollPosition;

        [MenuItem("Tools/Upgrade Generator")]
        public static void ShowWindow()
        {
            GetWindow<BaseUpgradeGenerator>("Upgrade Generator");
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GUILayout.Label("Upgrade Data Generator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use this tool to quickly create new UpgradeData assets.", MessageType.Info);

            // --- Section for Creating Upgrades ---
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("New Upgrade Details", EditorStyles.miniBoldLabel);

            _upgradeName = EditorGUILayout.TextField("Upgrade Name", _upgradeName);
            _upgradeType = (UpgradeType)EditorGUILayout.EnumPopup("Upgrade Type", _upgradeType);

            // --- Section for Save Path ---
            GUILayout.Space(10);
            GUILayout.Label("Save Location", EditorStyles.miniBoldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path", _savePath, EditorStyles.textField);
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                var selectedPath = EditorUtility.OpenFolderPanel("Select Save Folder", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // We need a relative path, not the full system path
                    _savePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // --- The Create Button ---
            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f); // A nice green color
            if (GUILayout.Button("Create Upgrade Asset", GUILayout.Height(30)))
            {
                CreateUpgradeAsset();
            }

            GUI.backgroundColor = Color.white; // Reset color

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        // --- Core Logic ---
        private void CreateUpgradeAsset()
        {
            if (string.IsNullOrWhiteSpace(_upgradeName))
            {
                EditorUtility.DisplayDialog("Error", "Upgrade Name cannot be empty.", "OK");
                return;
            }

            // 1. Ensure the directory exists
            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }

            // 2. Create an instance of the ScriptableObject
            var newUpgrade = CreateInstance<BaseUpgradeData>();

            // 3. Set up the file path and name
            // Sanitize name to prevent file system errors
            var sanitizedName = _upgradeName.Replace(" ", "_");
            var assetPath = Path.Combine(_savePath, $"Upg_{sanitizedName}.asset");
            var uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            // 4. Initialize the asset with default values using our new method
            newUpgrade.Initialize(_upgradeName);
            newUpgrade.upgradeType = _upgradeType; // Set the type from the window

            // 5. Create the .asset file in the project
            AssetDatabase.CreateAsset(newUpgrade, uniqueAssetPath);
            AssetDatabase.SaveAssets();

            // 6. Focus on the newly created asset in the Project window
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newUpgrade;

            Debug.Log($"<color=green>Successfully created upgrade at: {uniqueAssetPath}</color>");
        }
    }
}