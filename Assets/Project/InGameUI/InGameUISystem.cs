using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project.UI;
using UnityEngine;
using Zenject;

namespace Project
{
    public class InGameUISystem : MonoBehaviour
    {
        public const float ScaleCoefficient = 45f;
        private const int RecyclersResourceHudCount = 15;
        private const int TutorialArrowsCount = 5;
        private const int PlaceUIHudCount = 10;
        private const int UIResourceMaxSignCount = 4;
        private const int ServiceClientHudCount = 1;

        [SerializeField]
        private ResourceCounterUIHud _resourceCounterUIHudPrefab = null;

        [SerializeField]
        private PlaceUIHud _placeUIHudPrefab = null;

        [SerializeField]
        private ComplexResourceCounterUIHud _complexResourceCounterUIHudPrefab = null;

        [SerializeField]
        private UINextMarketHud _uiNextMarketUIHud = null;

        [SerializeField]
        private UIResourceMaxSign _uiResourceMaxSignPrefab = null;

        [SerializeField]
        private UITutorialArrow _uiTutorialArrowPrefab = null;

        [SerializeField]
        private UIServiceClientHud _uiServiceClientPrefab = null;

        private Camera _camera = null;
        private DiContainer _diContainer = null;

        private ResourceCounterUIHud[] _resourceCounterUIHuds = null;
        private ComplexResourceCounterUIHud[] _complexResourceCounterUIHuds = null;
        private UIServiceClientHud[] _uiServiceClientHuds = null;
        private PlaceUIHud[] _placeUIHuds = null;
        private UIResourceMaxSign[] _uiResourceMaxSign = null;
        private UITutorialArrow[] _uiTutorialArrows = null;

        private CancellationTokenSource _refreshCancellationTokenSource = null;

        [Inject]
        private void Construct(CameraController cameraController,
            DiContainer diContainer)
        {
            _camera = cameraController.Camera;
            _customerSpawnSettings = customerSpawnSettings;
            _diContainer = diContainer;
        }

        protected void Awake()
        {
            Prepare();
        }
        
        private void Start()
        {
            RefreshUILoopAsync(RefreshToken(ref _refreshCancellationTokenSource)).Forget();
        }

        private void Prepare()
        {
            _uiNextMarketUIHud.Prepare(this, _camera);

            _resourceCounterUIHuds = new ResourceCounterUIHud[_customerSpawnSettings.MinAndMaxCountOnScene.Max];
            _complexResourceCounterUIHuds = new ComplexResourceCounterUIHud[RecyclersResourceHudCount];
            _placeUIHuds = new PlaceUIHud[PlaceUIHudCount];
            _uiResourceMaxSign = new UIResourceMaxSign[UIResourceMaxSignCount];
            _uiTutorialArrows = new UITutorialArrow[TutorialArrowsCount];
            _uiServiceClientHuds = new UIServiceClientHud[ServiceClientHudCount];

            PrepareHuds(_uiServiceClientHuds, _uiServiceClientPrefab);
            PrepareHuds(_uiResourceMaxSign, _uiResourceMaxSignPrefab);
            PrepareHuds(_resourceCounterUIHuds, _resourceCounterUIHudPrefab);
            PrepareHuds(_complexResourceCounterUIHuds, _complexResourceCounterUIHudPrefab);
            PrepareHuds(_placeUIHuds, _placeUIHudPrefab);
            PrepareHuds(_uiTutorialArrows, _uiTutorialArrowPrefab);
        }

        private void PrepareHuds(UIHudItem[] uiHudItems, UIHudItem uiHudItemPrefab)
        {
            for (int i = 0; i < uiHudItems.Length; i++)
            {
                uiHudItems[i] = _diContainer.InstantiatePrefabForComponent<UIHudItem>(uiHudItemPrefab, transform);
                uiHudItems[i].Prepare(this, _camera);
            }
        }

        private void Update()
        {
            RefreshVisibility(_resourceCounterUIHuds);
            RefreshVisibility(_complexResourceCounterUIHuds);
            RefreshVisibility(_placeUIHuds);
            RefreshVisibility(_uiResourceMaxSign);
            RefreshVisibility(_uiTutorialArrows);
            RefreshVisibility(_uiServiceClientHuds);

            if (!_uiNextMarketUIHud.IsFree)
            {
                _uiNextMarketUIHud.RefreshVisibility();
            }
        }

        private async UniTaskVoid RefreshUILoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!ReferenceEquals(_refreshCancellationTokenSource, null))
                {
                    RefreshHuds(_resourceCounterUIHuds);
                    RefreshHuds(_complexResourceCounterUIHuds);
                    RefreshHuds(_placeUIHuds);
                    RefreshHuds(_uiResourceMaxSign);
                    RefreshHuds(_uiTutorialArrows);
                    RefreshHuds(_uiServiceClientHuds);

                    if (!_uiNextMarketUIHud.IsFree)
                    {
                        _uiNextMarketUIHud.Refresh(ScaleCoefficient);
                    }

                    await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, cancellationToken);
                }
            }
            catch (OperationCanceledException e)
            {
            }
        }

        private void RefreshVisibility(UIHudItem[] uiHudItems)
        {
            for (int i = 0; i < uiHudItems.Length; i++)
            {
                if (!uiHudItems[i].IsFree)
                {
                    uiHudItems[i].RefreshVisibility();
                }
            }
        }

        private void RefreshHuds(UIHudItem[] uiHudItems)
        {
            for (int i = 0; i < uiHudItems.Length; i++)
            {
                if (!uiHudItems[i].IsFree)
                {
                    uiHudItems[i].Refresh(ScaleCoefficient);
                }
            }
        }

        public Vector2 GetViewportPosition(Vector3 worldPosition)
        {
            return _camera.WorldToViewportPoint(worldPosition);
        }

        private void SetupResourceCounterUIHud(ResourceCounterUIHud[] resourceCounterUIHuds, UIHudData uiHudData)
        {
            var resourceCounter = resourceCounterUIHuds.FirstOrDefault(h => h.IsFree);

            if (!ReferenceEquals(resourceCounter, null))
            {
                resourceCounter.Setup(uiHudData);
            }
            else
            {
                DebugSafe.LogException(new Exception("No free in game UI"));
            }
        }

        private void SetupMultipleResourceCounterUIHud(ComplexResourceCounterUIHud[] complexResourceCounterUIHuds,
            UIHudData[] uiHudDatas, UIHudData complexUIHudData)
        {
            var complexResourceCounter = complexResourceCounterUIHuds.FirstOrDefault(h => h.IsFree);

            if (!ReferenceEquals(complexResourceCounter, null))
            {
                complexResourceCounter.Setup(complexUIHudData, uiHudDatas);
            }
            else
            {
                DebugSafe.LogException(new Exception("No free in game UI Multiple Resource Counter"));
            }
        }

        private void SetupResourceMaxSing(UIResourceMaxSign[] uiResourceMaxSigns, UIHudData uiHudData)
        {
            var uiResourceMaxSign = uiResourceMaxSigns.FirstOrDefault(h => h.IsFree);

            if (!ReferenceEquals(uiResourceMaxSign, null))
            {
                uiResourceMaxSign.Setup(uiHudData);
            }
            else
            {
                DebugSafe.LogException(new Exception("No free in game UI Resource Max Sign"));
            }
        }

        private void SetupPlaceUIHud(PlaceUIHud[] placeUIHuds, UIHudData placeHudData)
        {
            var freePlaceUIHud = placeUIHuds.FirstOrDefault(h => h.IsFree);

            if (!ReferenceEquals(freePlaceUIHud, null))
            {
                freePlaceUIHud.Setup(placeHudData);
            }
            else
            {
                DebugSafe.LogException(new Exception("No free in game UI Place Hud"));
            }
        }

        private void SetupUITutorialArrow(UITutorialArrow[] uiTutorialArrows, UIHudData placeHudData)
        {
            var uiTutorialArrow = uiTutorialArrows.FirstOrDefault(h => h.IsFree);

            if (!ReferenceEquals(uiTutorialArrow, null))
            {
                uiTutorialArrow.Setup(placeHudData);
            }
            else
            {
                DebugSafe.LogException(new Exception("No free in game UI Tutorial Arrow"));
            }
        }

        private void SetupClientServiceHud(Transform targetPosition, float serviceTime)
        {
            var uiServiceClientHud = _uiServiceClientHuds.FirstOrDefault(x => x.IsFree);

            if (!ReferenceEquals(uiServiceClientHud, null))
            {
                uiServiceClientHud.Setup(targetPosition, serviceTime);
            }
            else
            {
                DebugSafe.LogException(new Exception("No free in game UI Tutorial Arrow"));
            }
        }

        public void SetupNewMarketHud(Transform targetTransform, Func<float> fillValue)
        {
            _uiNextMarketUIHud.ToggleActive(true);
            _uiNextMarketUIHud.Setup(targetTransform, fillValue);
        }

        public void OnStartClientService(Transform targetPosition, float serviceTime)
        {
            SetupClientServiceHud(targetPosition, serviceTime);
        }

        private void RewardResourceController_CarOfferMade(UIHudData hudData)
        {
            SetupResourceCounterUIHud(_resourceCounterUIHuds, hudData);
        }

        private void Customer_ResourceListAdded(UIHudData hudData)
        {
            SetupResourceCounterUIHud(_resourceCounterUIHuds, hudData);
        }

        private void SpawnPlace_EntityPlaced(UIHudData placeHudData)
        {
            SetupPlaceUIHud(_placeUIHuds, placeHudData);
        }

        private void RecyclingResourceBuilding_Spawned(UIHudData complexUIHudData, UIHudData[] hudsData)
        {
            SetupMultipleResourceCounterUIHud(_complexResourceCounterUIHuds, hudsData, complexUIHudData);
        }

        private void PlayerController_ReachedResourceLimit(UIHudData hudData)
        {
            SetupResourceMaxSing(_uiResourceMaxSign, hudData);
        }

        private void Employee_ReachedResourceLimit(UIHudData hudData)
        {
            SetupResourceMaxSing(_uiResourceMaxSign, hudData);
        }


        private void ResourceCar_Spawned(UIHudData complexUIHudData, UIHudData[] hudsData)
        {
            SetupMultipleResourceCounterUIHud(_complexResourceCounterUIHuds, hudsData, complexUIHudData);
        }

        private void TutorialQueue_TutorialItemInitialized(UIHudData hudData)
        {
            SetupUITutorialArrow(_uiTutorialArrows, hudData);
        }
    }
}