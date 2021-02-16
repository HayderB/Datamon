using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    public static Stat[] STATS = new Stat[] { Stat.Health, Stat.Stamina, Stat.Attack, Stat.Defense, Stat.Luck };

    public Species species;

    public Element currentElement;
    public MonsterStats currentStats;

    public StatusEffect status;

    int _minStaminaCost;

    public bool IsDefeated() {
        return currentStats.health <= 0 || currentStats.stamina < _minStaminaCost;
    }

    // Reset monster to a brand new member of its species.
    public void SetSpecies(Species species) {
        this.species = species;
        currentElement = species.element;
        currentStats = species.baseStats;
        _minStaminaCost = int.MaxValue;
        foreach(var slot in species.abilities) {
            if (slot.ability == null) continue;
            _minStaminaCost = Mathf.Min(_minStaminaCost, slot.ability.staminaCost);
        }

        status = null;
    }

    public Ability SelectAbility(System.Random dice) {
        float maxRoll = 0;

        foreach (var slot in species.abilities) {
            if (slot.ability == null || slot.ability.staminaCost > currentStats.stamina)
                continue;

            maxRoll += slot.chanceToUse;
        }

        float roll = (float)(dice.NextDouble() * maxRoll);
        float accumulatedChance = 0f;

        Ability fallback = species.abilities[0].ability;

        foreach(var slot in species.abilities) {
            if (slot.ability == null || slot.ability.staminaCost > currentStats.stamina)
                continue;

            fallback = slot.ability;

            accumulatedChance += slot.chanceToUse;
            if (roll < accumulatedChance) return slot.ability;
        }

        return fallback;        
    }
}
