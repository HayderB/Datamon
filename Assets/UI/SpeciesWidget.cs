using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeciesWidget : ListWidget<SpeciesWidget>
{
    public Image image;   

    public TMP_Dropdown speciesField;

    public Image healthBar;

    System.Action<SpeciesWidget, int> OnSpeciesSelected;

    public void SetHealth(float ratio) {
        ratio = Mathf.Clamp01(ratio);

        healthBar.enabled = ratio > 0f;

        healthBar.rectTransform.anchorMax = new Vector2(ratio, 1f);
    }

    public void Setup(System.Action<SpeciesWidget, int> onSelect, SpeciesCollection species, int selection = -1) {
        OnSpeciesSelected = onSelect;

        speciesField.options = species.dropDownItems;

        if (selection < 0 || selection >= species.dropDownItems.Count)
            selection = Random.Range(0, species.dropDownItems.Count);        

        speciesField.SetValueWithoutNotify(selection);
        OnChangeSpecies(selection);
    }

    public void OnChangeSpecies(int value) {
        image.sprite = speciesField.options[value].image;
        OnSpeciesSelected?.Invoke(this, value);
    }
}

[System.Serializable]
public class SpeciesWidgetList : WidgetList<SpeciesWidget> {}