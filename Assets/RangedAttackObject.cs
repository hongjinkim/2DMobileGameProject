using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackObject : MonoBehaviour
{
    private Rigidbody2D Rigid;
    private AutoPush Pool;
    private CharacterBase Target;

    private float Going_time;

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody2D>();
        Pool = this.GetComponent<AutoPush>();
    }

    public void Init(Vector2 startPos, Vector2 direction, float range)
    {

        Rigid.velocity += direction * (range / Pool.deathtimer);
        Going_time = 0;
    }
    public void Init(Vector2 startPos, CharacterBase target , float range)
    {
        if(target != null)
            this.Target = target;

        // 타겟(방향) 설정
        Vector2 targetPos = Target.CenterPoint.position;

        // 시작점 및 방향 설정
        Vector2 direction = (targetPos - startPos).normalized;

        Rigid.velocity += direction * (range / Pool.deathtimer);
        Going_time = 0;
    }

    protected void Update()
    {
        if (Going_time < Pool.deathtimer) { Going_time += Time.deltaTime; }

        else { Pool.PoolPush(); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Target.tag.ToString()) == false) { return; }
        if (collision.CompareTag("Ground"))
        {
            Pool.PoolPush();
        }
        else if (collision.CompareTag("Monster"))
        {
            AttackInfo AttackInfo = new AttackInfo();

            // 기본 공격
            AttackInfo.Damage = StatManager.Instance.WeaponDamage;
            AttackInfo.AttackType = EAttackType.Normal;

            //AttackInfo.EffectType = EFXPoolType.HitEffect_Blue;
            AttackInfo.HitCount = 1;

            collision.gameObject.GetComponent<EnemyControl>().TakeHit(AttackInfo, AttackInfo.Damage);
            Pool.PoolPush();
        }
    }

}
