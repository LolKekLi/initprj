using UnityEngine;
using Zenject;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Enumerable = System.Linq.Enumerable;

namespace Project
{
    public class InGameUISystem : MonoBehaviour
    {
        public const float ScaleCoefficient = 45f;

        [SerializeField]
        private TestUIHud _testUIHud;
        
        // [SerializeField]
        // private ResourceCounterUIHud _resourceCounterUIHudPrefab = null;
        
        private Camera _camera = null;
        
        //private ResourceCounterUIHud[] _resourceCounterUIHuds = null;
       
        private CancellationTokenSource _refreshCancellationTokenSource = null;
       // private TestUIHud[] _testUIHuds;
        
        [Inject]
        private CameraController _cameraController;
        [Inject]
        private DiContainer _diContainer = null;

        // [Inject]
        // private void Construct(CameraController cameraController,
        //     DiContainer diContainer)
        // {
        //     _camera = cameraController.Camera;
        //     _diContainer = diContainer;
        // }

        protected void Awake()
        {
            _camera = _cameraController.Camera;
            
            Prepare();
        }
        
        private void Start()
        {
            RefreshUILoopAsync(UniTaskUtil.RefreshToken(ref _refreshCancellationTokenSource)).Forget();
        }

        private void Prepare()
        {
            // _resourceCounterUIHuds = new ResourceCounterUIHud[RecyclersResourceHudCount];
            // PrepareHuds(_resourceCounterUIHuds, _resourceCounterUIHudPrefab);

            // _testUIHuds = new TestUIHud[500];
            // PrepareHuds(_testUIHuds, _testUIHud);
        }

        private void PrepareHuds(UIHudItemBase[] uiHudItems, UIHudItemBase uiHudItemPrefab)
        {
            for (int i = 0; i < uiHudItems.Length; i++)
            {
                uiHudItems[i] = _diContainer.InstantiatePrefabForComponent<UIHudItemBase>(uiHudItemPrefab, transform);
                
                uiHudItems[i].Prepare(this, _camera);
            }
        }

        private void Update()
        {
           // RefreshVisibility(_testUIHuds);
        }

        private void RefreshVisibility(UIHudItemBase[] uiHudItems)
        {
            for (int i = 0; i < uiHudItems.Length; i++)
            {
                if (!uiHudItems[i].IsFree)
                {
                    uiHudItems[i].RefreshVisibility();
                }
            }
        }
        
        private async UniTaskVoid RefreshUILoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!ReferenceEquals(_refreshCancellationTokenSource, null))
                {
                   // RefreshHuds(_testUIHuds);
                    
                    await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, cancellationToken);
                }
            }
            catch (OperationCanceledException e)
            {
            }
        }
        
        private void RefreshHuds(UIHudItemBase[] uiHudItems)
        {
            for (int i = 0; i < uiHudItems.Length; i++)
            {
                if (!uiHudItems[i].IsFree)
                {
                    uiHudItems[i].Refresh(ScaleCoefficient);
                }
            }
        }

        public Vector2 GetViewportPosition(UnityEngine.Vector3 worldPosition)
        {
            return _camera.WorldToViewportPoint(worldPosition);
        }

        // private void SetupResourceCounterUIHud(ResourceCounterUIHud[] resourceCounterUIHuds, UIHudData uiHudData)
        // {
        //     var resourceCounter = Enumerable.FirstOrDefault(resourceCounterUIHuds, h => h.IsFree);
        //
        //     if (!ReferenceEquals(resourceCounter, null))
        //     {
        //         resourceCounter.Setup(uiHudData);
        //     }
        //     else
        //     {
        //         DebugSafe.LogException(new System.Exception("No free in game UI"));
        //     }
        // }
        
        
        // private void RewardResourceController_CarOfferMade(UIHudData hudData)
        // {
        //     SetupResourceCounterUIHud(_resourceCounterUIHuds, hudData);
        // }

        // public void SetupHud(Transform targetTransfrom)
        // {
        //     var freeHud = _testUIHuds.FirstOrDefault(x=>x.IsFree);
        //
        //     if (ReferenceEquals(freeHud, null))
        //     {
        //         DebugSafe.LogException(new System.Exception("No free in game UI"));
        //     }
        //     else
        //     {
        //         freeHud.Setup(targetTransfrom);
        //     }
        // }
    }
}