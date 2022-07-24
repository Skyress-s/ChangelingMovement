using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private StarterAssetsInputs _input;
    public GameObject CinemachineCameraTarget;
    
    // camera
    [Header("CameraLook")] 
    public float CameraLookSpeed = 100f;

    
    public float BottomClamp = -89f;
    public float TopClamp = 89f;
    private float _cinemachineTargetPitch;
    private float _cinemachineTargetYaw;
    
    
    private const float _threshold = 0.01f;

    
    
    private void LateUpdate()
    {
        CameraRotation();
    }
    
    void CameraRotation()
    {
        if (_input.look.sqrMagnitude >= _threshold)
        {
            _cinemachineTargetPitch += _input.look.y * CameraLookSpeed * Time.deltaTime;
            // _cinemachineTargetYaw += _input.look.x * CameraLookSpeed * Time.deltaTime;
            float _rotationVelocity = _input.look.x * CameraLookSpeed * Time.deltaTime;

            // clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            // CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}