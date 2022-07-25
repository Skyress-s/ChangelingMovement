using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FalconController : BaseController
{
    // references
    private CharacterController _controller;
    [SerializeField] private GameObject _cameraTarget;


    [Header("Controller")]
    public float StartHeight;
    public Vector3 _velocity;
    
    private float speed;
    
    
    [Header("Gravity")]
    public float GravityAcceleration = -9.81f;


    private Vector3 _positionLast;
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        
    }


    public override void OnMoveInput(Vector2 move) {
    
        speed += Vector3.Dot(_cameraTarget.transform.forward, GravityAcceleration * Vector3.up) * Time.deltaTime;
        
        _controller.Move(_cameraTarget.transform.forward * speed * Time.deltaTime);
        

        _positionLast = transform.position;
    }

    public override void OnJumpInput() {
        
    }
}
