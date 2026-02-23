
using System.Collections;
using Unity.VisualScripting;

public class TrainingDummy : BossCombatEntity
{

    protected void Start()
    {
        lookingAtPlayer = false;
        base.Start();
    }
    protected override IEnumerator BossRotation()
    {
        yield return null;
    }
}
