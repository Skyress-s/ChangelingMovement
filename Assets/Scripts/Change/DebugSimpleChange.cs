using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(StarterAssetsInputs))]
public class DebugSimpleChange : MonoBehaviour
{
    public MonoBehaviour ComponentDisable;
    public MonoBehaviour ComponentEnable;


    void OnDebug()
    {
        ComponentDisable.enabled = false;
        ComponentEnable.enabled = true;
        GetComponent<CharacterController>().Move(Vector3.up * 25f);
    }
}
