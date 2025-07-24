using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAndHide : MonoBehaviour
{
    [Header("UI组件设置")]
    [SerializeField] private GameObject targetUIObject; // 要控制的UI组件

    void Start()
    {
        // 如果没有指定UI对象，尝试查找Canvas下的第一个子对象
        if (targetUIObject == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null && canvas.transform.childCount > 0)
            {
                targetUIObject = canvas.transform.GetChild(0).gameObject;
            }
        }

        // 初始状态设为隐藏
        if (targetUIObject != null)
        {
            targetUIObject.SetActive(false);
        }
    }

    void Update()
    {
        // 检测按键输入
        HandleInput();
    }

    private void HandleInput()
    {
        // 按下空格键显示UI
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowUI();
        }

        // 按下ESC键隐藏UI
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideUI();
        }
    }

    /// <summary>
    /// 显示UI组件
    /// </summary>
    public void ShowUI()
    {
        if (targetUIObject != null)
        {
            targetUIObject.SetActive(true);
            Debug.Log("UI已显示");
        }
        else
        {
            Debug.LogWarning("目标UI对象未设置！");
        }
    }

    /// <summary>
    /// 隐藏UI组件
    /// </summary>
    public void HideUI()
    {
        if (targetUIObject != null)
        {
            targetUIObject.SetActive(false);
            Debug.Log("UI已隐藏");
        }
        else
        {
            Debug.LogWarning("目标UI对象未设置！");
        }
    }

    /// <summary>
    /// 切换UI显示状态
    /// </summary>
    public void ToggleUI()
    {
        if (targetUIObject != null)
        {
            bool isActive = targetUIObject.activeSelf;
            targetUIObject.SetActive(!isActive);
            Debug.Log($"UI状态切换为: {(!isActive ? "显示" : "隐藏")}");
        }
    }
}
