using UnityEngine;

[CreateAssetMenu(fileName ="NewMonsterSpecies.asset", menuName ="DataMon/Monster Species")]
public class Species : ScriptableObject
{
    public Sprite sprite;

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

        baseStats.RoundAndClamp();

        float totalChance = 0f;
        int nonNullCount = 0;
        for(int i = 0; i < abilities.Length; i++) {
            var slot = abilities[i];
            if (slot.ability == null) {
                slot.name = "{empty}";
            } else {
                slot.name = slot.ability.name;
                nonNullCount++;
            }
            slot.chanceToUse = Mathf.Clamp01(slot.chanceToUse);            
            abilities[i] = slot;
            totalChance += slot.chanceToUse;
        }

        if (!Mathf.Approximately(totalChance, 1f)) {
            if (totalChance == 0f) {
                if (nonNullCount == 0) {
                    Debug.LogError($"Monster species {name} has no valid abilities set.");
                } else {
                    for (int i = 0; i < abilities.Length; i++) {
                        var slot = abilities[i];
                        slot.chanceToUse = slot.ability == null ? 0f : 1f / nonNullCount;
                        abilities[i] = slot;
                    }
                }
            } else {
                for (int i = 0; i < abilities.Length; i++) {
                    var slot = abilities[i];
                    slot.chanceToUse = slot.chanceToUse / totalChance;
                    abilities[i] = slot;
                }
            }
        }
    }
#endif
}
