using UnityEngine;

[System.Serializable]
public struct MonsterStats
{
    public float health;
    public float stamina;
    public float attack;
    public float defense;
    public float luck;
    
    public float this[Stat stat] {
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

    public static MonsterStats operator + (MonsterStats a, MonsterStats b) {
        MonsterStats stats;

        stats.health    = a.health  + b.health;
        stats.stamina   = a.stamina + b.stamina;
        stats.attack    = a.attack  + b.attack;
        stats.defense   = a.defense + b.defense;
        stats.luck      = a.luck    + b.luck;

        return stats;
    }

    public void RoundAndClamp() {
        health  = Mathf.Max(Mathf.RoundToInt(health),   0);
        stamina = Mathf.Max(Mathf.RoundToInt(stamina),  0);
        attack  = Mathf.Max(Mathf.RoundToInt(attack),   0);
        defense = Mathf.Max(Mathf.RoundToInt(defense),  0);
        luck    = Mathf.Max(Mathf.RoundToInt(luck),     0);
    }
}
