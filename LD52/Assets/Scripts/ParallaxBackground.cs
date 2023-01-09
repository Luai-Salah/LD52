using System;
using UnityEngine;

namespace LD52
{
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private Vector2 m_ParallaxEffectMultiplier = new (0.5f, 0.5f);
        
        private Transform m_CameraTransform;
        private Transform m_Transform;

        private Vector3 m_LastCameraPosition;
        private float m_TextureUnitSizeX;
        
        private void Start()
        {
            Camera mainCamera = Camera.main;
            Debug.Assert(mainCamera, "Main Camera not available", gameObject);

            m_Transform = transform;

            m_CameraTransform = mainCamera.transform;
            m_LastCameraPosition = m_CameraTransform.position;

            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            m_TextureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        }

        private void LateUpdate()
        {
            Vector3 cameraPosition = m_CameraTransform.position;
            
            Vector3 deltaMovement = cameraPosition - m_LastCameraPosition;
            Vector2 value = m_ParallaxEffectMultiplier * deltaMovement;

            m_Transform.position += new Vector3(value.x, value.y, 0.0f);
            m_LastCameraPosition = cameraPosition;

            if (!(Mathf.Abs(cameraPosition.x - m_Transform.position.x) >= m_TextureUnitSizeX))
                return;
            
            Vector3 position = m_Transform.position;
            float offsetPositionX = (cameraPosition.x - position.x) % m_TextureUnitSizeX;
            m_Transform.position = new Vector3(cameraPosition.x + offsetPositionX, position.y);
        }
    }
}