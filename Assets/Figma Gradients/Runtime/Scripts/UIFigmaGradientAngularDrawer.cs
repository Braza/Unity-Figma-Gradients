﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nox7atra.UIFigmaGradients
{
    public class UIFigmaGradientAngularDrawer : UIFigmaGradientLinearDrawer
    {
        [SerializeField] protected Vector2 _Center;
        
        [SerializeField] 
        private Shader _angularGradientShader;

        protected override Material GradientMaterial
        {
            get
            {
                // Ensure the shader is assigned
                if (_angularGradientShader == null)
                {
                    _angularGradientShader = Shader.Find("UI/AngularGradientShader");

                    if (_angularGradientShader == null)
                    {
                        Debug.LogWarning("Shader 'UI/AngularGradientShader' not found! Please ensure it exists.");
                        return null;
                    }

                    // Mark for serialization in the editor
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
                }

                // Cache the material for efficiency
                if (_cachedGradientMaterial == null)
                {
                    _cachedGradientMaterial = new Material(_angularGradientShader);
                }

                return _cachedGradientMaterial;
            }
        }

        protected override TextureWrapMode WrapMode => TextureWrapMode.Repeat;

        
        protected override void GenerateHelperUvs(VertexHelper vh)
        {
            UIVertex vert = new UIVertex();
            for (int i = 0; i < vh.currentVertCount; i++) {
                vh.PopulateUIVertex(ref vert, i);
                vert.normal = new Vector3(_Center.x, 1 - _Center.y, _Angle);
                vh.SetUIVertex(vert, i);
            }
        }
        [EditorButton]
        public void ProcessGradient()
        {
            var colorKeys = _Gradient.colorKeys;
            var alphaKeys = _Gradient.alphaKeys;
            var lastColorKey = _Gradient.colorKeys[colorKeys.Length - 1];
            if (lastColorKey.time < 1)
            {
                var newColorKeys = new GradientColorKey[colorKeys.Length + 1];
                var newAlphaKeys = new GradientAlphaKey[alphaKeys.Length + 1];
                for (int i = 0; i < colorKeys.Length; i++)
                {
                    newColorKeys[i] = colorKeys[i];
                }

                for (int i = 0; i < alphaKeys.Length; i++)
                {
                    newAlphaKeys[i] = alphaKeys[i];
                }
                newColorKeys[newColorKeys.Length - 1] = colorKeys[0];
                newAlphaKeys[newAlphaKeys.Length - 1] = alphaKeys[0];
                newColorKeys[newColorKeys.Length - 1].time = 1;
                newAlphaKeys[newAlphaKeys.Length - 1].time = 1;
                _Gradient.colorKeys = newColorKeys;
                _Gradient.alphaKeys = newAlphaKeys;
                Refresh();
            }
        }
    }
}