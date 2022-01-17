using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] List<Monster> monsters;

    public List<Monster> Monsters {
        get {
            return monsters;
        }
    }

    private void Start()
    {
        foreach (var pokemon in monsters)
        {
            pokemon.Init();
        }
    }

    public Monster GetHealthyMonster()
    {
        return monsters.Where(x => x.HP > 0).FirstOrDefault();
    }
}