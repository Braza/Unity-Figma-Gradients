using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Nox7atra.UIFigmaGradients
{
    [ExecuteInEditMode] // Ensures this script also runs in the Editor
    public class UIFigmaGradientRadialDrawer : UIFigmaGradientLinearDrawer
    {
        // Serialized shader field, automatically filled if empty
        [SerializeField] private Shader _radialGradientShader;

        // Serialized gradient material, to cache the created material
        private Material _cachedGradientMaterial;

        [SerializeField] protected Vector2 _Center;
        [Range(0.01f, 10)]
        [SerializeField] protected float _Radius1 = 1;
        [Range(0.01f, 10)]
        [SerializeField] protected float _Radius2 = 1;
        // Override the Gradient Material, and ensure it's only created once
        protected override Material GradientMaterial
        {
            get
            {
                // Ensure the shader is assigned
                if (_radialGradientShader == null)
                {
                    // Find and assign the shader during runtime or edit-time
                    _radialGradientShader = Shader.Find("UI/RadialGradientShader");

                    // Warn if the shader could not be found
                    if (_radialGradientShader == null)
                    {
                        Debug.LogWarning("Shader 'UI/RadialGradientShader' not found! Please ensure it exists.");
                        return null;
                    }

                    // Mark the field as dirty in the editor (so changes are saved)
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
                }

                // Cache the material to avoid creating a new instance each time
                if (_cachedGradientMaterial == null)
                {
                    _cachedGradientMaterial = new Material(_radialGradientShader);
                }

                return _cachedGradientMaterial;
            }
        }


        protected override void GenerateHelperUvs(VertexHelper vh)
        {
            UIVertex vert = new UIVertex();
            for (int i = 0; i < vh.currentVertCount; i++) {
                vh.PopulateUIVertex(ref vert, i);
                vert.normal = new Vector3(_Radius1, _Radius2, _Angle);
                vert.uv1 = new Vector2(_Center.x, 1 - _Center.y);
                vh.SetUIVertex(vert, i);
            }
        }
    }
}