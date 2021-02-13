using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewAbility.asset", menuName ="DataMon/Ability")]
public class Ability : ScriptableObject
{
    public Element element;
    public int staminaCost = 1;
    public int minDamage = 0;
    public int maxDamage = 0;
    public SpecialEffect special;
    public float specialChance = 1f;

    public float GetDamage(System.Random dice) {
        return minDamage + (float)(dice.NextDouble() + dice.NextDouble() + dice.NextDouble()) * (maxDamage - minDamage) / 3.0f;
    }

    public bool GetSpecialSuccess(System.Random dice) {
        return dice.NextDouble() < specialChance;
    }
}
