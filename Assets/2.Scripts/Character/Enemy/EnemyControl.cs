using UnityEngine;
using UnityEngine.Rendering;
using System;
using Random = UnityEngine.Random;
using UnityEditor.Build;

public class MonsterInfo
{
    public EEnemyType EnemyType;
    public float AttackPower = 0;
    public float MaxHp = 1;
    public float DropGold = 0;
    public int RowIndex; // 0, 1, 2 열 중 현재 위치한 인덱스
}

public class EnemyControl : CharacterBase
{
    public static event Action OnDie = delegate { };
    public static event Action<float> OnHit = delegate { };

    public MonsterInfo Info = new MonsterInfo();

    public Animator Anim;
    public AnimationClip AttackClip;
    public AnimationClip RunClip;
    private State<EnemyControl>[] States;
    private State<EnemyControl> CurrentState;
    [SerializeField] private SortingGroup sortingGroup;

    [SerializeField] private LayerMask layerMask;

    [Header("Raycast")]
    [SerializeField] private Transform checkAnotherUnitRayTransform;
    [SerializeField] private Transform groundRayTransform;

    [Header("State")]
    public bool IsGrounded = true;
    public bool CanJump = false;
    public bool IsJumping= false;
    public bool IsOnAnotherUnit = true;

    private bool isEnd = false;

    protected new void Awake()
    {
        base.Awake();
        Setup();
    }

    protected new void Update()
    {

    }


    public void Init(int EnemyIndex)
    {
        //기본설정 (몬스터 종류에 따라 갱신될 수 있음)
        bool showHPWhenStart = false;

        SetStat();

        AttackCollider.gameObject.SetActive(false);     // 공격 콜라이더도 끈상태로 시작

        // 적 상태 설정
        InitHP(Info.MaxHp, showHPWhenStart);      // 체력 설정
        State.IsLive = true;
        //State.HitTermTime = 0f;

        Target = null; //타겟리셋 필요

        ChangeState(EActType.Init);      // 유닛 생성 시 마다 초기화 목적
    }

    private void SetStat()
    {
        Info.AttackPower = 4f;
        Info.MaxHp = 100f;
        Info.DropGold = 10f;
        Info.RowIndex = Random.Range(0, 3);
        gameObject.layer = LayerMask.NameToLayer($"Enemy_{Info.RowIndex+1}");
        layerMask = (1 << LayerMask.NameToLayer($"Enemy_{Info.RowIndex + 1}")) | (1 << LayerMask.NameToLayer($"Ground_{Info.RowIndex + 1}"));
        sortingGroup.sortingOrder = Info.RowIndex;
        State.NoneAttack = false;
        State.NoneMove = false;
        State.Speed = Random.Range(3f, 6f);
    }

    private void Setup()
    {
        States = new State<EnemyControl>[6];

        States[(int)EActType.Init] = new EnemyInit();
        States[(int)EActType.Idle] = new EnemyIdle();
        States[(int)EActType.Move] = new EnemyMove();
        States[(int)EActType.Attack] = new EnemyAttack();
        States[(int)EActType.Die] = new EnemyDie();
        States[(int)EActType.Jump] = new EnemyJump();

        isEnemy = true;
    }

    // 가까운 대상 탐색
    public CharacterBase NearPlayer()
    {
        return PlayerManager.Instance.FindNearTarget(this.CenterPoint.position);
    }

    public void Attack()
    {
        if (State.AttackTermTimer >= State.AttackTermTime)
        {
            AttackCollider.gameObject.SetActive(true);
            State.AttackTermTimer = 0;
        }
        else
            AttackCollider.gameObject.SetActive(true);

    }

    public new void TakeHit(AttackInfo HitInfo, float damage)
    {
        base.TakeHit(HitInfo,damage);
        State.CurrentHp -= damage;
        accumulatedDamage += damage;

        if(State.CanPopDamageText)
        {
            FXPoolManager.Instance.PopDamageText(this.transform.position + new Vector3(0, 1f, 0), HitInfo, accumulatedDamage);
            FXPoolManager.Instance.Pop(HitInfo.EffectType, new Vector3(this.transform.position.x, this.transform.position.y + 2f, 0));
            accumulatedDamage = 0;    // 누적 대미지 초기화
            State.HitTermTimer = 0;         // 타이머 초기화
        }
        UpdateHp();

        if (State.CurrentHp <= 0) { Die(); } // 사망 처리
    }

    // 상태 변경
    public void ChangeState(EActType NewState)
    {
        // 바꾸려는 상태가 비어있는 경우
        if (States[(int)NewState] == null)
            return;

        // 현재 재생중인 상태가 존재하면 기존 상태 종료
        if (CurrentState != null)
        {
            CurrentState.Exit(this);
        }

        // 새로운 상태로 변경하고, 새로 바뀐 상태의 Enter() 메소드 호출
        State.CurrentAct = NewState;
        CurrentState = States[(int)NewState];
        CurrentState.Enter(this);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Monster") && CurrentState != States[(int)EActType.Jump])
        {
            EnemyControl target;
            target = collision.gameObject.GetComponent<EnemyControl>();

            CanJump =
                collision.contacts[0].normal.x <= 1.0f &&
                collision.contacts[0].normal.x >= 0.5f &&
                target.IsGrounded == true &&
                target.IsOnAnotherUnit == true &&
                target.isEnd == false &&
                IsOnAnotherUnit;

            if (CanJump)
            {
                IsJumping = true;
                ChangeState(EActType.Jump);
                return;
            }

            IsJumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
      
        if (collision.collider.CompareTag("Monster") && CurrentState == States[(int)EActType.Jump])
        {
            ChangeState(EActType.Idle);
            return;
        }
    }

    public void StopAttackWhenHeroDie()
    {
        Target = null;
        ChangeState(EActType.Move);
    }

    protected override void HandleEvent(string eventName)
    {

    }

    protected override void Finish(EActType ActType)
    {

    }
    private void LateUpdate()
    {
        base.Update();

        if (CurrentState != null)
        {
            CurrentState.Execute(this);
        }

        // 정지
        if (State.NoneMove == true)
            return;
        CheckRaycast();

    }
    private void FixedUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.FixedExecute(this);
        }

        // 정지
        if (State.NoneMove == true)
            return;
    }

    private void CheckRaycast()
    {
        RaycastHit2D raycastHit2D_Stepping = Physics2D.Raycast((Vector2)checkAnotherUnitRayTransform.position, Vector2.up, 1f, layerMask);
        if (raycastHit2D_Stepping)
        {
            if (raycastHit2D_Stepping.collider.CompareTag("Monster")) IsOnAnotherUnit = false;
        }
        else
            IsOnAnotherUnit = true;

        RaycastHit2D raycastHit2D_Ground = Physics2D.Raycast((Vector2)groundRayTransform.position, Vector2.down, 0.01f, layerMask);
        if (raycastHit2D_Ground)
        {
            IsGrounded = true;
        }
        else
            IsGrounded = false;
    }

    public override void Die()
    {
        base.Die();
        // 살아있는 적만 처리(일반공격/멀티공격 중복 Die 체크 방지)
        if (State.IsLive == false) return;

        // 피격 이펙트
        //FXPoolManager.Instance.Pop(EFXPoolType.DestroyEnemy, new Vector3(this.transform.position.x, this.transform.position.y + 1f, 0));

        ChangeState(EActType.Die);
        State.IsLive = false;
        EnemyManager.Instance.EnemyDeath(this);

        if (!PlayerManager.Instance.HeroControl.State.IsLive)
            return;

        
         OnDie?.Invoke();
    }

    // 사망 애니메이션을 위한 시간 필요
    public void DieDisable()
    {
        EnemyPoolManager.Instance.Push(gameObject, EPoolType.Monster);
    }
}
