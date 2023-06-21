using UnityEngine;

namespace Project
{
    public class TestUIHud : UIHudItemBase
    {
        private Transform _targetTransfrom;

        protected override Vector3 TargetPosition
        {
            get => _targetTransfrom.position;
        }

        public void Setup(Transform targetTransfrom)
        {
            _targetTransfrom = targetTransfrom;
            
            SetupViewportPosition(TargetPosition);
            
            Show();
        }
    }
}