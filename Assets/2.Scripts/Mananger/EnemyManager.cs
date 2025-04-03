using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : BasicSingleton<EnemyManager>
{
    public static event Action OnMonsterGen = delegate { };

    private List<EnemyControl> MonsterList = new List<EnemyControl>();

    public EnemyControl FindNearTarget(Vector3 pos)
    {
        var monster = MonsterList;

        if (monster.Count <= 0)
        {
            return null;
        }
        else
        {
            EnemyControl select = monster[0];
            var distance = Vector2.Distance(pos, monster[0].transform.position);
            for (int i = 0; i < monster.Count; i++)
            {
                var monsterDistance = Vector2.Distance(pos, monster[i].transform.position);
                if (monsterDistance < distance)
                {
                    distance = monsterDistance;
                    select = monster[i];
                }
            }
            return select;
        }
    }
}
