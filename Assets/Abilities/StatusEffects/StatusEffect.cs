using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : ScriptableObject
{
    public float chanceToWearOff = 0.5f;

    public virtual void ApplyTo(Monster target) {
        if (target.status != null) {
            target.status.RemoveFrom(target);
        }
        target.status = this;
    }

    public virtual void RemoveFrom(Monster target) {
        target.status = null;
    }
}
