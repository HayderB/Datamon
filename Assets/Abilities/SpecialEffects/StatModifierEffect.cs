using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewStatModifierEffect.asset", menuName = "DataMon/Special Effect/Stat Modifier")]
public class StatModifierEffect : SpecialEffect
{
    public Stat statToModify;
    public float multiplier;
    public bool applyToSelf;
}
