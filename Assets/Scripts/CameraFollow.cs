using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] [Range(0.5f, 5f)] private float _followSpeed;

    private void Update()
    {
        var targetPosition = _target.position;
        targetPosition.z = transform.position.z;
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
    }
}
