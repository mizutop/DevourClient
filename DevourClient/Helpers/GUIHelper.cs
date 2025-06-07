using UnityEngine;
using MelonLoader;
using System.Collections.Generic;

namespace DevourClient.Helpers
{
    public static class GUIHelper
    {
        private static Texture2D previewTexture;
        private static GUIStyle boxStyle;
        
        private static Dictionary<Color, Texture2D> colorTextureCache = new Dictionary<Color, Texture2D>();
        private static Dictionary<int, Texture2D> circularTextureCache = new Dictionary<int, Texture2D>();
        

        public static void Initialize()
        {
            if (previewTexture == null)
            {
                previewTexture = new Texture2D(1, 1);
                boxStyle = new GUIStyle(GUI.skin.box);
            }
        }


        public static void Cleanup()
        {
            if (previewTexture != null)
            {
                UnityEngine.Object.Destroy(previewTexture);
                previewTexture = null;
            }

            foreach (var texture in colorTextureCache.Values)
            {
                if (texture != null)
                {
                    UnityEngine.Object.Destroy(texture);
                }
            }
            colorTextureCache.Clear();

            foreach (var texture in circularTextureCache.Values)
            {
                if (texture != null)
                {
                    UnityEngine.Object.Destroy(texture);
                }
            }
            circularTextureCache.Clear();
        }

        private static float R;
        private static float G;
        private static float B;

        public static Color ColorPick(string title, Color color)
        {
            Initialize();

            GUILayout.BeginArea(new Rect(Settings.Settings.x + 195, Settings.Settings.y + 70, 250, 250));
            
            GUILayout.Label(title);
            GUILayout.Space(10);


            GUILayout.BeginHorizontal();
            GUILayout.Label("R", GUILayout.Width(20));
            R = GUILayout.HorizontalSlider(color.r, 0f, 1f, GUILayout.Width(150));
            GUILayout.Label(((int)(R * 255)).ToString(), GUILayout.Width(30));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("G", GUILayout.Width(20));
            G = GUILayout.HorizontalSlider(color.g, 0f, 1f, GUILayout.Width(150));
            GUILayout.Label(((int)(G * 255)).ToString(), GUILayout.Width(30));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("B", GUILayout.Width(20));
            B = GUILayout.HorizontalSlider(color.b, 0f, 1f, GUILayout.Width(150));
            GUILayout.Label(((int)(B * 255)).ToString(), GUILayout.Width(30));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);


            void DrawPreview(Color color_to_draw)
            {
                if (previewTexture == null)
                {
                    previewTexture = new Texture2D(1, 1);
                }
                
                previewTexture.SetPixel(0, 0, color_to_draw);
                previewTexture.Apply();
                boxStyle.normal.background = previewTexture;
                GUILayout.Box(GUIContent.none, boxStyle, GUILayout.Height(30));
            }

            DrawPreview(new Color(R, G, B, 1));
            
            GUILayout.EndArea();

            return new Color(R, G, B, 1);
        }
        
        public static Texture2D MakeTex(int width, int height, Color col)
        {
            string cacheKey = $"{width}x{height}_{col.r}_{col.g}_{col.b}";
            
            if (colorTextureCache.TryGetValue(col, out Texture2D cachedTexture))
            {
                return cachedTexture;
            }

            Texture2D result = new Texture2D(width, height);
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            result.SetPixels(pix);
            result.Apply();
            
            colorTextureCache[col] = result;
            return result;
        }
        
        public static Texture2D GetCircularTexture(int width, int height)
        {
            int size = Mathf.Max(width, height);
            if (circularTextureCache.TryGetValue(size, out Texture2D cachedTexture))
            {
                return cachedTexture;
            }

            Texture2D texture = new Texture2D(width, height);
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    if (Vector2.Distance(new Vector2(x, y), new Vector2(texture.width / 2, texture.height / 2)) <= texture.width / 2)
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }

            texture.Apply();
            
            circularTextureCache[size] = texture;
            return texture;
        }
    }
}
