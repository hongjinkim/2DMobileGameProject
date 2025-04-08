using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{
    [SerializeField] CharacterBase Attacker;    // 공격하는 개체
    [SerializeField] bool IsPushObject = false;       // 소멸 여부
    [SerializeField] private TargetType TargetTag;
    private enum TargetType { Monster, Player };


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        AttackInfo AttackInfo = new AttackInfo();
        AttackInfo.AttackerWorldPosition = Attacker.transform.position;

        if (collision.CompareTag(TargetTag.ToString()) == false)
            return;

        switch (TargetTag)
        {
            // 플레이어 -> 적
            case TargetType.Monster:

                // 기본 공격
                AttackInfo.Damage = StatManager.Instance.WeaponDamage;
                AttackInfo.AttackType = EAttackType.Normal;


                AttackInfo.HitCount = 1;

                EnemyControl Target = collision.gameObject.GetComponent<EnemyControl>();

                Target.TakeHit(AttackInfo, AttackInfo.Damage);

                break;
            // 적 -> 플레이어
            case TargetType.Player:
                EnemyControl Enemy = Attacker.GetComponent<EnemyControl>();



                AttackInfo.Damage = Enemy.Info.AttackPower;
                AttackInfo.AttackType = EAttackType.Enemy;
                
                AttackInfo.HitCount = 1;

                Enemy.Target.TakeHit(AttackInfo, AttackInfo.Damage);

                break;
            default:
                break;
        }

        // 소멸
        if (IsPushObject == true)
            PushObject();
    }

    // 공격 오브젝트 소멸
    protected virtual void PushObject()
    {
    }
}
