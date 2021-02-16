using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "DataImporter.asset", menuName ="DataMon/Importer")]
public class DataImporter : ScriptableObject
{
    public string sourceFilePath;

    public ElementCollection elementCollection;
    public SpeciesCollection speciesCollection;

    public string lastImport;

    [ContextMenu("Import")]
    public void Import() {
        var excel = new ExcelImporter(sourceFilePath);

        // First, import elements.
        var elements = ImportElements(excel);

        // Then, import abilities (can reference elements).
        var abilities = ImportAbilities(excel, elements);

        // Lastly, import monster species (can reference elements and abilities).
        var monsters = ImportMonsterSpecies(excel, elements, abilities);

        MarkChangesForSaving(this);
        lastImport = System.DateTime.Now.ToString();

        AssetDatabase.SaveAssets();
    }

    Dictionary<string, Element> ImportElements(ExcelImporter excel) {   
        
        if (!excel.TryGetNamedRange("Elements", out ExcelImporter.Range range))
            throw new System.IO.InvalidDataException($"No Elements range was found!");

        var assets = GetAllAssetsOfType<Element>();
        int originalCount = assets.Count;

        var elementsInUse = new List<Element>();
        
        for (int row = 1; row <= range.RowCount; row++) {
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
        for(int row = 0; row < elementsInUse.Count; row++) {
            for(int column = 0; column < elementsInUse.Count; column++) {
                multipliers[column] = range.GetValue<float>(row + 1, column + 1);
            }
            elementCollection.SetElement(row, elementsInUse[row], multipliers);
        }

        Debug.Log($"Successfully imported {elementsInUse.Count} elements " +
            $"({assets.Count - originalCount} new, {assets.Count - elementsInUse.Count} unused).");
        return assets;
    }

    Dictionary<string, Ability> ImportAbilities(ExcelImporter excel, Dictionary<string, Element> elements) {       

        if (!excel.TryGetTable("Abilities", out ExcelImporter.Table table))
            throw new System.IO.InvalidDataException($"No Abilities table was found!");

        var specials = GetAllAssetsOfType<SpecialEffect>();

        var assets = GetAllAssetsOfType<Ability>();        
        int originalCount = assets.Count;

        int nonBlankCount = 0;

        for (int row = 1; row <= table.RowCount; row++) {

            string name = table.GetValue<string>(row, "Name");
            if (string.IsNullOrWhiteSpace(name)) continue;

            nonBlankCount++;

            var ability = GetOrCreateAsset(name, assets, "Abilities");

            string elementName = table.GetValue<string>(row, "Element");            
            if(!elements.TryGetValue(elementName, out ability.element))
                Debug.LogError($"Element {elementName} used by ability {name} was not found.");

            ability.staminaCost = table.GetValue<int>(row, "Stamina Cost");
            ability.minDamage = table.GetValue<int>(row, "Min Damage");
            ability.maxDamage = table.GetValue<int>(row, "Max Damage");

            string specialName = table.GetValue<string>(row, "Special Effect");
            if (string.IsNullOrWhiteSpace(specialName) || !specials.TryGetValue(specialName, out SpecialEffect special)) {
                if (!string.IsNullOrWhiteSpace(specialName))
                    Debug.LogError($"Special effect {specialName} used by ability {name} was not found.");

                ability.special = null;
                ability.specialChance = 0;                
            } else {
                ability.special = special;
                ability.specialChance = table.GetValue<float>(row, "Effect Chance");
            }
        }

        Debug.Log($"Successfully imported {nonBlankCount} abilities " +
            $"({assets.Count - originalCount} new, {assets.Count - nonBlankCount} unused).");
        return assets;
    }

    Dictionary<string, Species> ImportMonsterSpecies(ExcelImporter excel, Dictionary<string, Element> elements, Dictionary<string, Ability> abilities) {        

        if (!excel.TryGetTable("Monsters", out ExcelImporter.Table table))
            throw new System.IO.InvalidDataException($"No Monsters table was found!");

        var assets = GetAllAssetsOfType<Species>();
        int originalCount = assets.Count;

        int nonBlankCount = 0;

        for (int row = 1; row <= table.RowCount; row++) {

            string name = table.GetValue<string>(row, "Name");
            if (string.IsNullOrWhiteSpace(name)) continue;

            nonBlankCount++;

            var monster = GetOrCreateAsset(name, assets, "Monsters/Species");

            string rarityName = table.GetValue<string>(row, "Rarity");
            if (!System.Enum.TryParse(rarityName, out monster.rarity))
                Debug.LogError($"Unknown rarity {rarityName} used by monster species {name}.");

            string elementName = table.GetValue<string>(row, "Element");
            if (!elements.TryGetValue(elementName, out monster.element))
                Debug.LogError($"Element {elementName} used by monster {name} was not found.");

            monster.baseStats.health    = table.GetValue<int>(row, "Health");
            monster.baseStats.stamina   = table.GetValue<int>(row, "Stamina");
            monster.baseStats.attack    = table.GetValue<int>(row, "Attack");
            monster.baseStats.defense   = table.GetValue<int>(row, "Defense");
            monster.baseStats.luck      = table.GetValue<int>(row, "Luck");

            monster.abilities[0] = SetAbility(row, 1, table, abilities);
            monster.abilities[1] = SetAbility(row, 2, table, abilities);

            monster.OnValidate();
        }

        MarkChangesForSaving(speciesCollection);
        speciesCollection.species.Clear();
        foreach (var monster in assets.Values) {
            speciesCollection.species.Add(monster);            
        }
        speciesCollection.OnEnable();

        Debug.Log($"Successfully imported {nonBlankCount} monsters " +
            $"({assets.Count - originalCount} new, {assets.Count - nonBlankCount} unused).");
        return assets;
    }

    Species.AbilitySlot SetAbility(int row, int index, ExcelImporter.Table table, Dictionary<string, Ability> abilities) {
        string abilityName = table.GetValue<string>(row, $"Ability {index}");

        Species.AbilitySlot slot = default;

        if (string.IsNullOrWhiteSpace(abilityName) || !abilities.TryGetValue(abilityName, out Ability ability)) {
            if (!string.IsNullOrWhiteSpace(abilityName)) Debug.LogError($"Ability {abilityName} not found.");
            return slot;
        }
        
        slot.ability = ability;
        slot.chanceToUse = table.GetValue<float>(row, $"Chance to Use {index}");

        return slot;
    }

    void MarkChangesForSaving(Object target) {
        Undo.RecordObject(target, "Data Import");
        EditorUtility.SetDirty(target);
    }

    T GetOrCreateAsset<T>(string name, Dictionary<string, T> existing, string folder) where T : ScriptableObject{
        if (!existing.TryGetValue(name, out T asset)) {
            asset = CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, $"Assets/{folder}/{name}.asset");
            existing.Add(name, asset);
        }

        MarkChangesForSaving(asset);
        return asset;
    }

    Dictionary<string, T> GetAllAssetsOfType<T>() where T: Object {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        var collection = new Dictionary<string, T>(guids.Length);

        foreach (var guid in guids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            collection.Add(asset.name, asset);
        }

        return collection;
    }

    T GetUniqueAssetOfType<T>() where T:Object {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

        if (guids.Length > 1) {
            Debug.LogWarning($"{guids.Length} assets of type {typeof(T).Name} found, but your code expects only one.");
        } else if (guids.Length == 0) {
            throw new System.IO.FileNotFoundException($"Could not find an asset of type {typeof(T).Name}.");
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<T>(path);        
    }
}
