using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementShift.asset", menuName = "DataMon/Status Effect/Element Shift")]
public class ElementShiftStatusEffect : StatusEffect
{
    public override void RemoveFrom(Monster target) {
        target.currentElement = target.species.element;
        base.RemoveFrom(target);
    }
}
