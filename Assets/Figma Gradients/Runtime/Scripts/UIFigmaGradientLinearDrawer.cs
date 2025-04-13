using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Nox7atra.UIFigmaGradients
{
   [RequireComponent(typeof(CanvasRenderer))]
   public class UIFigmaGradientLinearDrawer : MaskableGraphic
   { 
      [SerializeField]
      protected Gradient _Gradient = new Gradient();
      [SerializeField]
      private GradientResolution _GradientResolution = GradientResolution.k256;
    
      [SerializeField] 
      protected float _Angle = 180;
      
      [SerializeField] 
      private Shader _linearGradientShader;

      private Material _cachedGradientMaterial;


      private Texture2D _GradientTexture;
      protected virtual TextureWrapMode WrapMode => TextureWrapMode.Clamp;
      // Updated to return cached material with dynamically assigned shader
      protected virtual Material GradientMaterial
      {
         get
         {
            // Ensure the shader is assigned
            if (_linearGradientShader == null)
            {
               // Try to find the shader during runtime or edit-time
               _linearGradientShader = Shader.Find("UI/LinearGradientShader");

               if (_linearGradientShader == null)
               {
                  Debug.LogError("Shader 'UI/LinearGradientShader' not found! Please ensure it exists.");
                  return null;
               }

               // Mark the field for serialization in the editor
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
            }

            // Cache the material for efficiency
            if (_cachedGradientMaterial == null)
            {
               _cachedGradientMaterial = new Material(_linearGradientShader);
            }

            return _cachedGradientMaterial;
         }
      }

      public override Texture mainTexture => _GradientTexture;
#if UNITY_EDITOR
      protected override void OnValidate()
      {
         base.OnValidate();
         Refresh();
      }
#endif
      protected override void Awake()
      {
         base.Awake();
         Refresh();
      }
      
      public void SetGradient(Gradient gradient)
      {
         _Gradient = gradient;
         Refresh();
      }

      public Texture2D GenerateTexture(bool makeNoLongerReadable = false)
      {
         Texture2D tex = new Texture2D(1, (int)_GradientResolution, TextureFormat.ARGB32, false, true);
         tex.wrapMode = WrapMode;
         tex.filterMode = FilterMode.Bilinear;
         tex.anisoLevel = 1;
         Color[] colors = new Color[(int)_GradientResolution];
         float div = (float)(int)_GradientResolution;
         for (int i = 0; i < (int)_GradientResolution; ++i)
         {
            float t = (float)i/div;
            colors[i] = _Gradient.Evaluate(t);
         }
         tex.SetPixels(colors);
         tex.Apply(false, makeNoLongerReadable);
         
         return tex;
      }
      
      public void Refresh()
      {
         if (_GradientTexture != null)
         {
            DestroyImmediate(_GradientTexture);
         }

         material = GradientMaterial;
         _GradientTexture = GenerateTexture();
      }

      protected override void OnDestroy()
      {
         base.OnDestroy();
         if (_GradientTexture != null)
         {
            DestroyImmediate(_GradientTexture);
         }
      }

      protected virtual void GenerateHelperUvs(VertexHelper vh)
      {
         UIVertex vert = new UIVertex();
         for (int i = 0; i < vh.currentVertCount; i++) {
            vh.PopulateUIVertex(ref vert, i);
            vert.uv1 = new Vector2(_Angle, _Angle);          
            vh.SetUIVertex(vert, i);
         }
      }
      protected override void OnPopulateMesh(VertexHelper vh)
      {
         base.OnPopulateMesh(vh);
         GenerateHelperUvs(vh);
      }

      public virtual void ParseCss(string css)
      {
         var parameters = UIFigmaGradientTools.ParseLinearCssParams(css);
         var angle = parameters[0].Trim().Replace("deg", "");
         _Angle = float.Parse(angle, NumberStyles.Any, CultureInfo.InvariantCulture);
         List<GradientColorKey> colorKeys = new List<GradientColorKey>();
         List<GradientAlphaKey> alphaKeys = new List<GradientAlphaKey>();
         for (int i = 1; i < parameters.Count; i++)
         {
            float time = 0;
            var col = UIFigmaGradientTools.ParseColor(parameters[i], out time);
            var colorKey = new GradientColorKey();
            colorKey.color = col;
            colorKey.time = time;
            var alphaKey = new GradientAlphaKey();
            alphaKey.alpha = col.a;
            alphaKey.time = time;
            colorKeys.Add(colorKey);
            alphaKeys.Add(alphaKey);
         }
         _Gradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
         Refresh();
      }
      
   }
}