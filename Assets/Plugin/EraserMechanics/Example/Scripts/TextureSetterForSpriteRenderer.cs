namespace Plugin.EraserMechanics.Example.Scripts
{
    using Plugin.EraserMechanics.Core.Scripts;
    using UnityEngine;

    public sealed class TextureSetterForSpriteRenderer : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        
        [SerializeField]
        private ErasablePlane plane;

        private void Start()
        {
            if (plane.IsInited == false)
            {
                plane.OnInited += SetSprite;
                
                return;
            }
            
            SetSprite();
        }

        private void SetSprite()
        {
            plane.OnInited -= SetSprite;
            
            spriteRenderer.sprite = Sprite.Create
            (
                plane.ErasedTexture,
                new Rect(0, 0, plane.ErasedTexture.width, plane.ErasedTexture.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }
}
