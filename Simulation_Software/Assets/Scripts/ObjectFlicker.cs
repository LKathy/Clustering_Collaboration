using System.Collections;
using UnityEngine;

public class ObjectFlicker : MonoBehaviour
{
    public Renderer objectRenderer; // 物体的渲染器
    public float flickerInterval = 0.5f; // 闪烁间隔时间（秒）
    public Transform target; // 目标游戏物体的 Transform

    private bool isFlickering = false;

    void Start()
    {
        if (objectRenderer == null)
        {
            objectRenderer = GetComponent<Renderer>();
        }

        StartFlicker(); // 开始闪烁
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // 销毁当前游戏物体
            return;
        }

        FollowTarget(); // 每帧更新跟随逻辑
    }

    public void StartFlicker()
    {
        if (!isFlickering)
        {
            StartCoroutine(Flicker());
        }
    }

    private IEnumerator Flicker()
    {
        isFlickering = true;

        while (true) // 无限循环，直到物体被销毁
        {
            objectRenderer.enabled = !objectRenderer.enabled; // 切换渲染器的启用状态
            yield return new WaitForSeconds(flickerInterval); // 等待指定的间隔时间
        }
    }

    private void FollowTarget()
    {
        if (target != null)
        {
            // 直接将物体的位置设置为目标的位置
            transform.position = target.position + new Vector3(0f, 5f, 0f);
        }
    }

    void OnDestroy()
    {
        // 确保在销毁时停止闪烁（协程会自动结束）
        objectRenderer.enabled = true; // 确保销毁前渲染器启用
    }
}
