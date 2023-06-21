using System;
using System.Collections;
using System.Collections.Generic;
using Project;
using UnityEngine;
using Zenject;

public class InGameUITest : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5;

    [SerializeField]
    private Transform _startZ;

    [SerializeField]
    private Transform _endZ;

    // [SerializeField]
    // private Transform _canvas;

    [Inject]
    private InGameUISystem _inGameUISystem;
    
    private float _endPosZ;
    private float _startPosZ;

    private void Start()
    {
        _endPosZ = _endZ.transform.position.z;
        _startPosZ = _startZ.transform.position.z;
        
        //_inGameUISystem.SetupHud(transform);
    }

    private void Update()
    {
        var transform1 = transform;

        transform1.position += Vector3.back * (Time.deltaTime * _speed);
//        _canvas.LookAt(_camera.position);

        if (transform1.position.z <= _endPosZ)
        {
            transform1.position = transform1.position.ChangeZ(_startPosZ);
        }
    }
}