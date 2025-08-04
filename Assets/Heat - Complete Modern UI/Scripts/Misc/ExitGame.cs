using UnityEngine;

namespace Heat___Complete_Modern_UI.Scripts.Misc
{
    public class ExitGame : MonoBehaviour
    {
        public void Exit() 
        { 
            Application.Quit();
#if UNITY_EDITOR
            Debug.Log("<b>[Heat UI]</b> Exit function works in builds only.");
#endif
        }
    }
}