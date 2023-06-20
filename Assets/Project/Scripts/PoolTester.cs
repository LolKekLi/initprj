using System;
using Project;
using UnityEngine;
using Zenject;

public class PoolTester : MonoBehaviour
{
    [SerializeField]
    private Transform _spawnPoint;

    [Inject]
    private PoolManager _poolManager;


    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     var testPoolItem = _poolManager.Get<TestPoolItem>(_poolManager.PoolSettings.TestPoolItem,
        //         _spawnPoint.position, Quaternion.identity, transform);
        // }
    }
}