#if UNITY_EDITOR
using Heat___Complete_Modern_UI.Scripts.Editor_Handlers;
using UnityEditor;
using UnityEngine;

namespace Heat___Complete_Modern_UI.Scripts.Rendering
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GradientFilter))]
    public class GradientFilterEditor : Editor
    {
        private GradientFilter gfTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            gfTarget = (GradientFilter)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var selectedFilter = serializedObject.FindProperty("selectedFilter");
            var opacity = serializedObject.FindProperty("opacity");

            HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 6);
            HeatUIEditorHandler.DrawProperty(selectedFilter, customSkin, "Selected Filter");
            HeatUIEditorHandler.DrawProperty(opacity, customSkin, "Opacity");

            gfTarget.UpdateFilter();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif