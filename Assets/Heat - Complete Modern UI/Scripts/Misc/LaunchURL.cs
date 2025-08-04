using UnityEngine;

namespace Heat___Complete_Modern_UI.Scripts.Misc
{
    public class LaunchURL : MonoBehaviour
    {
        public void GoToURL(string URL)
        {
            Application.OpenURL(URL);
        }
    }
}