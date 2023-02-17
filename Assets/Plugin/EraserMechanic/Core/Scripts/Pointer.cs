using System;
using UnityEngine;

namespace Plugin.EraserMechanic.Core.Scripts
{
    using UnityEngine.EventSystems;

    [Serializable]
    public sealed class Pointer
    {
        public event Action OnChangedPointerPosition;

        private Vector3 _pointerPosition;
        public Vector3 PointerPosition
        {
            get
            {
                return _pointerPosition;
            }

            private set
            {
                if (_pointerPosition == value)
                {
                    return;
                }

                _pointerPosition = value;
                
                OnChangedPointerPosition?.Invoke();
            }
        }
        
        [SerializeField]
        private Camera raycastCamera;
        
        private const float MAX_RAYCAST_DISTANCE = 50;
        
        private readonly RaycastHit[] _hits = new RaycastHit[5];

        public void CheckPointerPosition()
        {
            if (Input.GetMouseButton(0) == false)
            {
                return;
            }
            
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Raycast();
        }

        private void Raycast()
        {
            var ray = raycastCamera.ScreenPointToRay(Input.mousePosition);

            var size = Physics.RaycastNonAlloc(ray, _hits, MAX_RAYCAST_DISTANCE);
            
            if (size == 0)
            {
                return;
            }

            GetHits(size);
        }
        
        private void GetHits(in int size)
        {
            for (var i = 0; i < size; ++i)
            {
                if (_hits[i].collider.TryGetComponent(out ErasablePlane _))
                {
                    PointerPosition = _hits[i].point;
                }
            }
        }
    }
}
