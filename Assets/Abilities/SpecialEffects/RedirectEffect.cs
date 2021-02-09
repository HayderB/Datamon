using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRedirectEffect.asset", menuName = "DataMon/Special Effect/Redirect")]
public class RedirectEffect : SpecialEffect
{
    public enum Behaviour {
        Normal,
        Block,
        Reflect
    }

    public Behaviour incomingDamage;
    public Behaviour incomingEffects;    
}