using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MonsterStats
{
    public int health;
    public int stamina;
    public int attack;
    public int defense;
    public int luck;
    
    public int this[Stat stat] {
        get {
            switch(stat) {
                case Stat.Health:   return health;
                case Stat.Stamina:  return stamina;
                case Stat.Attack:   return attack;
                case Stat.Defense:  return defense;
                case Stat.Luck:     return luck;
                default:            return default;
            }
        }
        set {
            switch (stat) {
                case Stat.Health:   health  = value; return;
                case Stat.Stamina:  stamina = value; return;
                case Stat.Attack:   attack  = value; return;
                case Stat.Defense:  defense = value; return;
                case Stat.Luck:     luck    = value; return;                
            }
        }
    }
}
