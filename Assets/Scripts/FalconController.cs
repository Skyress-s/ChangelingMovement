using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class FalconController : BaseController
{
    // references
    [SerializeField] private GameObject _cameraTarget;


    [Header("Controller")]
    public float StartHeight;
    
    
    
    
    [Header("Gravity")]
    public float GravityAcceleration = -9.81f;

    public float AirControl = 10f;


    private Vector3 _positionLast;
    


    public override void OnMoveInput(Vector2 move) {
    
        /*float speed = _velocity.magnitude;
        float speedToAdd = Vector3.Dot(_cameraTarget.transform.forward, GravityAcceleration * Vector3.up) * Time.deltaTime;
        speed += speedToAdd;

        _velocity = _cameraTarget.transform.forward * speed;*/

        // add gravity
        _velocity += Vector3.up * GravityAcceleration * Time.deltaTime;
        
        //rotates
        //very basic has to be refined
        _velocity = Vector3.RotateTowards(_velocity, _cameraTarget.transform.forward,  AirControl * Time.deltaTime, 0f);
        
        // add lift
        // _velocity += GetLiftAcceleration() * Time.deltaTime;
        
        
        _controller.Move(_velocity * Time.deltaTime);
        _positionLast = transform.position;
        
        
        SaveTopBottomHemiSphereLocation();
        UpdateGroundedState(0.5f, out RaycastHit hit);

        if (_grounded) { // if we hit the ground, change enitity to falcon
            Debug.Log("player hit ground!");
            GameEvents.instance.OnChangeEntity.Invoke(ChangelingManager.EEntity.Wizard);
        }
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
