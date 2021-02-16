using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName ="Species.asset", menuName ="DataMon/Species Collection")]
public class SpeciesCollection : ScriptableObject
{
    public List<Species> species = new List<Species>();

    public List<TMP_Dropdown.OptionData> dropDownItems = new List<TMP_Dropdown.OptionData>();

    public void OnEnable() {
        species.RemoveAll(s => s == null);
        species.Sort((a, b) => a.name.CompareTo(b.name));

        dropDownItems.Clear();
        foreach (var monster in species)
            dropDownItems.Add(new TMP_Dropdown.OptionData(monster.name, monster.sprite));
    }
}
