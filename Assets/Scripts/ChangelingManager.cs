using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(StarterAssetsInputs))]
public class ChangelingManager : MonoBehaviour
{
    [SerializeField] private WizardController wizardController;
    [SerializeField] private FalconController _falconController;
    private StarterAssetsInputs _input;

    private BaseController _activeController;
    private void Awake() {
        _input = GetComponent<StarterAssetsInputs>();
    }
    
    private void Update() {
        _activeController.OnMoveInput(_input.move);
        if (_input.jump) {
            _activeController.OnJumpInput();
        }
    }
    void OnDebug()
    {
        EnterFalconState();
        GetComponent<CharacterController>().Move(Vector3.up * 25f);
    }
    public void EnterWizardState()
    {
        wizardController.enabled = true;
        _falconController.enabled = false;
        
    }

    public void EnterFalconState()
    {
        wizardController.enabled = false;
        _falconController.enabled = true;
    }

    
}