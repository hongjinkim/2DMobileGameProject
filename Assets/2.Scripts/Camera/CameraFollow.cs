using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(2.5f, 3f, -15f);

    private void Start()
    {
        target = PlayerManager.GetHeroTransform();
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x + offset.x, offset.y, offset.z);
        }
    }
}
