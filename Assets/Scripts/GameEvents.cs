using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour {
    public static GameEvents instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this);
        }
    }

    public UnityEvent<ChangelingManager.EEntity> OnChangeEntity;

}