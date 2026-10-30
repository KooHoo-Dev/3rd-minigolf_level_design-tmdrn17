namespace MiniGolf.KSG
{
    using UnityEngine;

    [RequireComponent(typeof(MeshRenderer))]
    public class GoalAngleIndicator : MonoBehaviour
    {
        [Header("내적 판정 설정")]
        [Range(-1f, 1f)]
        public float goalAllowDot = 0f;

        [Header("색상 설정")]
        public Color allowColor = Color.green; // 골 허용 범위 색상
        public Color disallowColor = Color.black; // 도망 범위 색상

        [Header("텍스처 해상도")]
        [SerializeField] private int textureResolution = 512;

        [ContextMenu("원형 인디케이터 생성 및 갱신")]
        public void GenerateIndicator()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            Texture2D texture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;

            float center = textureResolution * 0.5f;
            float radius = textureResolution * 0.5f;
            Color[] pixels = new Color[textureResolution * textureResolution];

            for (int y = 0; y < textureResolution; y++)
            {
                for (int x = 0; x < textureResolution; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    int index = y * textureResolution + x;

                    if (dist > radius)
                    {
                        pixels[index] = Color.clear;
                        continue;
                    }

                    if (dist == 0)
                    {
                        pixels[index] = allowColor;
                        continue;
                    }

                    // 2D 방향 벡터 계산
                    Vector2 dir = new Vector2(dx, dy) / dist;

                    // [수정 포인트] 기준 방향을 Vector2.up(Y축)에서 Vector2.right(X축/Red Arrow)로 변경
                    float dot = Vector2.Dot(Vector2.right, dir);

                    if (dot > goalAllowDot)
                    {
                        pixels[index] = disallowColor; // 도망 구역
                    }
                    else
                    {
                        pixels[index] = allowColor; // 골 허용 구역
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            Shader transparentShader = Shader.Find("Unlit/Transparent");
            if (transparentShader == null) transparentShader = Shader.Find("Sprites/Default");

            Material mat = new Material(transparentShader);
            mat.mainTexture = texture;
            meshRenderer.sharedMaterial = mat;
        }

        private void OnValidate()
        {
            GenerateIndicator();
        }
    }
}