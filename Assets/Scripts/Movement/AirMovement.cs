using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AirMovement
{
    public float AirMaxSpeed = 15f;
    public float AirAcceleration = 2f;
    public float GravityAcceleration = -9.81f;
    
    
    public void Move(Vector2 move, BaseController c) {
        Vector3 worldSpaceMoveInput = c.transform.TransformVector(new Vector3(move.x, 0, move.y));
        //Add air acceleration
        c._velocity += worldSpaceMoveInput * AirAcceleration * Time.deltaTime;
                
        // limit air speed to a maximum, but only horizontally
        float verticalVelocity = c._velocity.y;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(c._velocity, Vector3.up);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, AirMaxSpeed);
        c._velocity = horizontalVelocity + (Vector3.up * verticalVelocity);
                
        // apply the gravity to the velocity
        c._velocity += Vector3.up * GravityAcceleration * Time.deltaTime;
        
        c._controller.Move(c._velocity * Time.deltaTime);
    }
}