namespace Plugin.EraserMechanics.Core.Scripts
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

    public sealed class ErasablePlane : MonoBehaviour
    {
        public event Action OnInited;
        
        public bool IsInited { get; private set; }

        public Texture2D ErasedTexture => eraser.ErasedTexture;
        
        [SerializeField]
        private Eraser eraser;

        [FormerlySerializedAs("pointer")] [SerializeField]
        private EraserPointer eraserPointer;

        private void OnEnable()
        {
            Init();

            eraserPointer.OnChangedPointerPosition += Erase;
        }

        private void OnDisable()
        {
            eraserPointer.OnChangedPointerPosition -= Erase;
        }

        private void Init()
        {
            if (IsInited)
            {
                return;
            }

            eraser.Init();

            IsInited = true;
            OnInited?.Invoke();
        }

        private void Erase()
        {
            var localPoint = transform.InverseTransformPoint(eraserPointer.PointerPosition);
            
            if (eraserPointer.IsPointedLastFrame)
            {
                var lastLocalPoint = transform.InverseTransformPoint(eraserPointer.LastPointerPosition);

                eraser.EraseLine(lastLocalPoint, localPoint);
                
                return;
            }
            
            eraser.EraseInPoint(localPoint);
        }
    }
}
