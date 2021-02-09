using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbsorbEffect.asset", menuName = "DataMon/Special Effect/Absorb")]
public class AbsorbEffect : SpecialEffect
{
    public enum Source {
        DamageDealt,
        DamageTaken,
        StaminaSpentByOpponent
    }

    public Stat statToBuff;
    public float amountToBuff;
    public Source source;
}
