using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageDisplay : MonoBehaviour
{
    // ���� Inspector �����룺��ť��ģ�塢Canvas
    public Button triggerButton;
    public GameObject messageTemplate;
    public Transform canvasTransform;

    void Start()
    {
        // Ϊ��ť��ӵ���¼�������
        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(ShowMessage);
        }
        else
        {
            Debug.LogWarning("δ�󶨰�ť��");
        }
    }

    // ��ʾ��Ϣ�ķ���
    void ShowMessage()
    {
        // ��¡ģ��
        GameObject newMessage = Instantiate(messageTemplate, canvasTransform);

        // �����������ģ�������صģ�
        newMessage.SetActive(true);

        // �޸���������
        Text textComponent = newMessage.GetComponent<Text>();
        if (textComponent != null)
        {
            textComponent.text = "�ѳɹ�����ʱ��ͬ��������";
        }

        // ��ѡ������λ��
        // newMessage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        // ��ѡ��2�������
        StartCoroutine(HideAfterDelay(newMessage, 2f));
    }

    // �ӳ�����
    IEnumerator HideAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}
