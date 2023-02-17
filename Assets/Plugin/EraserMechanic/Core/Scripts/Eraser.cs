namespace Plugin.EraserMechanic.Core.Scripts
{
    using System;
    using UnityEngine;

    [Serializable]
    public sealed class Eraser
    {
        public event Action OnInited;
        
        public bool IsInited { get; private set; } = false;
        
        public Texture2D ErasedTexture { get; private set; }

        [SerializeField]
        private Texture2D erasedTextureSample;

        [SerializeField]
        private Color eraseColor = new Color(0, 0, 0, 0);

        [SerializeField]
        private int brushRadius = 5;

        public void Init()
        {
            if (IsInited)
            {
                return;
            }
            
            ErasedTexture = new Texture2D
            (
                erasedTextureSample.width, 
                erasedTextureSample.height, 
                TextureFormat.RGBA32,
                true
            );

            var pixels = erasedTextureSample.GetPixels();
            ErasedTexture.SetPixels(pixels);

            ErasedTexture.Apply();
            
            IsInited = true;
            OnInited?.Invoke();
        }
        
        public void EraseInPoint(Vector2 point)
        {
            var x = Mathf.FloorToInt(point.x * ErasedTexture.width + (float)ErasedTexture.width / 2);
            var y = Mathf.FloorToInt(point.y * ErasedTexture.height + (float)ErasedTexture.height / 2);
            
            var pointInPixels = new Vector2Int(x, y);
            
            EraseInPixel(pointInPixels);
        }
        
        public void EraseInPixel(Vector2Int pointInPixels)
        {
            for (int i = -brushRadius; i < brushRadius; ++i)
            {
                for (int j = -brushRadius; j < brushRadius; ++j)
                {
                    var dist = Mathf.Sqrt(i * i + j * j);
                    
                    if (dist <= brushRadius)
                    {
                        ErasePixel(pointInPixels + new Vector2Int(j, i));
                    }
                }
            }
            
            ErasedTexture.Apply();
        }
        
        private void ErasePixel(Vector2Int point)
        {
            if (point.x > ErasedTexture.width || point.x < 0)
            {
                return;
            }
            
            if (point.y > ErasedTexture.height || point.y < 0)
            {
                return;
            }
            
            ErasedTexture.SetPixel(point.x, point.y, eraseColor);
        }
    }
}
