[System.Serializable]
public struct BattleImpacts
{
    public MonsterStats statChanges;
    public SpecialEffect successfulSpecial;
    
    public float GetHealthChangeWithVulnerability(BattleSide side, Monster target) {
        if (statChanges.health >= 0)
            return statChanges.health;
            
        float vulnerability = (100f / target.currentStats.defense);
        vulnerability *= ElementCollection.Instance.GetAttackMultiplier(side.selectedAbility.element, target.currentElement);
        return statChanges.health * vulnerability;
    }

    public void ApplyPreResolveEffects(BattleSide side, Monster target, System.Random dice) {

        if (successfulSpecial != null) {
            successfulSpecial.ApplyPreResolveEffects(side, target, dice);
        }

        statChanges.health = GetHealthChangeWithVulnerability(side, target);

        target.currentStats += statChanges;
    }

    public void ApplyPostResolveEffects(BattleSide side, Monster target, System.Random dice) {
        if (successfulSpecial != null) {
            successfulSpecial.ApplyPostResolveEffects(side, target, dice);
        }
    }
}
