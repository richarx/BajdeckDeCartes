using System;
using System.Collections;

[Serializable]
public abstract class AEnnemyAction
{
    public abstract IEnumerator ExecuteRoutine();
}
