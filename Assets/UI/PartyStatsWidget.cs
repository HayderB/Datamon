using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyStatsWidget : ListWidget<PartyStatsWidget>
{
    [System.Serializable]
    public struct SpeciesInfo {
        public Image image;
        public TextMeshProUGUI name;
    }

    public SpeciesInfo[] party;

    public TextMeshProUGUI rank;
    public TextMeshProUGUI averageAdvantage;
    public TextMeshProUGUI minAdvantage;
    public TextMeshProUGUI maxAdvantage;
    public TextMeshProUGUI preyCount;
    public TextMeshProUGUI fairCount;
    public TextMeshProUGUI predatorCount;

    public void Setup(int rank, List<Species> species, PartyEvaluator.PartyIds ids, PartyEvaluator.PartyAdvantageStats stats) {
        PopulatePartyMember(0, species[ids.firstId]);
        PopulatePartyMember(1, species[ids.secondId]);
        PopulatePartyMember(2, species[ids.thirdId]);

        this.rank.text = $"#{rank}";
        averageAdvantage.text = $"Avg. {stats.average}";
        minAdvantage.text = $"Min {stats.min}";
        maxAdvantage.text = $"Max {stats.max}";
        preyCount.text = $"Beats {stats.preyCount}";
        fairCount.text = $"On Par {stats.fairCount}";
        predatorCount.text = $"Beaten By {stats.predatorCount}";
    }

    void PopulatePartyMember(int index, Species species) {
        party[index].name.text = species.name;
        party[index].image.sprite = species.sprite;
    }
}

[System.Serializable]
public class PartyStatsList : WidgetList<PartyStatsWidget> { }
