using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatChangeStatusEffect.asset", menuName ="DataMon/Status Effect/Stat Change")]
public class StatChangeStatusEffect : StatusEffect
{
    public Stat statToChange = Stat.Health;
    public float changeRatio;
    public int changeAbsolute;
    public bool repeatEveryRound = true;
}
