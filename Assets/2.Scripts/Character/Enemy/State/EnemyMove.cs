using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : State<EnemyControl>
{
    public override void Enter(EnemyControl entity)
    {
        entity.Anim.speed = entity.State.Speed / entity.RunClip.length;
        entity.Rigid.velocity = Vector3.zero;
    }

    public override void Execute(EnemyControl entity)
    {
        // 타겟 없으면 idle전환
        if (entity.Target == null) { entity.ChangeState(EActType.Idle); }
        // 타켓이 있는 경우
        else
        {
            float Distance = Vector2.Distance(entity.CenterPoint.position, entity.Target.CenterPoint.position);

            //사거리 안에 있는 경우
            if (Distance < entity.State.Range)
            {
                //공격텀인 경우 공격 발동
                if (entity.State.AttackTermTimer >= entity.State.AttackTermTime) { entity.ChangeState(EActType.Attack); }

                //공격텀이 아닌 경우 idle전환
                else { entity.ChangeState(EActType.Idle); }
            }
            else
            {
                entity.transform.position += Vector3.left * entity.State.Speed * Time.deltaTime;
            }
        }
    }
    public override void Exit(EnemyControl entity)
    {
    }
}
