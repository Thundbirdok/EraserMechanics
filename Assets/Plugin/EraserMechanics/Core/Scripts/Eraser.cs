namespace Plugin.EraserMechanics.Core.Scripts
{
    using System;
    using Unity.Burst;
    using UnityEngine;

    [BurstCompile]
    [Serializable]
    public sealed class Eraser
    {
        public Texture2D ErasedTexture { get; private set; }

        [SerializeField]
        private Texture2D erasedTextureSample;

        [SerializeField]
        private Color eraseColor = new Color(0, 0, 0, 0);

        [SerializeField]
        private int brushRadius = 5;

        private bool _isInited;

        private Color[] _pixels;
        
        [BurstCompile]
        public void Init()
        {
            if (_isInited)
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

            _pixels = erasedTextureSample.GetPixels();
            
            Apply();
            
            _isInited = true;
        }
        
        [BurstCompile]
        public void EraseInPoint(Vector2 point)
        {
            GetPointInPixels(point, out var x, out var y);

            EraseInPixel(x, y);
            
            ErasedTexture.SetPixels(_pixels);
            
            ErasedTexture.Apply();
        }

        [BurstCompile]
        public void EraseLine(Vector2 pointA, Vector2 pointB)
        {
            GetPointInPixels(pointA, out var xA, out var yA);
            GetPointInPixels(pointB, out var xB, out var yB);

            var xDistance = xB - xA;
            var yDistance = yB - yA;

            var distance = Mathf.Sqrt(xDistance * xDistance + yDistance * yDistance);

            if (distance == 0)
            {
                EraseInPixel(xA, yA);

                Apply();
            }

            for (var i = 1; i <= distance; ++i)
            {
                var t = i / distance;

                var x = Mathf.FloorToInt(xA + xDistance * t);
                var y = Mathf.FloorToInt(yA + yDistance * t);
                
                EraseInPixel(x, y);
            }

            Apply();
        }

        [BurstCompile]
        private void EraseInPixel(int x, int y)
        {
            for (int i = -brushRadius; i < brushRadius; ++i)
            {
                for (int j = -brushRadius; j < brushRadius; ++j)
                {
                    var distance = Mathf.Sqrt(i * i + j * j);
                    
                    if (distance <= brushRadius)
                    {
                        ErasePixel(x + j, y + i);
                    }
                }
            }
        }

        [BurstCompile]
        private void GetPointInPixels(Vector2 point, out int x, out int y)
        {
            x = Mathf.FloorToInt(point.x * ErasedTexture.width + (float)ErasedTexture.width / 2);
            y = Mathf.FloorToInt(point.y * ErasedTexture.height + (float)ErasedTexture.height / 2);
        }

        [BurstCompile]
        private void ErasePixel(int x, int y)
        {
            if (x >= ErasedTexture.width || x < 0)
            {
                return;
            }
            
            if (y >= ErasedTexture.height || y < 0)
            {
                return;
            }

            _pixels[y * ErasedTexture.width + x] = eraseColor;
        }

        [BurstCompile]
        private void Apply()
        {
            ErasedTexture.SetPixels(_pixels);

            ErasedTexture.Apply();
        }
    }
}
