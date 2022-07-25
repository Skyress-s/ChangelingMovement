using System;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(StarterAssetsInputs))]
[RequireComponent(typeof(CharacterController))]
public class WizardController : MonoBehaviour
{
    private StarterAssetsInputs _input;
    private CharacterController _controller;

    [Header("Movement")] 
    public float GroundMaxSpeed = 15f;
    public float AirMaxSpeed = 15f;
    public float GroundMoveSharpness = 2f;
    public float AirAcceleration = 2f;
    public float GravityAcceleration = -9.81f;
    
    
    //private
    private Vector3 _groundNormal;
    private Vector3 _velocity;
    private Vector3 _lastImpactSpeed;
    
    [Header("Grounded")] 
    [SerializeField] private bool _grounded;	
    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float GroundCheckDistance = 1f;
    
    
    // jumping landing
    [Header("Jump/Land")] 
    public LayerMask GroundCheckLayers;

    public float JumpForce = 10f;
    
    // private
    private float _LastTimeJumped;
    
    
    
    // private constants
    const float k_JumpGroundingPreventionTime = 0.2f;
    const float k_GroundCheckDistanceInAir = 0.07f;
    
    
    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Jumping();
        GroundedCheck();
        Movement();
    }
   

    private void Jumping()
    {
        if (_input.jump && _grounded)
        {
            _velocity += Vector3.up * JumpForce;
            _LastTimeJumped = Time.time;
        }
    }

    private void Movement()
    {
        float speedModifier = _input.sprint ? GroundMaxSpeed * 1.5f : GroundMaxSpeed;
        Vector3 worldSpaceMoveInput = transform.TransformVector(new Vector3(_input.move.x, 0,_input.move.y));

        if (_grounded)
        {
            // calculate the desired velocity from inputs, max speed, and current slope
            Vector3 targetVelocity = worldSpaceMoveInput * GroundMaxSpeed * speedModifier;
            targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, _groundNormal) *
                             targetVelocity.magnitude;
            _velocity = Vector3.Lerp(_velocity, targetVelocity,
                GroundMoveSharpness * Time.deltaTime);
            
        }
        else
        {
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
                QueryTriggerInteraction.Ignore))
        {
            // We remember the last impact speed because the fall damage logic might need it
            _lastImpactSpeed = _velocity;

            _velocity = Vector3.ProjectOnPlane(_lastImpactSpeed, hit.normal);
        }
        
        
    }

    private void GroundedCheck()
    {
        
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance =
            _grounded ? (_controller.skinWidth + GroundCheckDistance) : k_GroundCheckDistanceInAir;
        
        // reset values before the ground check
        _grounded = false;
        _groundNormal = Vector3.up;
        
        
        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        if (Time.time >= _LastTimeJumped + k_JumpGroundingPreventionTime)
        {
            
            // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(), _controller.radius, Vector3.down,
                    out RaycastHit hit, GroundCheckDistance, GroundCheckLayers, QueryTriggerInteraction.Ignore))
            {
                // storing the upward direction for the surface found
                _groundNormal = hit.normal;
        
                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                    IsNormalUnderSlopeLimit(_groundNormal))
                {
                    _grounded = true;
        
                    // handle snapping to the ground
                    if (hit.distance > _controller.skinWidth)
                    {
                        _controller.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }
    
    // Gets a reoriented direction that is tangent to a given slope
    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }
    // Gets the center point of the bottom hemisphere of the character controller capsule    
    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + _controller.center - transform.up * ( _controller.height/2f - _controller.radius);
        return transform.position + (transform.up * _controller.radius);
    }
    
    // Gets the center point of the top hemisphere of the character controller capsule    
    Vector3 GetCapsuleTopHemisphere()
    {
        return transform.position + _controller.center + transform.up * ( _controller.height/2f - _controller.radius);
        return transform.position + (transform.up * ( - _controller.radius));
    }
    
    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= _controller.slopeLimit;
    }
    
   
}