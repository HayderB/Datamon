using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "DataImporter.asset", menuName = "DataMon/ DataImporter")]
public class DataImporter : ScriptableObject
{
    public string sourceFilePath;

    public ElementCollection elementCollection;

    public string lastImportTime;

    [ContextMenu("Import")]
    public void Import()
    {
        var excel = new ExcelImporter(sourceFilePath);

        //Import elements
        var elements = ImportElements(excel);

        //Import abilities
        var abilities = ImportAbilities(excel, elements);


        MarkChangesForSaving(this);
        lastImportTime = System.DateTime.Now.ToString();

        AssetDatabase.SaveAssets();
    }

    Dictionary<string, Ability> ImportAbilities(ExcelImporter excel, Dictionary<string, Element> elements)
    {

        if (!excel.TryGetTable("Abilities", out ExcelImporter.Table table))
        {
            throw new System.IO.InvalidDataException("No Abilities Table was found in the excel file.");
        }

        var assets = GetAllAssetsOfType<Ability>();
        int originalCount = assets.Count;

        int nonBlankCount = 0;

        for (int row = 1; row <= table.RowCount; row++)
        {
            string name = table.GetValue<string>(row, "Name");

            if (string.IsNullOrWhiteSpace(name)) continue;

            nonBlankCount++;

            var ability = GetOrCreateAsset(name, assets, "Abilities");

            string elementName = table.GetValue<string>(row, "Element");
            if (string.IsNullOrWhiteSpace(elementName) || !elements.TryGetValue(elementName, out ability.element))
            {
                Debug.LogError($"Invalid element name {elementName} used by ability {name}.");
            }


            ability.staminaCost = table.GetValue<int>(row, "Stamina Cost");
            ability.minDamage = table.GetValue<int>(row, "Min Damage");
            ability.maxDamage = table.GetValue<int>(row, "Max Damage");
        }
        Debug.Log($"Successfully imported {nonBlankCount} abilities: " + $"({assets.Count - originalCount} new, {assets.Count - nonBlankCount} unused)");
        return assets;
    }



    void MarkChangesForSaving(Object target)
    {
        Undo.RecordObject(target, "Data Import");
        EditorUtility.SetDirty(target);
    }

    T GetOrCreateAsset<T>(string name, Dictionary<string, T> existing, string folder) where T : ScriptableObject
    {
        if (!existing.TryGetValue(name, out T asset))
        {
            asset = CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, $"Assets/{folder}/{name}.asset");
            existing.Add(name, asset);
        }

        MarkChangesForSaving(asset);
        return asset;
    }

    Dictionary<string, T> GetAllAssetsOfType<T>() where T : Object
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        var collection = new Dictionary<string, T>(guids.Length);

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            collection.Add(asset.name, asset);
        }

        return collection;
    }

    Dictionary<string, Element> ImportElements(ExcelImporter excel)
    {

        if (!excel.TryGetNamedRange("Elements", out ExcelImporter.Range range))
            throw new System.IO.InvalidDataException($"No Elements range was found!");

        var assets = GetAllAssetsOfType<Element>();
        int originalCount = assets.Count;

        var elementsInUse = new List<Element>();

        for (int row = 1; row <= range.RowCount; row++)
        {
            string name = range.GetValue<string>(row, 1);
            if (string.IsNullOrWhiteSpace(name)) continue;

            var element = GetOrCreateAsset(name, assets, "Elements");

            elementsInUse.Add(element);
        }

        if (!excel.TryGetNamedRange("ElementMultiplier", out range))
            throw new System.IO.InvalidDataException($"No Element Multiplier range was found!");

        MarkChangesForSaving(elementCollection);
        elementCollection.SetElementCount(elementsInUse.Count);

        var multipliers = new float[elementsInUse.Count];
        for (int row = 0; row < elementsInUse.Count; row++)
        {
            for (int column = 0; column < elementsInUse.Count; column++)
            {
                multipliers[column] = range.GetValue<float>(row + 1, column + 1);
            }
            elementCollection.SetElement(row, elementsInUse[row], multipliers);
        }

        Debug.Log($"Successfully imported {elementsInUse.Count} elements: " +
            $"({assets.Count - originalCount} new, {assets.Count - elementsInUse.Count} unused).");
        return assets;
    }

    Species.AbilitySlot SetAbility(int row, int index, ExcelImporter.Table table, Dictionary<string, Ability> abilities)
    {
        string abilityName = table.GetValue<string>(row, $"Ability {index}");

        Species.AbilitySlot slot = default;

        if (string.IsNullOrWhiteSpace(abilityName) || !abilities.TryGetValue(abilityName, out Ability ability))
        {
            if (!string.IsNullOrWhiteSpace(abilityName)) Debug.LogError($"Ability {abilityName} not found.");
            return slot;
        }

        slot.ability = ability;
        slot.chanceToUse = table.GetValue<float>(row, $"Chance to Use {index}");

        return slot;
    }
}
