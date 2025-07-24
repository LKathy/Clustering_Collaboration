using UnityEngine;
using UnityEngine.UI;

public class ModeManager : MonoBehaviour
{
    [Header("ģʽ����")]
    public Toggle mode1Toggle;  // ģʽһToggle
    public Toggle mode2Toggle;  // ģʽ��Toggle

    [Header("ģʽ������Ԫ��")]
    public GameObject interactionPanel;  // ���������Ͱ�ť�����
    public InputField dataInputField;    // ���������
    public Button submitButton;          // �ύ��ť

    private bool isMode2 = false;        // ��ǰ�Ƿ�Ϊģʽ��

    void Start()
    {
        // ��ʼ������
        InitializeMode();

        // ��Toggle�¼�
        if (mode1Toggle != null)
        {
            mode1Toggle.onValueChanged.AddListener(OnMode1ToggleChanged);
        }

        if (mode2Toggle != null)
        {
            mode2Toggle.onValueChanged.AddListener(OnMode2ToggleChanged);
        }

        // ���ύ��ť�¼�
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }
    }

    void InitializeMode()
    {
        // Ĭ��Ϊģʽһ
        isMode2 = false;

        if (mode1Toggle != null)
        {
            mode1Toggle.isOn = true;
        }

        if (mode2Toggle != null)
        {
            mode2Toggle.isOn = false;
        }

        UpdateModeUI();
    }

    public void OnMode1ToggleChanged(bool isOn)
    {
        if (isOn)
        {
            isMode2 = false;

            // ȷ��ģʽ��Toggle����ѡ��
            if (mode2Toggle != null)
            {
                mode2Toggle.isOn = false;
            }

            UpdateModeUI();
            Debug.Log("�л���ģʽһ");
        }
    }

    public void OnMode2ToggleChanged(bool isOn)
    {
        if (isOn)
        {
            isMode2 = true;

            // ȷ��ģʽһToggle����ѡ��
            if (mode1Toggle != null)
            {
                mode1Toggle.isOn = false;
            }

            UpdateModeUI();
            Debug.Log("�л���ģʽ��");
        }
    }

    void UpdateModeUI()
    {
        // ���½������Ŀ�����
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(isMode2);
        }

        // ���������Ŀɽ�����
        if (dataInputField != null)
        {
            dataInputField.interactable = isMode2;
        }

        // ���°�ť�Ŀɽ�����
        if (submitButton != null)
        {
            submitButton.interactable = isMode2;
        }

        // ����л���ģʽһ����������
        if (!isMode2 && dataInputField != null)
        {
            dataInputField.text = "";
        }
    }

    public void OnSubmitButtonClicked()
    {
        // ֻ����ģʽ���²Ŵ����ύ
        if (!isMode2)
        {
            Debug.LogWarning("��ǰ����ģʽ�����޷��ύ����");
            return;
        }

        if (dataInputField != null)
        {
            string inputData = dataInputField.text.Trim();

            if (string.IsNullOrEmpty(inputData))
            {
                Debug.LogWarning("��������Ч����");
                return;
            }

            // �������������
            ProcessInputData(inputData);
        }
    }

    void ProcessInputData(string data)
    {
        // �����ﴦ���û����������
        Debug.Log($"��������: {data}");

        // ��������
        if (dataInputField != null)
        {
            dataInputField.text = "";
        }

        // �����������ľ���ҵ���߼�
        // ���磺�������ݡ������������󡢸�����Ϸ״̬��
    }

    // �������������ⲿ�ű�����
    public bool IsMode2Active()
    {
        return isMode2;
    }

    public void SetMode(int mode)
    {
        if (mode == 1)
        {
            if (mode1Toggle != null)
            {
                mode1Toggle.isOn = true;
            }
        }
        else if (mode == 2)
        {
            if (mode2Toggle != null)
            {
                mode2Toggle.isOn = true;
            }
        }
    }
}
