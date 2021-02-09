using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRestoreEffect.asset", menuName = "DataMon/Special Effect/Restore")]
public class RestoreEffect : SpecialEffect
{
    public Stat[] statsToRestore;
    public bool clearStatusEffect;
}
