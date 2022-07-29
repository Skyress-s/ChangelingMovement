using System;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;

public class WizardController : BaseController {
    [Header("Comps")]
    public GroundMovement GroundMovement;
    public AirMovement AirMovement;
    
    
    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float GroundCheckDistance = 0.05f;

    [HideInInspector] public float k_JumpGroundingPreventionTime = 0.2f;
    public float AirCheckDistance = 0.5f;

    [HideInInspector]
    public Vector2 Move;
    
    //states
    [HideInInspector] public WizardAir wizardAir = new WizardAir();
    [HideInInspector] public WizardGrounded wizardGrounded = new WizardGrounded();

    //FSM
    private WizardBaseState _currentState = new WizardGrounded();

    public void ChangeState(WizardBaseState newState) {
        _currentState.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
    }

    private void Update() {
        _currentState.Update(this);
        
        // Debug.Log("vel : " + _velocity);
    }

    public override void OnJumpInput() {
        if (_grounded) {
            Jump();
            ChangeState(wizardAir);
        }
    }

    public override void OnMoveInput(Vector2 move) {
        Move = move;
    }
    
   
}