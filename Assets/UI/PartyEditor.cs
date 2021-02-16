using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PartyEditor : MonoBehaviour
{
    public SpeciesCollection species;

    public SpeciesWidget widgetPrefab;
    public SpeciesWidgetList list;

    [System.NonSerialized]
    List<Species> _editTarget;

    [System.Serializable]
    public class TeamEvent : UnityEvent<PartyEditor> { }
    public TeamEvent OnPartyChange;

    public void LinkEditableList(List<Species> editable) {
        _editTarget = editable;

        list.OnDelete = HandleRemoval;
        list.OnReorder = HandleReorder;
    }

    public void Add() {
        _editTarget.Add(null);

        var widget = list.InstantiateInto(widgetPrefab);
        widget.Setup(HandleSpeciesChange, species);

        // The widget will choose a species and invoke HandleSpeciesChange,
        // which then triggers our OnPartyChange event.
    }

    void HandleRemoval(SpeciesWidget removed, int formerIndex) {
        _editTarget.RemoveAt(formerIndex);

        OnPartyChange?.Invoke(this);
    }

    void HandleReorder(SpeciesWidget widget, int currentIndex, int newIndex) {
        var entry = _editTarget[currentIndex];
        _editTarget.RemoveAt(currentIndex);
        _editTarget.Insert(newIndex, entry);

        OnPartyChange?.Invoke(this);
    }

    void HandleSpeciesChange(SpeciesWidget widget, int newSelectionIndex) {
        int index = list.Widgets.IndexOf(widget);
        _editTarget[index] = species.species[newSelectionIndex];

        OnPartyChange?.Invoke(this);
    }

    public void UpdateHealth(BattleSide source) {
        for(int i = 0; i < list.Widgets.Count; i++) {
            list.Widgets[i].SetHealth(source.GetHealthRatio(i));
        }
    }
}
