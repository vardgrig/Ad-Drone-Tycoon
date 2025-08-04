using System;
using Interfaces;
using StarterAssets.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets.FirstPersonController.Scripts
{
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonInteract : MonoBehaviour
    {
        [SerializeField] private float interactionDistance = 3.0f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private Camera mainCamera;
        
        private bool _lookingForInteractable = true;
        
        private StarterAssetsInputs _input;
#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        
        private void Start()
        {
            if (!mainCamera)
            {
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            }
            
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
        }

        private void Update()
        {
            if (_lookingForInteractable)
            {
                CheckInteractableObject();
            }
        }

        private void CheckInteractableObject()
        {
            if (!Physics.Raycast(mainCamera.transform.position,
                    mainCamera.transform.forward,
                    out var hit,
                    interactionDistance, interactableLayer)) return;

            Debug.Log("INTERACTABLE OBJECT FOUND: " + hit.collider.name);
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && _input.interact)
            {
                interactable.Interact();
            }
        }
    }
}