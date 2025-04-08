using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageFloating : MonoBehaviour
{
    private Animator Anim;
    [SerializeField] private TextMeshPro DamageText;
    [SerializeField] private Material enemyMt;
    [SerializeField] private Material normalMt;

    private void OnEnable()
    {
        this.Anim = this.GetComponent<Animator>();
        Anim.Play("DamageText");
    }

    // 데미지 및 데미지 타입에 따라 텍스트 설정
    public void SetDamage(AttackInfo attackInfo, float damage)
    {
        switch (attackInfo.AttackType)
        {
            case EAttackType.Enemy:
                DamageText.fontSharedMaterial = enemyMt;
                break;
            case EAttackType.Normal:
                DamageText.fontSharedMaterial = normalMt;
                break;
        }
        
        DamageText.text = $"{damage}";
    }

    public void AnimationEnd()
    {
        FXPoolManager.Instance.Push(gameObject, EFXPoolType.DamageText);
    }
}
