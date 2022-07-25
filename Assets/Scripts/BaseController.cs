using System;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(ChangelingManager))]

//base controller with pre implemented needed references and helper functions related to ground checking
public class BaseController : MonoBehaviour
{
    // references
    [HideInInspector]
    public CharacterController _controller;
    
    [Header("Base Controller")]
    [SerializeField] private LayerMask GroundCheckLayers;
    
    
    public float JumpForce = 15f;
    
    
    // private
    [SerializeField]
    public bool _grounded;
    [HideInInspector]
    public Vector3 _groundNormal;
    [HideInInspector]
    public Vector3 _lastImpactSpeed;

    //protected
    public Vector3 _velocity;
    
    Vector3 capsuleBottomBeforeMove;
    Vector3 capsuleTopBeforeMove;

    
    /// <summary>
    /// will be called by changeling manager
    /// </summary>
    public virtual void OnJumpInput() { }

    /// <summary>
    /// will be called by ChanglingManager
    /// </summary>
    /// <param name="move"></param>
    public virtual void OnMoveInput(Vector2 move) { }
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }
    

    public void Jump() {
        Vector3 vel = _velocity;
        vel.y = 0;
        _velocity = vel + Vector3.up * JumpForce;
    }
    
    public void UpdateGroundedState(float distance, out RaycastHit outHit)
    {
        // reset values before the ground check
        _grounded = false;
        _groundNormal = Vector3.up;
        
        outHit = new RaycastHit();
        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        // if (Time.time >= _LastTimeJumped + k_JumpGroundingPreventionTime) {

            
        // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
        if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(), _controller.radius, Vector3.down,
                out RaycastHit hit, distance, GroundCheckLayers, QueryTriggerInteraction.Ignore))
        {
            // storing the upward direction for the surface found
            _groundNormal = hit.normal;
            
            // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
            // and if the slope angle is lower than the character controller's limit
            if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                IsNormalUnderSlopeLimit(_groundNormal))
            {
                _grounded = true;
                outHit = hit;
            }
        }
        // }

    }

    public void SnapToGround(in RaycastHit hit) {
        // handle snapping to the ground
        if (hit.distance > _controller.skinWidth) {
            _controller.Move(Vector3.down * hit.distance);
        }
    }

    public void SaveTopBottomHemiSphereLocation() {
        capsuleTopBeforeMove = GetCapsuleTopHemisphere();
        capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
    }

    public void HandleCollision() {
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
    
    // Gets a reoriented direction that is tangent to a given slope
    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }
    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    protected bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= _controller.slopeLimit;
    }
        
    // Gets the center point of the bottom hemisphere of the character controller capsule    
    protected Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + _controller.center - transform.up * ( _controller.height/2f - _controller.radius);
        return transform.position + (transform.up * _controller.radius);
    }
            
    // Gets the center point of the top hemisphere of the character controller capsule    
    protected Vector3 GetCapsuleTopHemisphere()
    {
        return transform.position + _controller.center + transform.up * ( _controller.height/2f - _controller.radius);
        return transform.position + (transform.up * ( - _controller.radius));
    }
}