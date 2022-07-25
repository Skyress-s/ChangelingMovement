using System;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;

public class WizardController : BaseController {

    [Header("Movement")] public float GroundMaxSpeed = 15f;
    public float AirMaxSpeed = 15f;
    public float GroundMoveSharpness = 2f;
    public float AirAcceleration = 2f;
    public float GravityAcceleration = -9.81f;

    
    
    
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

        return;
        // float speedModifier = _input.sprint ? GroundMaxSpeed * 1.5f : GroundMaxSpeed;
        float speedModifier = GroundMaxSpeed;
        Vector3 worldSpaceMoveInput = transform.TransformVector(new Vector3(move.x, 0, move.y));
        if (_grounded) {
            // calculate the desired velocity from inputs, max speed, and current slope
            Vector3 targetVelocity = worldSpaceMoveInput * GroundMaxSpeed * speedModifier;
            targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, _groundNormal) *
                             targetVelocity.magnitude;
            _velocity = Vector3.Lerp(_velocity, targetVelocity,
                GroundMoveSharpness * Time.deltaTime);

        }
        else {
            //Add air acceleration
            _velocity += worldSpaceMoveInput * AirAcceleration * Time.deltaTime;

            // limit air speed to a maximum, but only horizontally
            float verticalVelocity = _velocity.y;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(_velocity, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, AirMaxSpeed * speedModifier);
            _velocity = horizontalVelocity + (Vector3.up * verticalVelocity);

            // apply the gravity to the velocity
            _velocity += Vector3.up * GravityAcceleration * Time.deltaTime;

        }

        // apply the final calculated velocity value as a character movement
        Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere();
        _controller.Move(_velocity * Time.deltaTime);

        // detect obstructions to adjust velocity accordingly
        _lastImpactSpeed = Vector3.zero;
        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, _controller.radius,
                _velocity.normalized, out RaycastHit hit, _velocity.magnitude * Time.deltaTime, -1,
                QueryTriggerInteraction.Ignore)) {
            // We remember the last impact speed because the fall damage logic might need it
            _lastImpactSpeed = _velocity;

            _velocity = Vector3.ProjectOnPlane(_lastImpactSpeed, hit.normal);
        }


    }
    
   
}