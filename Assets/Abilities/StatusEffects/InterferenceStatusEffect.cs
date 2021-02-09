using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInterferenceStatusEffect.asset", menuName = "DataMon/Status Effect/Interference")]
public class InterferenceStatusEffect : StatusEffect
{
    public float chanceOfNoAction;
    public float chanceToAttackSelf;
}
