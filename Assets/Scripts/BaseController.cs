using System;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(StarterAssetsInputs))]
[RequireComponent(typeof(ChangelingManager))]

//base controller with pre implemented needed references and helper functions related to ground checking
public class BaseController : MonoBehaviour
{
    // references
    private CharacterController _controller;
    
    [Header("Base Controller")]
    [SerializeField] private float GroundCheckDistance = 0.05f;
    [SerializeField] private float k_JumpGroundingPreventionTime = 0.2f;
    [SerializeField] private LayerMask GroundCheckLayers;
    
    
    // private
    private bool _grounded;
    private Vector3 _groundNormal;
    private float _LastTimeJumped;


    /// <summary>
    /// will be called by changeling manager
    /// </summary>
    public virtual void OnJumpInput() {
        
    }

    /// <summary>
    /// will be called by ChanglingManager
    /// </summary>
    /// <param name="move"></param>
    public virtual void OnMoveInput(Vector2 move) {
        
    }
    
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }


    public bool GroundedCheck()
    {
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

                    return true;
                }
            }
        }

        return false;
    }
    
    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= _controller.slopeLimit;
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
}