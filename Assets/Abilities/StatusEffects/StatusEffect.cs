using UnityEngine;

public class StatusEffect : ScriptableObject
{
    public float chanceToWearOff = 0.5f;

    public virtual void ApplyTo(Monster target) {
        if (target.status != null) {
            target.status.RemoveFrom(target);
        }
        target.status = this;
    }

    public virtual void RemoveFrom(Monster target) {
        target.status = null;
    }

    public virtual bool TryModifyRolledAbilities(BattleSide side, System.Random dice) {
        return false;
    }

    public virtual bool TryPersistAtEndOfRound(BattleSide side, System.Random dice) {

        if (dice.NextDouble() < chanceToWearOff) {
            RemoveFrom(side.currentMonster);
            return false;
        }

        return true;
    }
}
