using System.Collections.Generic;
using UnityEngine;

namespace Heat___Complete_Modern_UI.Scripts.Input
{
    [CreateAssetMenu(fileName = "New Controller Preset Manager", menuName = "Heat UI/Controller/New Controller Preset Manager")]
    public class ControllerPresetManager : ScriptableObject
    {
        public ControllerPreset keyboardPreset;
        public ControllerPreset xboxPreset;
        public ControllerPreset dualsensePreset;
        public List<ControllerPreset> customPresets = new List<ControllerPreset>();
    }
}