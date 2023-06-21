using System.Collections;
using System.Collections.Generic;
using Project;
using UnityEngine;
using Zenject;

public class ScenContextInstaller : MonoInstaller
{
    [SerializeField]
    private InGameUISystem _inGameUISystem;

    [SerializeField]
    private CameraController _cameraController;

    public override void InstallBindings()
    {
        Container.ParentContainers[0].Bind<InGameUISystem>().FromInstance(_inGameUISystem).AsCached();
        Container.ParentContainers[0].Bind<CameraController>().FromInstance(_cameraController).AsCached();
    }
}