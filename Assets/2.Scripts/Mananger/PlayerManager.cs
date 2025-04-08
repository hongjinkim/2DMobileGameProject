using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : BasicSingleton<PlayerManager>
{
    [SerializeField] private Transform heroTransform;

    public static event Action<float, float, float> OnShake = delegate { };
    public HeroControl HeroControl;
    public List<TowerControl> TowerList => TowerManager.Instance.activeTowers;
    public GameObject HeroObject;

    private void Start()
    {
        HeroObject.SetActive(true);
    }

    public CharacterBase FindNearTarget(Vector3 pos)
    {
        CharacterBase select = (HeroControl as CharacterBase);
        var distance = Vector2.Distance(pos, HeroControl.CenterPoint.position);

        CharacterBase compare;
        for (int i = 0; i < TowerList.Count; i++)
        {
            compare = TowerList[i];
            if (compare.State.IsLive) //살아있는 경우만 서치 포함
            {
                var HeroDistance = Vector2.Distance(pos, compare.CenterPoint.position);
                if (HeroDistance < distance)
                {
                    distance = HeroDistance;
                    select = compare;
                }
            }
        }
        return select;
    }

    public static Transform GetHeroTransform() => Instance.heroTransform;
    public static Vector3 GetHeroPosition() => Instance.heroTransform.position;
    
    public static void SetHeroTransform(Transform transform)
    {
        Instance.heroTransform = transform;
    }
    public static void SetHeroPosition(Vector3 pos)
    {
        Instance.heroTransform.position = pos;
    }
}
