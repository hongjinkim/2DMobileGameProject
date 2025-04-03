using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    public float offset = -33f;
    public bool isPlayerWeapon = false;

    public CharacterBase Owner;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    private void Update()
    {
        if (InputHandler.IsTouchActive() && isPlayerWeapon)
        {
            LookAtTarget(InputHandler.GetCurrentTouchPosition());
        }
        else
        {
            //LookAtTarget(FindNearestTarget());
        }
    }

    // 무기를 대상 방향으로 회전
    private void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, -90f, 90f); // 오른쪽 방향에서 벗어나지 않도록 제한

        transform.rotation = Quaternion.Euler(0f, 0f, angle + offset);
    }

    private void RefreshTarget()
    {
        Owner.Target = SearchTarget();
    }

    public EnemyControl SearchTarget() => EnemyManager.Instance.FindNearTarget(this.Owner.transform.position);
}
