using UnityEngine;

[ExecuteInEditMode]
public abstract class MassiveBehaviour<TM, T>: MonoBehaviour
    where TM: MassiveRegistry<TM, T>
    where T: MassiveBehaviour<TM, T>
{
    void OnEnable() => MassiveRegistry<TM, T>.Instance.register((T)this);
}