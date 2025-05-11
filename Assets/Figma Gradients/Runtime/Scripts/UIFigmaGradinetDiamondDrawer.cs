using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nox7atra.UIFigmaGradients
{
    public class UIFigmaGradinetDiamondDrawer : UIFigmaGradientRadialDrawer
    {
        [SerializeField]
        private Shader _diamondGradientShader;

        protected override Material GradientMaterial
        {
            get
            {
                // Ensure the shader is assigned
                if (_diamondGradientShader == null)
                {
                    _diamondGradientShader = Shader.Find("UI/DiamondGradientShader");

                    if (_diamondGradientShader == null)
                    {
                        Debug.LogError("Shader 'UI/DiamondGradientShader' not found! Please ensure it exists.");
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
                    _cachedGradientMaterial = new Material(_diamondGradientShader);
                }

                return _cachedGradientMaterial;
            }
        }

        private void OnValidate()
        {
            // Automatically assign the shader when the script is validated
            if (_diamondGradientShader == null)
            {
                _diamondGradientShader = Shader.Find("UI/DiamondGradientShader");

                if (_diamondGradientShader == null)
                {
                    Debug.LogWarning("Shader 'UI/DiamondGradientShader' could not be found during validation.");
                }
                else
                {
                    // Ensure Unity serializes the changes
                    #if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
                    #endif
                }
            }

            // Clear cached material if the shader changes, allowing recreation
            if (_cachedGradientMaterial != null 
                && (_cachedGradientMaterial.shader != _diamondGradientShader))
            {
                _cachedGradientMaterial = null;
            }
        }

    }
}