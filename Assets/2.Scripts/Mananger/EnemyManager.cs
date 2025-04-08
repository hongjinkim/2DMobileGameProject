using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : BasicSingleton<EnemyManager>
{
    // 적 리젠
    public float RegenTermTimer = 0; 
    public float RegenTermTime = 0.5f;
    private bool canRegen = false;

    private bool gameStart = false;

    public static event Action OnMonsterGen = delegate { };

    public Transform EnemySpawner;


    [SerializeField] private List<EnemyControl> MonsterList = new List<EnemyControl>();

    private void OnEnable()
    {
        HeroControl.OnDie += EnemyStopAttack;
    }

    private void OnDisable()
    {
        HeroControl.OnDie -= EnemyStopAttack;
    }

    private void Update()
    {
        RegenTermTimer += Time.deltaTime;
        SetTimer();
        GenMonster();
    }

    private void SetTimer()
    {
        if (RegenTermTimer >= RegenTermTime &&  gameStart)
            canRegen = true;
        else
            canRegen = false;
    }

    private void Start()
    {
        StartStage();
    }

    private void StartStage()
    {
        gameStart = true;
        EnemyClearAll();
    }

    private void GenMonster()
    {
        if(canRegen)
        {
            RegenTermTimer = 0;
            EnemySummon();
            OnMonsterGen?.Invoke();
        }
    }

    private void EnemySummon()
    {
        var NewMonster = EnemyPoolManager.Instance.Pop(EPoolType.Monster);
        var MonsterControl = NewMonster.GetComponent<EnemyControl>();

        SetPosition(MonsterControl);

        MonsterControl.Init(0); // 몬스터 종류가 한 개 이므로 일단 항상 0으로
        MonsterList.Add(MonsterControl);

        // 몬스터 소환 이펙트
        //FXPoolManager.Instance.Pop(EFXPoolType.MonsterSpawnEffect, monsterComp.transform.position + new Vector3(0, -2.2f, 0));
    }

    private void SetPosition(EnemyControl m)
    {
        m.transform.localPosition = EnemySpawner.position;
    }

    private void EnemyStopAttack()
    {
        for (int i = 0; i < MonsterList.Count; i++)
        {
            MonsterList[i].StopAttackWhenHeroDie();
        }
    }

    // 적 전부 삭제
    private void EnemyClearAll()
    {
        for (int i = 0; i < MonsterList.Count; i++)
        {
            EnemyPoolManager.Instance.Push(MonsterList[i].gameObject, EPoolType.Monster);
        }
        MonsterList.Clear();
    }

    // 적 처치(사망처리, 보상)
    public void EnemyDeath(EnemyControl m)
    {
        MonsterList.Remove(m);
    }

    public EnemyControl FindNearTarget(Vector3 pos, float detectionRange = 100f)
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
            if (distance > detectionRange)
                return null;
            for (int i = 0; i < monster.Count; i++)
            {
                var monsterDistance = Vector2.Distance(pos, monster[i].transform.position);
                if (monsterDistance > detectionRange)
                    return null;
                else if (monsterDistance < distance)
                {
                    distance = monsterDistance;
                    select = monster[i];
                }
            }
            return select;
        }
    }
}
