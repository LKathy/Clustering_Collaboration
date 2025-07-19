using UnityEngine;

public class WaterMaterialSwitcher : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera; // 拖拽你的主相机到这里，如果为空则默认使用Camera.main
    public Renderer waterRenderer; // 拖拽你的水面网格的MeshRenderer组件到这里

    [Header("Water Materials")]
    public Material waterSurfaceMaterial; // 相机在水上时使用的材质
    public Material underwaterMaterial;   // 相机在水下时使用的材质

    [Header("Water Level Settings")]
    public float waterLevelY = 0f; // 水面的世界坐标Y值

    private bool isUnderwater = false; // 跟踪当前状态，避免重复设置材质

    void Start()
    {
        // 如果没有手动指定相机，尝试获取主相机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("WaterMaterialSwitcher: 没有找到主相机！请手动指定或确保你的相机标签为 'MainCamera'。");
                enabled = false; // 如果没有相机，禁用此脚本
                return;
            }
        }

        // 检查是否指定了水面渲染器
        if (waterRenderer == null)
        {
            Debug.LogError("WaterMaterialSwitcher: 水面渲染器未指定！请拖拽水面网格的MeshRenderer组件到脚本上。");
            enabled = false; // 如果没有渲染器，禁用此脚本
            return;
        }

        // 初始检查并设置材质
        CheckWaterLevel();
    }

    void Update()
    {
        // 每帧检查相机位置
        CheckWaterLevel();
    }

    void CheckWaterLevel()
    {
        // 获取相机在世界坐标中的Y轴位置
        float cameraY = mainCamera.transform.position.y;

        // 判断相机是否在水下（或与水面齐平）
        if (cameraY <= waterLevelY)
        {
            // 如果相机当前在水下，则切换到水下材质
            if (!isUnderwater)
            {
                waterRenderer.material = underwaterMaterial;
                isUnderwater = true;
                Debug.Log("相机进入水下。切换到水下材质。");
            }
        }
        else // 相机在水上
        {
            // 如果相机当前不在水下，则切换到水上材质
            if (isUnderwater)
            {
                waterRenderer.material = waterSurfaceMaterial;
                isUnderwater = false;
                Debug.Log("相机浮出水面。切换到水上材质。");
            }
        }
    }
}

