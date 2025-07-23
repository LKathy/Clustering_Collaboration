using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAndHide : MonoBehaviour
{
    [Header("UI�������")]
    [SerializeField] private GameObject targetUIObject; // Ҫ���Ƶ�UI���

    void Start()
    {
        // ���û��ָ��UI���󣬳��Բ���Canvas�µĵ�һ���Ӷ���
        if (targetUIObject == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null && canvas.transform.childCount > 0)
            {
                targetUIObject = canvas.transform.GetChild(0).gameObject;
            }
        }

        // ��ʼ״̬��Ϊ����
        if (targetUIObject != null)
        {
            targetUIObject.SetActive(false);
        }
    }

    void Update()
    {
        // ��ⰴ������
        HandleInput();
    }

    private void HandleInput()
    {
        // ���¿ո����ʾUI
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowUI();
        }

        // ����ESC������UI
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideUI();
        }
    }

    /// <summary>
    /// ��ʾUI���
    /// </summary>
    public void ShowUI()
    {
        if (targetUIObject != null)
        {
            targetUIObject.SetActive(true);
            Debug.Log("UI����ʾ");
        }
        else
        {
            Debug.LogWarning("Ŀ��UI����δ���ã�");
        }
    }

    /// <summary>
    /// ����UI���
    /// </summary>
    public void HideUI()
    {
        if (targetUIObject != null)
        {
            targetUIObject.SetActive(false);
            Debug.Log("UI������");
        }
        else
        {
            Debug.LogWarning("Ŀ��UI����δ���ã�");
        }
    }

    /// <summary>
    /// �л�UI��ʾ״̬
    /// </summary>
    public void ToggleUI()
    {
        if (targetUIObject != null)
        {
            bool isActive = targetUIObject.activeSelf;
            targetUIObject.SetActive(!isActive);
            Debug.Log($"UI״̬�л�Ϊ: {(!isActive ? "��ʾ" : "����")}");
        }
    }
}
