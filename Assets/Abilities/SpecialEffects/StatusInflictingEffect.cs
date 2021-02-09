using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewStatusInflictingEffect.asset", menuName ="DataMon/Special Effect/Inflict Status")]
public class StatusInflictingEffect : SpecialEffect
{
    public StatusEffect effectToInflict;
    public bool applyToSelf;
}
