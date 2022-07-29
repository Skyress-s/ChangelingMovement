using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GroundMovement
{
    public float GroundMoveSharpness = 2f;
    public float GroundMaxSpeed = 7f;

    /// <summary>
    /// called in update
    /// </summary>
    /// <param name="move"></param>
    public void Move(Vector2 move, BaseController c) {
        //movement
        Vector3 worldSpaceMoveInput = c.transform.TransformVector(new Vector3(move.x, 0, move.y));
        // calculate the desired velocity from inputs, max speed, and current slope
        Vector3 targetVelocity = worldSpaceMoveInput * GroundMaxSpeed;
        targetVelocity = c.GetDirectionReorientedOnSlope(targetVelocity.normalized, c._groundNormal) *
                         targetVelocity.magnitude;
        c._velocity = Vector3.Lerp(c._velocity, targetVelocity,
            GroundMoveSharpness * Time.deltaTime);

        c._controller.Move(c._velocity * Time.deltaTime);
    }
}