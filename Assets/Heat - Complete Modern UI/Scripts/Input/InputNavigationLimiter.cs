using UnityEngine;
using UnityEngine.EventSystems;

namespace Heat___Complete_Modern_UI.Scripts.Input
{
    [AddComponentMenu("Heat UI/Input/Input Navigation Limiter")]
    public class InputNavigationLimiter : MonoBehaviour
    {
        void Update()
        {
            if (ControllerManager.instance != null && EventSystem.current.currentSelectedGameObject.transform.parent != transform.parent)
            {
                ControllerManager.instance.SelectUIObject(transform.GetChild(0).gameObject);
            }
        }
    }
}