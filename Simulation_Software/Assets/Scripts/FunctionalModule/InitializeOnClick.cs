using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InitializeOnClick : MonoBehaviour
{
    public GameObject targetObject;  // 要显示的物体
    public Button initButton;        // 初始化按钮
    public Text infoText;            // 显示信息的Text UI

    private bool isInitialized = false;

    void Start()
    {
        // 初始隐藏目标物体
        if (targetObject != null)
            targetObject.SetActive(false);

        // 清空提示信息
        if (infoText != null)
            infoText.text = "";

        // 添加按钮监听
        if (initButton != null)
            initButton.onClick.AddListener(OnInitClicked);
    }

    void OnInitClicked()
    {
        if (!isInitialized)
        {
            // 显示物体
            if (targetObject != null)
                targetObject.SetActive(true);

            // 执行初始化逻辑
            Initialize();

            // 显示提示信息
            if (infoText != null)
            {
                infoText.text = "敌方舰队初始化成功！请点击“开启Matlab通信”启动Matlab进程！";
                StartCoroutine(HideInfoTextAfterDelay(2f)); // 2 秒后隐藏文字
            }

            // 禁用按钮
            if (initButton != null)
                initButton.interactable = false;

            isInitialized = true;
        }
    }

    void Initialize()
    {
        // 此处可以添加你的初始化内容
        Debug.Log("初始化逻辑执行完成");
    }

    IEnumerator HideInfoTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (infoText != null)
            infoText.text = "";
    }
}
