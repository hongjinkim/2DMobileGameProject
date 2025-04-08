using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TowerManager : BasicSingleton<TowerManager>
{
    [SerializeField] private int maxTowerCount = 1;
    [SerializeField] private float towerSpacing = 1.5f;
    [SerializeField] private Transform towerParent;
    [SerializeField] private GameObject towerPrefab;

    public List<TowerControl> activeTowers = new List<TowerControl>();
    private TowerPoolManager towerPool;

    private void Start()
    {
        towerPool = TowerPoolManager.Instance;

        InitializeTowers();
    }


    private void OnEnable()
    {
        TowerControl.OnDie += FillEmptySpace;
    }
    private void OnDisable()
    {
        TowerControl.OnDie -= FillEmptySpace;
    }

    private void InitializeTowers()
    {
        // 초기 타워 배치
        for (int i = 0; i < maxTowerCount; i++)
        {
            SpawnTower(i);
        }
        SetHeroTransform();
    }

    private void SpawnTower(int towerIndex)
    {
        GameObject towerObj = towerPool.Pop(EPoolType.Tower);

        if (towerObj != null)
        {
            towerObj.transform.SetParent(towerParent);
            towerObj.transform.localPosition = CalculateTowerPosition(towerIndex);

            // Tower 컴포넌트 가져오기 또는 추가
            TowerControl tower = towerObj.GetComponent<TowerControl>();
            tower.TowerIndex = towerIndex;
            tower.Init();

            // 활성 타워 목록에 추가
            if (towerIndex >= activeTowers.Count)
            {
                activeTowers.Add(tower);
            }
            else
            {
                activeTowers[towerIndex] = tower;
            }
        }
    }

    private Vector3 CalculateTowerPosition(int index)
    {
        return new Vector3(0, index * towerSpacing, 0);
    }

    private void FillEmptySpace(int destroyedIndex)
    {
        towerPool.Push(gameObject, EPoolType.Tower);

        StartCoroutine(ShiftTowersCoroutine(destroyedIndex));
    }

    private IEnumerator ShiftTowersCoroutine(int emptyIndex)
    {
        for (int i = emptyIndex + 1; i < activeTowers.Count; i++)
        {
            TowerControl tower = activeTowers[i];
            if (tower != null)
            {
                int newIndex = i - 1;
                Vector3 targetPosition = CalculateTowerPosition(newIndex);

                // 타워 위치 업데이트
                tower.TowerIndex = newIndex;

                tower.transform.DOMove(targetPosition, 0.3f)
                    .SetEase(Ease.OutQuad);

                // 활성 타워 리스트 업데이트
                activeTowers[newIndex] = tower;
            }
        }

        // DOTween 애니메이션이 완료될 때까지 약간의 대기 시간
        yield return new WaitForSeconds(0.4f);

        // 마지막 위치에 새 타워 생성
        if (activeTowers.Count > 0)
        {
            int lastIndex = activeTowers.Count - 1;
            activeTowers[lastIndex] = null;
            SpawnTower(lastIndex);
        }
    }

    private void SetHeroTransform()
    {
        PlayerManager.SetHeroPosition(GetTowerTopTransform().position + new Vector3(0, towerSpacing, 0));
    }

    private Transform GetTowerTopTransform()
    {
        // 활성화된 타워가 없는 경우
        if (activeTowers.Count == 0)
            return towerParent; // 기본값으로 부모 Transform 반환

        // 가장 높은 인덱스의 타워 찾기 (비어있지 않은 타워 중)
        for (int i = activeTowers.Count - 1; i >= 0; i--)
        {
            if (activeTowers[i] != null)
            {
                return activeTowers[i].transform;
            }
        }

        // 모든 슬롯이 비어있는 경우
        return towerParent;
    }
}
