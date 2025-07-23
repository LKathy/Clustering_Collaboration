using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TitleBG : MonoBehaviour
{
    [Header("渐变方向")]
    public GradientDirection direction = GradientDirection.Horizontal;

    [Header("颜色设置")]
    public Color startColor = Color.white;
    public Color endColor = Color.black;
    [Tooltip("渐变过渡的平滑度")]
    [Range(0.1f, 10f)] public float gradientSmoothness = 1f;

    private Image _targetImage;
    private RectTransform _rectTransform;

    void Start()
    {
        _targetImage = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();

        // 初始化为纯色
        _targetImage.type = Image.Type.Simple;
        _targetImage.preserveAspect = false;

        // 创建渐变纹理
        UpdateGradientTexture();
    }

    void UpdateGradientTexture()
    {
        // 根据方向确定纹理尺寸
        int width = direction == GradientDirection.Horizontal ? 256 : 1;
        int height = direction == GradientDirection.Vertical ? 256 : 1;

        Texture2D texture = new Texture2D(width, height);
        texture.wrapMode = TextureWrapMode.Clamp;

        // 填充渐变像素
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float t = direction == GradientDirection.Horizontal ?
                    Mathf.Pow(x / (float)width, gradientSmoothness) :
                    Mathf.Pow(y / (float)height, gradientSmoothness);

                texture.SetPixel(x, y, Color.Lerp(startColor, endColor, t));
            }
        }
        texture.Apply();

        // 应用纹理
        _targetImage.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, width, height),
            Vector2.zero
        );
    }

    // 编辑器实时预览
    void OnValidate()
    {
        if (_targetImage != null) UpdateGradientTexture();
    }
}

public enum GradientDirection { Horizontal, Vertical }