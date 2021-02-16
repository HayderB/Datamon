using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Elements.asset", menuName = "DataMon/Elements Collection")]
public class ElementCollection : ScriptableObject
{
    public static ElementCollection Instance { get; private set; }

    [System.Serializable]
    struct ElementRow {
#if UNITY_EDITOR
        [HideInInspector]
        public string name;
#endif
        public Element element;
        public float[] attackMultipliers;

        public ElementRow(Element element, float[] attackMultipliers) {
            this.element = element;
            this.attackMultipliers = attackMultipliers;
            #if UNITY_EDITOR
            name = element.name;
            #endif
        }
    }

    [SerializeField]
    ElementRow[] _elementTable;

    Dictionary<Element, int> _elementIndex;

    public void SetElementCount(int count) {
        if(_elementTable == null || _elementTable.Length != count)
            _elementTable = new ElementRow[count];
    }

    public void SetElement(int id, Element element, float[] attackMultipliers) {
        _elementTable[id] = new ElementRow(element, (float[])attackMultipliers.Clone());
    }

    private void OnEnable() {
        if (_elementTable == null) return;

        Instance = this;

        _elementIndex = new Dictionary<Element, int>(_elementTable.Length);
        for (int i = 0; i < _elementTable.Length; i++)
            _elementIndex.Add(_elementTable[i].element, i);
    }

    public float GetAttackMultiplier(Element attack, Element defender) {
        int a = _elementIndex[attack];
        int d = _elementIndex[defender];

        return _elementTable[a].attackMultipliers[d];
    }
}
