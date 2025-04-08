using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;
using UnityEngine.Rendering;

public class TowerControl : CharacterBase
{
    public int TowerIndex;

    public static event Action<int> OnDie = delegate { };

    protected new void Awake()
    {
        base.Awake();
        Setup();
    }

    public void Init()
    {
        //기본설정 (몬스터 종류에 따라 갱신될 수 있음)
        bool showHPWhenStart = false;

        // 적 상태 설정
        InitHP(100, showHPWhenStart);      // 체력 설정
        State.IsLive = true;
    }

    private void Setup()
    {
        isEnemy = false;
    }


    public new void TakeHit(AttackInfo HitInfo, float damage)
    {
        base.TakeHit(HitInfo, damage);
        Debug.Log("attacked");

        State.CurrentHp -= damage;
        accumulatedDamage += damage;

        if (State.CanPopDamageText)
        {
            FXPoolManager.Instance.PopDamageText(this.transform.position + new Vector3(0, 1f, 0), HitInfo, accumulatedDamage);
            FXPoolManager.Instance.Pop(HitInfo.EffectType, new Vector3(this.transform.position.x, this.transform.position.y + 2f, 0));
            accumulatedDamage = 0;    // 누적 대미지 초기화
            State.HitTermTimer = 0;         // 타이머 초기화
        }
        UpdateHp();

        if (State.CurrentHp <= 0) { Die(); } // 사망 처리
    }

    protected override void HandleEvent(string eventName)
    {

    }

    protected override void Finish(EActType ActType)
    {

    }

    public override void Die()
    {
        //base.Die();
        //// 살아있는 적만 처리(일반공격/멀티공격 중복 Die 체크 방지)
        //if (State.IsLive == false) return;

        //// 피격 이펙트
        ////FXPoolManager.Instance.Pop(EFXPoolType.DestroyEnemy, new Vector3(this.transform.position.x, this.transform.position.y + 1f, 0));

        //ChangeState(EActType.Die);
        //State.IsLive = false;
        //EnemyManager.Instance.EnemyDeath(this);

        //if (!PlayerManager.Instance.HeroControl.State.IsLive)
        //    return;


        //OnDie?.Invoke();
    }

}
