using System;
using UnityEngine;

namespace Plugin.EraserMechanic.Core.Scripts
{
    using UnityEngine.EventSystems;
    
    public sealed class Pointer : MonoBehaviour
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
                LastPointerPosition = _pointerPosition;
                
                _pointerPosition = value;
            }
        }

        public Vector3 LastPointerPosition { get; private set; }
        
        public bool IsPointedLastFrame { get; private set; }

        [SerializeField]
        private Camera raycastCamera;

        private const float MAX_RAYCAST_DISTANCE = 50;

        private readonly RaycastHit[] _hits = new RaycastHit[5];
        
        private void Update()
        {
            if (TryGetPointerPosition() == false)
            {
                IsPointedLastFrame = false;
                
                return;
            }
            
            OnChangedPointerPosition?.Invoke();
            
            IsPointedLastFrame = true;
        }
        
        private bool TryGetPointerPosition()
        {
            if (Input.GetMouseButton(0) == false)
            {
                return false;
            }
            
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }

            return Raycast();
        }

        private bool Raycast()
        {
            var ray = raycastCamera.ScreenPointToRay(Input.mousePosition);

            var size = Physics.RaycastNonAlloc(ray, _hits, MAX_RAYCAST_DISTANCE);
            
            if (size == 0)
            {
                return false;
            }

            return GetHits(size);
        }
        
        private bool GetHits(in int size)
        {
            for (var i = 0; i < size; ++i)
            {
                if (_hits[i].collider.TryGetComponent(out ErasablePlane _) == false)
                {
                    continue;
                }

                PointerPosition = _hits[i].point;

                return true;
            }

            return false;
        }
    }
}
