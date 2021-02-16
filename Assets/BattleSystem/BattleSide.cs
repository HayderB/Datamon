using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleSide
{
    [HideInInspector]
    public Battle.Team team;

    public List<Species> party = new List<Species>();
    public int partyPosition;

    public Monster currentMonster = new Monster();

    public Ability selectedAbility;
    public int staminaSpent;

    public BattleImpacts effectOnOpponent;
    public BattleImpacts effectOnSelf;

    // Avoid upsetting Unity's serializer with a reference cycle.
    [System.NonSerialized]
    public BattleSide opponent;

    public float GetHealthRatio(int index) {
        if (index < partyPosition) return 0f;

        if (index > partyPosition) return 1f;

        return currentMonster.currentStats.health / party[partyPosition].baseStats.health;
    }

    public bool ReadyToBattle() {
        partyPosition = 0;

        if (party.Count < 1 || party[0] == null)
            return false;
        
        currentMonster.SetSpecies(party[0]);
        return true;
    }

    public void RollAbility(System.Random dice) {
        selectedAbility = currentMonster.SelectAbility(dice);
        staminaSpent = selectedAbility.staminaCost;
        selectedAbility.Roll(this, dice);
    }

    public Battle.Outcome ApplyIncomingImpactsAndStatus(System.Random dice) {
        currentMonster.currentStats.stamina -= staminaSpent;        

        opponent.effectOnOpponent.ApplyPreResolveEffects(opponent, currentMonster, dice);
        effectOnSelf.ApplyPreResolveEffects(this, currentMonster, dice);

        opponent.effectOnOpponent.ApplyPostResolveEffects(opponent, currentMonster, dice);
        effectOnSelf.ApplyPostResolveEffects(this, currentMonster, dice);

        if (currentMonster.status != null)
            currentMonster.status.TryPersistAtEndOfRound(this, dice);

        currentMonster.currentStats.RoundAndClamp();        

        return GetOutcome();
    }

    Battle.Outcome GetOutcome() {
        if (currentMonster.IsDefeated()) {
            if (++partyPosition < party.Count) {
                currentMonster.SetSpecies(party[partyPosition]);
                return Battle.Outcome.MonsterDefeated;
            }
            return Battle.Outcome.PartyDefeated;
        }

        return Battle.Outcome.BattleContinues;
    }
}
