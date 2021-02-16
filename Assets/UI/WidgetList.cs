using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WidgetList<T> where T : ListWidget<T> {
    public delegate void ReorderAction(T widget, int oldIndex, int newIndex);

    public ReorderAction OnReorder;
    public System.Action<T, int> OnDelete;

    public RectTransform container;
    public List<T> Widgets = new List<T>();

    public float padding = 5f;

    public void Add(T widget) {
        widget.OnAdd(this);
        Widgets.Add(widget);
    }

    public T InstantiateInto(T prefab) {
        var widget = Object.Instantiate(prefab, container, false);        
        Add(widget);
        SizeContainer();
        return widget;
    }

    public bool Reorder(T widget, int shift) {
        int currentIndex = Widgets.IndexOf(widget);
        int newIndex = Mathf.Clamp(currentIndex + shift, 0, Widgets.Count - 1);

        if (newIndex == currentIndex) return false;

        Widgets.RemoveAt(currentIndex);

        if (newIndex == Widgets.Count) {
            Widgets.Add(widget);
        } else {
            Widgets.Insert(newIndex, widget);
        }

        widget.transform.SetSiblingIndex(newIndex);
        OnReorder((T)widget, currentIndex, newIndex);

        if (newIndex == 0 && shift < 0) return false;
        if (newIndex == Widgets.Count - 1 && shift > 0) return false;
        return true;
    }

    public void Delete(T widget) {
        OnDelete(widget, Widgets.IndexOf(widget));
        Widgets.Remove(widget);
        Object.Destroy(widget.gameObject);
        SizeContainer();
    }

    public void Clear() {
        foreach(var widget in Widgets) {
            Object.Destroy(widget.gameObject);
        }
        Widgets.Clear();
    }

    protected virtual float SizeContainer() {
        float sum = 2 * padding;

        if (container.childCount > 0) {
            foreach (var widget in Widgets) {
                var r = ((RectTransform)widget.transform);
                sum += r.rect.height + padding;
            }
        }
        // Debug.Log($"Setting {container.name} height to {sum}");
        var size = container.sizeDelta;
        size.y = sum;
        container.sizeDelta = size;
        return size.y;
    }
}
