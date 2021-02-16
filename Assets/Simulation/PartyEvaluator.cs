using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using TMPro;

public class PartyEvaluator : MonoBehaviour {
    public enum SortBy {
        MaxAdvantage,
        MinAdvantage,
        AverageAdvantage,
        BeatsCount,
        FairCount,
        BeatenByCount
    }

    

    public struct PartyIds {
        public short firstId;
        public short secondId;
        public short thirdId;
        public PartyIds(int first, int second, int third) {
            firstId = (short)first;
            secondId = (short)second;
            thirdId = (short)third;
        }
    }

    public int threadCount = 4;
    const int MATCHES_PER_PAIR = 50;

    public int maxListSize = 60;
    public SortBy sortBy;
    public bool ascending;

    int Compare(PartyAdvantageStats a, PartyAdvantageStats b) {
        int result = GetSortStat(a).CompareTo(GetSortStat(b));
        return ascending ? result : -result;
    }

    float GetSortStat(PartyAdvantageStats a) {
        switch(sortBy) {
            case SortBy.AverageAdvantage: return a.average;
            case SortBy.MinAdvantage: return a.min;
            case SortBy.MaxAdvantage: return a.max;
            case SortBy.BeatenByCount: return a.predatorCount;
            case SortBy.BeatsCount: return a.preyCount;
            case SortBy.FairCount: return a.fairCount;
            default: return 0;
        }
    }

    public ElementCollection elements;
    public SpeciesCollection species;

    PartyIds[] _allParties;
    List<PartyAdvantageStats> _stats;

    class PartyBattler {
        readonly public Thread thread;
        readonly PartyIds[] allParties;
        readonly List<Species> species;
        readonly short[,,] winLossCounts;
        readonly int start, end;
        public volatile int progress;

        public PartyBattler(PartyIds[] allParties, List<Species> species, short[,,] winLossCounts, int start, int end) {
            this.allParties = allParties;
            this.species = species;
            this.winLossCounts = winLossCounts;
            this.start = start;
            this.end = end;

            thread = new Thread(Evaluate);
            thread.Start();
        }

        void Evaluate() {
            progress = 0;
            var battle = new Battle();
            var dice = new System.Random();

            for (int i = start; i < end; i++) {
                PrepParty(battle.player.party, i);

                for (int j = 0; j < allParties.Length; j++) {
                    if (j == i) continue;
                    PrepParty(battle.opponent.party, j);

                    int playerWins = 0;
                    int opponentWins = 0;
                    for (int m = 0; m < MATCHES_PER_PAIR; m++) {
                        battle.Start();
                        var winner = battle.EvaluateMatch(dice);

                        switch(winner) {
                            case Battle.Team.Player: playerWins++; break;
                            case Battle.Team.Opponent: opponentWins++; break;
                        }
                    }
                    winLossCounts[i, j, 0] = (short)playerWins;
                    winLossCounts[i, j, 1] = (short)opponentWins;
                }
                progress++;
            }
        }

        void PrepParty(List<Species> party, int id) {
            party.Clear();

            var ids = allParties[id];
            party.Add(species[ids.firstId]);
            party.Add(species[ids.secondId]);
            party.Add(species[ids.thirdId]);
        }
    }

    List<PartyBattler> TestAllPartyBattles(out short[,,] winLossCounts) {
        int speciesCount = species.species.Count;
        int partyCount = speciesCount * (speciesCount - 1) * (speciesCount - 2);

        _allParties = new PartyIds[partyCount];

        // Naive loop to build all possible parties. We can do something fancier,
        // but this is simple and clear and fast enough for a few thousand cases.
        int tallied = 0;
        for (int first = 0; first < speciesCount; first++) {
            for (int second = 0; second < speciesCount; second++) {
                if (second == first) continue;
                for (int third = 0; third < speciesCount; third++) {
                    if (third == first || third == second) continue;
                    _allParties[tallied++] = new PartyIds(first, second, third);
                }
            }
        }

        winLossCounts = new short[partyCount, partyCount, 2];

        var battlers = new List<PartyBattler>(threadCount);

        int partiesCovered = 0;
        float partiesPerThread = partyCount / (float)threadCount;
        for(int i = 0; i < threadCount; i++) {
            int next = Mathf.RoundToInt(partiesPerThread * (i + 1));
            battlers.Add(new PartyBattler(
                _allParties, 
                species.species, 
                winLossCounts, 
                partiesCovered, 
                next));
            partiesCovered = next;
        }

        UnityEngine.Assertions.Assert.AreEqual(partyCount, partiesCovered, "Bad loop math! You didn't cover all the parties!");

        return battlers;
    }

    public TextMeshProUGUI statusText;

    float[,] advantages;

    [System.Serializable]
    public struct PartyAdvantageStats {
        public short partyID;

        public short preyCount;
        public short predatorCount;
        public short fairCount;

        public float total;
        public float average;
        public float min;
        public float max;        
    }

    public PartyStatsList list;
    public PartyStatsWidget widgetPrefab;

    private IEnumerator Start() {
        statusText.text = "Initializing...";
        yield return null;

        var battlers = TestAllPartyBattles(out short[,,] winLossCounts);        

        int banked = 0;

        while (battlers.Count > 0) {
            yield return null;
            int progress = banked;
            for(int i = 0; i < battlers.Count; i++) {
                progress += battlers[i].progress;
                if (!battlers[i].thread.IsAlive) {
                    banked += battlers[i].progress;
                    battlers[i].thread.Join();
                    battlers[i] = null;
                }
            }
            battlers.RemoveAll(b => b == null);
            statusText.text = $"Evaluating match ups... {progress}/{_allParties.Length}";
        }

        battlers = null;

        statusText.text = "Computing advantages...";
        yield return null;

        advantages = new float[_allParties.Length, _allParties.Length];

        float denominator = 0.5f / MATCHES_PER_PAIR;

        _stats = new List<PartyAdvantageStats>();

        for (int i = 0; i < _allParties.Length; i++) {
            PartyAdvantageStats stats = default;
            stats.partyID = (short)i;
            float advantage = 0f;
            for (int j = 0; j < _allParties.Length; j++) {
                if (i != j) { 
                    advantage = (  winLossCounts[i, j, 0]
                                 - winLossCounts[i, j, 1]
                                 + winLossCounts[j, i, 1]
                                 - winLossCounts[j, i, 0]) * denominator;
                }
                advantages[i, j] = advantage;

                stats.total += advantage;
                stats.max = Mathf.Max(stats.max, advantage);
                stats.min = Mathf.Min(stats.min, advantage);

                if (Mathf.Approximately(advantage, 0)) {
                    stats.fairCount++;
                } else if (advantage > 0) {
                    stats.preyCount++;
                } else {
                    stats.predatorCount++;
                }
            }
            stats.average = stats.total / _allParties.Length;

            _stats.Add(stats);
        }

        statusText.text = "Tallying stats...";
        yield return null;

        OnValidate();
        statusText.text = "Top sorted parties:";
    }

    private void OnValidate() {
        if (_stats == null || _stats.Count == 0) return;

        _stats.Sort(Compare);

        maxListSize = Mathf.Min(maxListSize, _stats.Count);

        list.Clear();

        for (int i = 0; i < maxListSize; i++) {
            var widget = list.InstantiateInto(widgetPrefab);

            widget.Setup((i + 1), species.species, _allParties[_stats[i].partyID], _stats[i]);
        }
    }
}
