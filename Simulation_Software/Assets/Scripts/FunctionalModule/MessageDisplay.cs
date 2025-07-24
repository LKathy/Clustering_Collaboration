using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageDisplay : MonoBehaviour
{
    // 可在 Inspector 中拖入：按钮、模板、Canvas
    public Button triggerButton;
    public GameObject messageTemplate;
    public Transform canvasTransform;

    void Start()
    {
        // 为按钮添加点击事件监听器
        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(ShowMessage);
        }
        else
        {
            Debug.LogWarning("未绑定按钮！");
        }
    }

    // 显示消息的方法
    void ShowMessage()
    {
        // 克隆模板
        GameObject newMessage = Instantiate(messageTemplate, canvasTransform);

        // 激活它（如果模板是隐藏的）
        newMessage.SetActive(true);

        // 修改文字内容
        Text textComponent = newMessage.GetComponent<Text>();
        if (textComponent != null)
        {
            textComponent.text = "已成功生成时空同步吸引域！";
        }

        // 可选：设置位置
        // newMessage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        // 可选：2秒后销毁
        StartCoroutine(HideAfterDelay(newMessage, 2f));
    }

    // 延迟销毁
    IEnumerator HideAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}
