using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameBaseState
{
    public abstract void EnterState(GameStateManager gameSM);

    public abstract void UpdateState(GameStateManager gameSM);

    public abstract void FixedUpdateState(GameStateManager gameSM);
}
