using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class WizardBaseState {
    protected float _stateTime;
    private bool dDebugState = false;

    public virtual void Enter(WizardController controller) {
    }

    public virtual void Update(WizardController c) {
        _stateTime += Time.deltaTime;
        if (dDebugState) {
            Debug.Log(this.GetType());
        }
        
    }

    public virtual void Exit(WizardController controller) {
        _stateTime = 0f;
    }
}

public class WizardGrounded : WizardBaseState {
    public override void Enter(WizardController controller) {
        base.Enter(controller);
    }

    public override void Update(WizardController c) {
        base.Update(c);
        c.UpdateGroundedState(c.GroundCheckDistance, out RaycastHit hit);
        if (!c._grounded) {
            c.ChangeState(c.wizardAir);
            return;
        }
        
        c.SaveTopBottomHemiSphereLocation();
        c.GroundMovement.Move(c.Move, c);
        c.HandleCollision();
        return;
    }

    public override void Exit(WizardController controller) {
        base.Exit(controller);
    }
}

public class WizardAir : WizardBaseState {
    public override void Enter(WizardController controller) {
        base.Enter(controller);
    }

    public override void Update(WizardController c) {
        base.Update(c);
        c.UpdateGroundedState(c.AirCheckDistance, out RaycastHit hit);
        if (c._grounded && _stateTime > c.k_JumpGroundingPreventionTime) {
            c.SnapToGround(hit);
            c.ChangeState(c.wizardGrounded);
            return;
        }
        c.SaveTopBottomHemiSphereLocation();
        c.AirMovement.Move(c.Move, c);
        c.HandleCollision();

        return;
         
        /*Vector3 worldSpaceMoveInput = c.transform.TransformVector(new Vector3(c.Move.x, 0, c.Move.y));
        //Add air acceleration
        c._velocity += worldSpaceMoveInput * c.AirAcceleration * Time.deltaTime;
        
        // limit air speed to a maximum, but only horizontally
        float verticalVelocity = c._velocity.y;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(c._velocity, Vector3.up);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, c.AirMaxSpeed);
        c._velocity = horizontalVelocity + (Vector3.up * verticalVelocity);
        
        // apply the gravity to the velocity
        c._velocity += Vector3.up * c.GravityAcceleration * Time.deltaTime;
        
        if (c._grounded && _stateTime > c.k_JumpGroundingPreventionTime) {
            c.SnapToGround(hit);
            c.ChangeState(c.wizardGrounded);
            return;
        }

        c.SaveTopBottomHemiSphereLocation();
        c._controller.Move(c._velocity * Time.deltaTime);
        c.HandleCollision();*/
    }

    public override void Exit(WizardController controller) {
        base.Exit(controller);
    }
}