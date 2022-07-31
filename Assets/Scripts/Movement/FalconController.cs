using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class FalconController : BaseController
{
    // references
    [SerializeField] private GameObject _cameraTarget;

    [Header("Gravity")]
    public float GravityAcceleration = -9.81f;

    public float AirControl = 10f;


    public override void OnMoveInput(Vector2 move) {
    
        // gravity
        //-------------------------------
        _velocity += Vector3.up * GravityAcceleration * Time.deltaTime;
        
        // drawing where the force should come frome
        Vector3 newUp = Vector3.Cross(_cameraTarget.transform.right, _velocity ).normalized;
        // if (Vector3.SignedAngle(_velocity, _cameraTarget.transform.forward, _cameraTarget.transform.right) < 0f)
        //     newUp = -newUp;

        float sin = Mathf.Sin( 
            Mathf.Deg2Rad * Vector3.SignedAngle(_velocity, _cameraTarget.transform.forward, -_cameraTarget.transform.right)
            );
        
        Debug.Log(sin);
        Debug.DrawLine(transform.position, transform.position + _velocity * 5f, Color.green);
        Debug.DrawLine(transform.position, transform.position + _cameraTarget.transform.forward * 5, Color.red);
        
        
        // lift 
        //------------------------------
        float force = sin * AirControl * _velocity.magnitude;
        
        //adding lift
        Debug.DrawLine(transform.position, transform.position + _cameraTarget.transform.up * force * 0.05f, Color.white);
        _velocity += _cameraTarget.transform.up * force * Time.deltaTime;
        
        // steering
        //-----------------------------------

        Vector3 xzForward = _velocity;
        xzForward.y = 0f;
        xzForward.Normalize();

        float angle = Vector3.SignedAngle(_cameraTarget.transform.forward, xzForward, Vector3.up);
        
        Vector3 crossRight = Vector3.Cross(_velocity, Vector3.up).normalized;
        
        //adding sterring
        // _velocity += crossRight * angle * Time.deltaTime;
        // _velocity = Vector3.RotateTowards(_velocity, _cameraTarget.transform.forward,  AirControl * Time.deltaTime, 0f);
        
        //handlig physics and state
        UpdateGroundedState(0.5f, out RaycastHit hit);
        if (_grounded) { // if we hit the ground, change enitity to wizard
            Debug.Log("player hit ground!");
            GameEvents.instance.OnChangeEntity.Invoke(ChangelingManager.EEntity.Wizard);
        }
        
        SaveTopBottomHemiSphereLocation();
        _controller.Move(_velocity * Time.deltaTime);
        HandleCollision();
    }
    
    public override void OnJumpInput() {
        
    }

    // Vector3 GetLiftAcceleration() {
    //     Vector3.RotateTowards
    //     
    //     
    //     return adjustedLift;
    // }

    /// <summary>
    /// invertse of the dot product, using the absolute value and normalizes vectores
    /// Example is the vectors arre 90 deg from eachother, will return 0, if they are parralell, will return 1
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    float inverteDot(Vector3 v1, Vector3 v2) {
        float dot = Vector3.Dot(v1.normalized, v2.normalized);
        return 1 - Mathf.Abs(dot);
    }
    
}