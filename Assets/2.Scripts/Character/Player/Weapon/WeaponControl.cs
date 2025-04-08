using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class WeaponControl : MonoBehaviour
{
    private float offset = -33f; // 무기 sprite의 회전값 조정
    public bool isHeroWeapon;

    public CharacterBase Owner;
    public Transform ProjectileTransform;

    private void OnEnable()
    {
        EnemyManager.OnMonsterGen += RefreshTarget;
        EnemyControl.OnDie += RefreshTarget;
    }

    private void OnDisable()
    {
        EnemyManager.OnMonsterGen -= RefreshTarget;
        EnemyControl.OnDie -= RefreshTarget;
    }

    private void Start()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        if (Owner != null)
            Owner.Target = SearchTarget();
    }
    private void Update()
    {
        if(Owner.Target != null)
        {
            LookAtTarget(Owner.Target.CenterPoint.position);
            if (Owner.State.AttackTermTimer >= Owner.State.AttackTermTime)
            {
                var Attack = FXPoolManager.Instance.Pop(EFXPoolType.AttackObject01_Bullet, ProjectileTransform.position);
                Attack.GetComponent<RangedAttackObject>().Init(ProjectileTransform.position, Owner.Target, Owner.State.Range);
                Owner.State.AttackTermTimer = 0;
            }
        }

        

    }

    // 무기를 대상 방향으로 회전
    private void LookAtTarget(Vector3 targetPosition)
    {
        targetPosition.z = 0;
        Vector3 direction = targetPosition - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, -90f, 90f); // 오른쪽 방향에서 벗어나지 않도록 제한

        transform.rotation = Quaternion.Euler(0f, 0f, angle + offset);
    }

    private void RefreshTarget()
    {
        Owner.Target = SearchTarget();
    }

    public EnemyControl SearchTarget() => EnemyManager.Instance.FindNearTarget(this.Owner.transform.position, Owner.State.Range);
}
