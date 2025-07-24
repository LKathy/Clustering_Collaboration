using UnityEngine;
using UnityEngine.UI;

public class ModeManager : MonoBehaviour
{
    [Header("模式控制")]
    public Toggle mode1Toggle;  // 模式一Toggle
    public Toggle mode2Toggle;  // 模式二Toggle

    [Header("模式二交互元素")]
    public GameObject interactionPanel;  // 包含输入框和按钮的面板
    public InputField dataInputField;    // 数据输入框
    public Button submitButton;          // 提交按钮

    private bool isMode2 = false;        // 当前是否为模式二

    void Start()
    {
        // 初始化设置
        InitializeMode();

        // 绑定Toggle事件
        if (mode1Toggle != null)
        {
            mode1Toggle.onValueChanged.AddListener(OnMode1ToggleChanged);
        }

        if (mode2Toggle != null)
        {
            mode2Toggle.onValueChanged.AddListener(OnMode2ToggleChanged);
        }

        // 绑定提交按钮事件
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }
    }

    void InitializeMode()
    {
        // 默认为模式一
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

            // 确保模式二Toggle不被选中
            if (mode2Toggle != null)
            {
                mode2Toggle.isOn = false;
            }

            UpdateModeUI();
            Debug.Log("切换到模式一");
        }
    }

    public void OnMode2ToggleChanged(bool isOn)
    {
        if (isOn)
        {
            isMode2 = true;

            // 确保模式一Toggle不被选中
            if (mode1Toggle != null)
            {
                mode1Toggle.isOn = false;
            }

            UpdateModeUI();
            Debug.Log("切换到模式二");
        }
    }

    void UpdateModeUI()
    {
        // 更新交互面板的可用性
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(isMode2);
        }

        // 更新输入框的可交互性
        if (dataInputField != null)
        {
            dataInputField.interactable = isMode2;
        }

        // 更新按钮的可交互性
        if (submitButton != null)
        {
            submitButton.interactable = isMode2;
        }

        // 如果切换到模式一，清空输入框
        if (!isMode2 && dataInputField != null)
        {
            dataInputField.text = "";
        }
    }

    public void OnSubmitButtonClicked()
    {
        // 只有在模式二下才处理提交
        if (!isMode2)
        {
            Debug.LogWarning("当前不在模式二，无法提交数据");
            return;
        }

        if (dataInputField != null)
        {
            string inputData = dataInputField.text.Trim();

            if (string.IsNullOrEmpty(inputData))
            {
                Debug.LogWarning("请输入有效数据");
                return;
            }

            // 处理输入的数据
            ProcessInputData(inputData);
        }
    }

    void ProcessInputData(string data)
    {
        // 在这里处理用户输入的数据
        Debug.Log($"处理数据: {data}");

        // 清空输入框
        if (dataInputField != null)
        {
            dataInputField.text = "";
        }

        // 在这里添加你的具体业务逻辑
        // 例如：保存数据、发送网络请求、更新游戏状态等
    }

    // 公共方法，供外部脚本调用
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
