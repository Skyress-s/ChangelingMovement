using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChangelingBaseState
{
    public abstract void Enter(ref ChangelingFSM fsm);

    public abstract void Update(ref ChangelingFSM fsm);

    public abstract void Exit(ref ChangelingFSM fsm);
}
