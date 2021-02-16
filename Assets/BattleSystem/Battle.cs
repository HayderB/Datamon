[System.Serializable]
public class Battle {

    public enum Team : int {
        None = -1,
        Player = 0,
        Opponent = 1,
        Tie = 2
    }

    public enum Outcome {
        BattleContinues,
        MonsterDefeated,
        PartyDefeated
    }

    struct ModifierEffect {
        public BattleSide side;
        public bool self;

        public ModifierEffect(BattleSide side, bool self) {
            this.side = side;
            this.self = self;
        }
    }

    public int roundCount = -1;
    public Outcome outcome = Outcome.PartyDefeated;
    public BattleSide player = new BattleSide();
    public BattleSide opponent = new BattleSide();

    // Group our two sides into an array for ease of access.
    [System.NonSerialized]
    BattleSide[] _sides;

    public bool Start() {
        if (_sides == null || _sides.Length == 0)
            Initialize();

        roundCount = 0;
        outcome = Outcome.BattleContinues;

        foreach (var side in _sides)
            if (side.ReadyToBattle() == false)
                return false;

        return true;
    }

    public void Initialize() {
        _sides = new BattleSide[2];

        _sides[(int)Team.Player] = player;
        player.team = Team.Player;

        _sides[(int)Team.Opponent] = opponent;
        opponent.team = Team.Opponent;

        player.opponent = opponent;
        opponent.opponent = player;
    }

    public Team EvaluateMatch(System.Random dice, int roundLimit = 100) {
        Team winner = Team.None;        
        
        while(roundCount < roundLimit) {
            winner = EvaluateRound(dice);
            if (winner != Team.None)
                break;
        }

        return winner;
    }

    public Team EvaluateRound(System.Random dice) {
        roundCount++;

        // Roll basic damage, special effect success, and simple effects like miss/critical.
        foreach (var side in _sides)
            side.RollAbility(dice);

        // Apply status effects like sleep/confusion that can block abilites.
        foreach (var side in _sides) {
            if (side.currentMonster.status != null) 
                side.currentMonster.status.TryModifyRolledAbilities(side, dice);
        }

        // Apply modifier special effects like block/dodge/counter/copycat
        // that can change what one monster or the other is doing.
        HandleModifierEffects(dice);

        // End of round. Evaluate what happened.
        var winner = Team.Tie;
        outcome = Outcome.BattleContinues;

        foreach (var side in _sides) {
            // Apply all outcomes of the battle round.
            // Including abilities whose effects depend on damage dealt/taken,
            // abilities that inflict or remove status effects,
            // and status effect end-of-round effects.            
            var newOutcome = side.ApplyIncomingImpactsAndStatus(dice);

            if (newOutcome != Outcome.PartyDefeated)
                winner = side.team;

            if (newOutcome > outcome) outcome = newOutcome;
        }

        if (outcome != Outcome.PartyDefeated)
            winner = Team.None;

        return winner;
    }

    void HandleModifierEffects(System.Random dice) {
        // Effects that can modify the opponent's effect, or depend on the opponent's effect,
        // need to be applied in priority order for consistent stacking.
        // This is ugly and could probably be improved.
        while (true) {
            // Search the four effect slots for any special effects that need this treatment.
            // Remember the one that comes first in priority order.

            // We have to do this search multiple times, because effects can change
            // slots that we already looked at.
            float firstPriority = float.PositiveInfinity;
            ModifierEffect nextEffect = default;
            foreach (var side in _sides) {
                var mutual = side.effectOnOpponent.successfulSpecial as MutualModifierEffect;

                if (mutual != null && mutual.priorityOrder < firstPriority) {
                    firstPriority = mutual.priorityOrder;
                    nextEffect = new ModifierEffect(side, false);
                }

                mutual = side.effectOnSelf.successfulSpecial as MutualModifierEffect;
                if (mutual != null && mutual.priorityOrder < firstPriority) {
                    firstPriority = mutual.priorityOrder;
                    nextEffect = new ModifierEffect(side, true);
                }
            }

            // If we didn't find any effects that need this treatment, abort. We're done.
            if (float.IsInfinity(firstPriority)) break;

            // Otherwise, cache the effect, clear it out (so we don't handle it again), and handle it.
            if (nextEffect.self) {
                var mutual = (MutualModifierEffect)nextEffect.side.effectOnSelf.successfulSpecial;
                nextEffect.side.effectOnSelf.successfulSpecial = null;
                mutual.OnPostAbilitiesRolled(nextEffect.side, nextEffect.side.currentMonster, dice);
            } else {
                var mutual = (MutualModifierEffect)nextEffect.side.effectOnOpponent.successfulSpecial;
                nextEffect.side.effectOnOpponent.successfulSpecial = null;
                mutual.OnPostAbilitiesRolled(nextEffect.side, nextEffect.side.opponent.currentMonster, dice);
            }
        }
    }
}
