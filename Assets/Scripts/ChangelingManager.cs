using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(StarterAssetsInputs))]
public class ChangelingManager : MonoBehaviour
{
    [SerializeField] private WizardController _wizardController;
    [SerializeField] private FalconController _falconController;
    private StarterAssetsInputs _input;

    private BaseController _activeController;
    private void Start() {
        _input = GetComponent<StarterAssetsInputs>();
        EnterWizardState();
        
        
        // events
        GameEvents.instance.OnChangeEntity.AddListener((entity) => {
            switch (entity) {
                case  EEntity.Wizard:
                    EnterWizardState();
                    break;
                case EEntity.Falcon:
                    EnterFalconState();
                    break;
            }
        });
    }
    
    private void Update() {
        // Debug.Log("current state : " + _activeController.ToString());
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
        _wizardController.enabled = true;
        _wizardController._velocity = _falconController._velocity;
        
        _falconController.enabled = false;
        _activeController = _wizardController;
    }

    public void EnterFalconState()
    {
        _falconController.enabled = true;
        _falconController._velocity = _wizardController._velocity;
        
        _wizardController.enabled = false;
        _activeController = _falconController;
    }



    public enum EEntity {
        Wizard, Falcon
    }
}