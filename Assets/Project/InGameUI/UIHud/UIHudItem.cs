using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project
{
    public abstract class UIHudItem : MonoBehaviour
    {
        [SerializeField]
        protected Vector3 _offset = Vector3.zero;
        
        [SerializeField]
        private bool _isLerp = false;

        [SerializeField, EnabledIf(nameof(_isLerp), true, EnabledIfAttribute.HideMode.Invisible)]
        protected float _lerpSpeed = 1f;
         
        [SerializeField]
        protected TextMeshProUGUI _text = null;
        
        private Vector2 _expectedPosition = Vector2.zero;

        private RectTransform _transform = null;

        private CancellationTokenSource _cancellationTokenSource = null;
        
        private Camera _mainCamera = null;
        protected InGameUISystem _inGameUISystem = null;
        private UIBehaviour[] _allUI = null;
        
        
        public bool IsFree
        {
            get => !gameObject.activeSelf;
        }

        public bool IsVisible
        {
            get => _transform.IsVisibleFrom(_mainCamera);
        }

        protected abstract Vector3 TargetPosition
        {
            get;
        }
         
        protected override void Awake()
        {
            base.Awake();
            
            _transform = (RectTransform)transform;
            _allUI = gameObject.GetComponentsInChildren<UIBehaviour>();
        }

        public void Prepare(InGameUISystem inGameUISystem, Camera camera)
        {
            _mainCamera = camera;
            _inGameUISystem = inGameUISystem;
            
            Hide();
        }

        protected void SetupViewportPosition(Vector3 position)
        {
            _transform.SetViewportPosition(_inGameUISystem.GetViewportPosition(position + _offset));
        }

        protected void DisableVisual()
        {
            _allUI.Do(ui => ui.enabled = false);
        }

        protected void EnableVisual()
        {
            _allUI.Do(ui => ui.enabled = true);
        }

        protected void Hide()
        {
            gameObject.SetActive(false);
        }

        protected void Show()
        {
            gameObject.SetActive(true);
        }

        public void RefreshVisibility()
        {
            if (IsVisible)
            {
                EnableVisual();
            }
            else
            {
                DisableVisual();
            }
        }
        
        public virtual void Refresh(float scaleCoefficient)
        {
            if (ReferenceEquals(_inGameUISystem, null))
            {
                return;
            }
            
            var distanceToCamera = Vector3.Distance(_mainCamera.transform.position, TargetPosition);

            var scale = 1 / distanceToCamera * scaleCoefficient;
            _transform.localScale = Vector3.one * scale;
            
            _expectedPosition = _inGameUISystem.GetViewportPosition(TargetPosition + _offset);

            var endPosition = _isLerp
                ? Vector2.Lerp(_transform.anchoredPosition, _expectedPosition, _lerpSpeed * Time.deltaTime)
                : _expectedPosition;
            
            _transform.SetViewportPosition(endPosition);
        }
    }
}