using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleMove : MonoBehaviour
{
    public float maxRange = 0.2f;
    public float speed = 1.0f;

    private float originY;

    private void Start()
    {
        originY = transform.position.y;
    }

    void Update()
    {
        float amount = Mathf.Sin(speed * Time.time) * maxRange;
        transform.position = new Vector3(transform.position.x, originY + amount, transform.position.z);
    }
}
