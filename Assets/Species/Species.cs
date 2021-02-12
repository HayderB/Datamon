using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewMonsterSpecies.asset", menuName ="DataMon/Monster Species")]
public class Species : ScriptableObject
{
    public Rarity rarity;
    public Element element;
    public MonsterStats baseStats;

    [System.Serializable]
    public struct AbilitySlot {
#if UNITY_EDITOR
        [HideInInspector] public string name;
#endif
        public Ability ability;
        public float chanceToUse;
    }

    public AbilitySlot[] abilities = new AbilitySlot[2];

#if UNITY_EDITOR
    void Reset() {
        OnValidate();
    }

    public void OnValidate() {
        if (abilities == null) return;
        for(int i = 0; i < abilities.Length; i++) {
            var slot = abilities[i];
            if (slot.ability == null) {
                slot.name = "{empty}";
            } else {
                slot.name = slot.ability.name;
            }
            slot.chanceToUse = Mathf.Clamp01(slot.chanceToUse);
            abilities[i] = slot;
        }
    }
#endif
}
