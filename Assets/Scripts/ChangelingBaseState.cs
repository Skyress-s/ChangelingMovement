using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChangelingBaseState
{
    public abstract void Enter(ref ChangelingManager manager);

    public abstract void Update(ref ChangelingManager manager);

    public abstract void Exit(ref ChangelingManager manager);
}
