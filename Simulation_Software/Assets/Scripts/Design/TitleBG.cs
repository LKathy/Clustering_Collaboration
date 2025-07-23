using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TitleBG : MonoBehaviour
{
    [Header("���䷽��")]
    public GradientDirection direction = GradientDirection.Horizontal;

    [Header("��ɫ����")]
    public Color startColor = Color.white;
    public Color endColor = Color.black;
    [Tooltip("������ɵ�ƽ����")]
    [Range(0.1f, 10f)] public float gradientSmoothness = 1f;

    private Image _targetImage;
    private RectTransform _rectTransform;

    void Start()
    {
        _targetImage = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();

        // ��ʼ��Ϊ��ɫ
        _targetImage.type = Image.Type.Simple;
        _targetImage.preserveAspect = false;

        // ������������
        UpdateGradientTexture();
    }

    void UpdateGradientTexture()
    {
        // ���ݷ���ȷ������ߴ�
        int width = direction == GradientDirection.Horizontal ? 256 : 1;
        int height = direction == GradientDirection.Vertical ? 256 : 1;

        Texture2D texture = new Texture2D(width, height);
        texture.wrapMode = TextureWrapMode.Clamp;

        // ��佥������
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

        // Ӧ������
        _targetImage.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, width, height),
            Vector2.zero
        );
    }

    // �༭��ʵʱԤ��
    void OnValidate()
    {
        if (_targetImage != null) UpdateGradientTexture();
    }
}

public enum GradientDirection { Horizontal, Vertical }