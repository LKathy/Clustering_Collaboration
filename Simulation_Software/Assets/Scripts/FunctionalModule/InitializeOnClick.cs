using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InitializeOnClick : MonoBehaviour
{
    public GameObject targetObject;  // Ҫ��ʾ������
    public Button initButton;        // ��ʼ����ť
    public Text infoText;            // ��ʾ��Ϣ��Text UI

    private bool isInitialized = false;

    void Start()
    {
        // ��ʼ����Ŀ������
        if (targetObject != null)
            targetObject.SetActive(false);

        // �����ʾ��Ϣ
        if (infoText != null)
            infoText.text = "";

        // ��Ӱ�ť����
        if (initButton != null)
            initButton.onClick.AddListener(OnInitClicked);
    }

    void OnInitClicked()
    {
        if (!isInitialized)
        {
            // ��ʾ����
            if (targetObject != null)
                targetObject.SetActive(true);

            // ִ�г�ʼ���߼�
            Initialize();

            // ��ʾ��ʾ��Ϣ
            if (infoText != null)
            {
                infoText.text = "�з����ӳ�ʼ���ɹ�������������Matlabͨ�š�����Matlab���̣�";
                StartCoroutine(HideInfoTextAfterDelay(2f)); // 2 �����������
            }

            // ���ð�ť
            if (initButton != null)
                initButton.interactable = false;

            isInitialized = true;
        }
    }

    void Initialize()
    {
        // �˴����������ĳ�ʼ������
        Debug.Log("��ʼ���߼�ִ�����");
    }

    IEnumerator HideInfoTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (infoText != null)
            infoText.text = "";
    }
}
