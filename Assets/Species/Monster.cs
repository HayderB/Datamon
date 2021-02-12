using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    public Species species;

    public Element currentElement;
    public MonsterStats currentStats;

    public StatusEffect status;

    // Reset monster to a brand new member of its species.
    public void SetSpecies(Species species) {
        this.species = species;
        currentElement = species.element;
        currentStats = species.baseStats;
        status = null;
    }
}
