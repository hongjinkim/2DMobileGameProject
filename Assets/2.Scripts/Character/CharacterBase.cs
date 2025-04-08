using System;
using UnityEngine;

[Serializable]
// 캐릭터 상태값
public class CharacterState
{
    public EActType CurrentAct;
    public float MaxHp;
    public float CurrentHp;

    public bool IsLive = true;               // 생존

    // 피격 텀
    public bool CanPopDamageText = true;
    public float HitTermTimer = 0;
    public float HitTermTime = 0.2f;

    // 공격 텀
    public float AttackTermTimer = 0;
    public float AttackTermTime = 0.5f;

    // 소환 시 잠시
    public float InitTimer = 0;
    public float InitTime = 0.3f;

    // 사망 시간
    public float DieTimer = 0;
    public float DieTime = 1f;           // 사망 이펙트 대기 시간

    public float Range = 4f;
    public float Speed = 10f;
    public Vector3 SpriteScale;
    public bool NoneAttack;     // 공격 유무
    public bool NoneMove;       // Idle상태 유지
}

public abstract class CharacterBase : MonoBehaviour
{
    [field: SerializeField] public CharacterBase Target { get; set; }
    [SerializeField] protected RectTransform HP_HUD;
    [SerializeField] protected RectTransform HP_HUD_After;
    [SerializeField] protected RoundedFillUI HP_HUD_Fill;
    [SerializeField] protected RoundedFillUI HP_HUD_Fill_After;
    [SerializeField] protected Transform CharacterSprites;
    [SerializeField] protected CapsuleCollider2D AttackCollider;
    public Transform CenterPoint;
    public Rigidbody2D Rigid;
    public CharacterState State = new CharacterState();
    private bool _showHPWhenStart;

    // 짧은 시간 내에 데미지를 여러번 받으면 텍스트가 겹침, 일정 시간 간격으로 받은 데미지를 합해서 출력
    [SerializeField]protected float accumulatedDamage = 0;

    protected bool isEnemy;

    protected void Awake()
    {
        State.SpriteScale = CharacterSprites.transform.localScale;
    }

    protected void Update()
    {
        State.AttackTermTimer += Time.deltaTime;
        State.HitTermTimer += Time.deltaTime;
        State.DieTimer += Time.deltaTime;
        State.InitTimer += Time.deltaTime;

        SetTimer();
    }

    // 각종 타이머 계산
    private void SetTimer()
    {
        // 타이머가 지정된 시간을 초과했고 누적된 대미지가 있으면 대미지 텍스트 표시
        if (State.HitTermTimer >= State.HitTermTime && accumulatedDamage > 0)
        {
            State.CanPopDamageText = true;
        }
        else
            State.CanPopDamageText = false;
    }
    public virtual void TakeHit(AttackInfo HitInfo, float Damage)
    {

    }

    public virtual void Die()
    {
        HP_HUD.gameObject.SetActive(false);
        HP_HUD_After.gameObject.SetActive(false);
    }

    protected abstract void HandleEvent(string eventName);

    protected abstract void Finish(EActType ActType);

    // 체력 초기화
    protected void InitHP(float maxHp, bool showHPWhenStart)
    {
        HP_HUD.transform.localScale = new Vector3(1, 1, 1);
        HP_HUD.gameObject.SetActive(showHPWhenStart);
        HP_HUD_After.gameObject.SetActive(showHPWhenStart);
        _showHPWhenStart = showHPWhenStart;

        State.MaxHp = maxHp;
        State.CurrentHp = maxHp;
        UpdateHp();
    }

    // 체력바 갱신
    protected void UpdateHp()
    {
        if (State.CurrentHp >= State.MaxHp)
        {
            HP_HUD_Fill.SetProgress(1f);
            HP_HUD_Fill_After.SetProgress(1f);
            HP_HUD_Fill_After.StopAfterSlide();
        }
        else if (State.CurrentHp <= 0 || State.CurrentHp * 1000000000 < State.MaxHp)
        {
            HP_HUD_Fill.SetProgress(0f);
            HP_HUD_Fill_After.SetProgress_After(0f);
        }
        else
        {
            if (!_showHPWhenStart) //시작시 노출 안됐다면 노출시킴
            {
                HP_HUD.gameObject.SetActive(true);
                HP_HUD_After.gameObject.SetActive(true);
                _showHPWhenStart = true;
            }

            float ratio = (float)(double)(State.CurrentHp / State.MaxHp);
            HP_HUD_Fill.SetProgress(ratio);
            HP_HUD_Fill_After.SetProgress_After(ratio);
        }
    }
}
