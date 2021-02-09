using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewDamageMultiplierEffect.asset", menuName ="DataMon/Special Effect/Damage Multiplier")]
public class DamageMultiplierEffect : SpecialEffect
{
    public float damageMultiplierOnFail;
    public float damageMultiplierOnSuccess;
}
