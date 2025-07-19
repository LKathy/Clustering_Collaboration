using UnityEngine;

public class WaterMaterialSwitcher : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera; // ��ק����������������Ϊ����Ĭ��ʹ��Camera.main
    public Renderer waterRenderer; // ��ק���ˮ�������MeshRenderer���������

    [Header("Water Materials")]
    public Material waterSurfaceMaterial; // �����ˮ��ʱʹ�õĲ���
    public Material underwaterMaterial;   // �����ˮ��ʱʹ�õĲ���

    [Header("Water Level Settings")]
    public float waterLevelY = 0f; // ˮ�����������Yֵ

    private bool isUnderwater = false; // ���ٵ�ǰ״̬�������ظ����ò���

    void Start()
    {
        // ���û���ֶ�ָ����������Ի�ȡ�����
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("WaterMaterialSwitcher: û���ҵ�����������ֶ�ָ����ȷ����������ǩΪ 'MainCamera'��");
                enabled = false; // ���û����������ô˽ű�
                return;
            }
        }

        // ����Ƿ�ָ����ˮ����Ⱦ��
        if (waterRenderer == null)
        {
            Debug.LogError("WaterMaterialSwitcher: ˮ����Ⱦ��δָ��������קˮ�������MeshRenderer������ű��ϡ�");
            enabled = false; // ���û����Ⱦ�������ô˽ű�
            return;
        }

        // ��ʼ��鲢���ò���
        CheckWaterLevel();
    }

    void Update()
    {
        // ÿ֡������λ��
        CheckWaterLevel();
    }

    void CheckWaterLevel()
    {
        // ��ȡ��������������е�Y��λ��
        float cameraY = mainCamera.transform.position.y;

        // �ж�����Ƿ���ˮ�£�����ˮ����ƽ��
        if (cameraY <= waterLevelY)
        {
            // ��������ǰ��ˮ�£����л���ˮ�²���
            if (!isUnderwater)
            {
                waterRenderer.material = underwaterMaterial;
                isUnderwater = true;
                Debug.Log("�������ˮ�¡��л���ˮ�²��ʡ�");
            }
        }
        else // �����ˮ��
        {
            // ��������ǰ����ˮ�£����л���ˮ�ϲ���
            if (isUnderwater)
            {
                waterRenderer.material = waterSurfaceMaterial;
                isUnderwater = false;
                Debug.Log("�������ˮ�档�л���ˮ�ϲ��ʡ�");
            }
        }
    }
}

