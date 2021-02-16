using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListWidget<T> : MonoBehaviour where T:ListWidget<T>
{
    public Button moveUpButton;
    public Button moveDownButton;
    public Button destroyButton;

    WidgetList<T> _list;

    public void OnAdd(WidgetList<T> list) {
        _list = list;
    }

    public void Move(int shift) {        
        _list.Reorder((T)this, shift);
    }

    public void Delete() {
        _list.Delete((T)this);
    }
}
