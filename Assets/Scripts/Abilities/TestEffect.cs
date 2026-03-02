using UnityEngine;

public class TestEffect : StatusEffect
{
    protected override void OnApplied()
    {
        Debug.Log("TestEffect Applied");
    }

    protected override void OnRefresh()
    {
        Debug.Log("TestEffect Reapplied");
    }

    protected override void OnTick()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnExpired()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnRemoved()
    {
        throw new System.NotImplementedException();
    }

    protected override float GetTickRate()
    {
        throw new System.NotImplementedException();
    }
}
