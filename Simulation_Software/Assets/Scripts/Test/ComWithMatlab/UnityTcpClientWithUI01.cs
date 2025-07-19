using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System.Collections; // ����Э�������ռ�

public class UnityTcpClientWithUI01 : MonoBehaviour
{
    // ��Inspector����ק��Ӧ��UIԪ�ص���Щ����������
    public InputField inputField;
    public Button sendButton;
    public Text responseText;

    private TcpClient client;
    private NetworkStream stream;
    private StreamReader reader;
    private StreamWriter writer;

    private string serverIP = "127.0.0.1";
    private int serverPort = 55001;
    private bool isConnected = false;

    private Coroutine currentDisplayCoroutine; // ���ڴ洢��ǰ�������е�Э������

    void Start()
    {
        if (inputField == null || sendButton == null || responseText == null)
        {
            Debug.LogError("����Unity Inspector��ΪInput Field, Button, �� Response Text��ֵ��");
            return;
        }

        // ��ʼ�ı�������ʾ����״̬
        SetResponseTextAndFade("��������MATLAB������...", 0); // ������ʾ������ʧ

        ConnectToServer();

        sendButton.onClick.AddListener(SendNumberToMatlab);
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            client.NoDelay = true; // ����Nagle�㷨

            stream = client.GetStream();
            reader = new StreamReader(stream, new UTF8Encoding(false)); // ����BOMͷ
            writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

            isConnected = true;
            SetResponseTextAndFade("�����ӵ�MATLAB��������", 3f); // ���ӳɹ�����ʾ3��
            Debug.Log("�����ӵ�MATLAB��������");
        }
        catch (SocketException e)
        {
            isConnected = false;
            SetResponseTextAndFade("����ʧ��: " + e.Message, 5f); // ����ʧ����ʾ5��
            Debug.LogError("SocketException: " + e.Message);
        }
    }

    public void SendNumberToMatlab()
    {
        if (!isConnected)
        {
            SetResponseTextAndFade("δ���ӵ�MATLAB������������Ƿ����С�", 3f);
            Debug.LogWarning("������δ����״̬�·������ݡ�");
            return;
        }

        string numberString = inputField.text;
        int number;

        if (int.TryParse(numberString, out number))
        {
            try
            {
                writer.WriteLine(numberString);
                SetResponseTextAndFade("���ڷ���: " + numberString + "...", 3f); // ��������ʾ3��
                Debug.Log("���͵�MATLAB: " + numberString);

                string receivedData = reader.ReadLine();
                SetResponseTextAndFade(receivedData, 3f); // MATLAB�ظ���ʾ3��
                Debug.Log("��MATLAB���յ�: " + receivedData);
            }
            catch (IOException e)
            {
                SetResponseTextAndFade("ͨ�Ŵ���: " + e.Message, 5f);
                Debug.LogError("IOException during send/receive: " + e.Message);
                isConnected = false;
            }
            catch (SocketException e)
            {
                SetResponseTextAndFade("�������: " + e.Message, 5f);
                Debug.LogError("SocketException during send/receive: " + e.Message);
                isConnected = false;
            }
        }
        else
        {
            SetResponseTextAndFade("������һ����Ч�����֣�", 3f); // ��Ч������ʾ3��
            Debug.LogWarning("��Ч����: �������֡�");
        }
    }

    // �������������������ı��������Զ���ʧЭ��
    private void SetResponseTextAndFade(string message, float duration)
    {
        // ������������е�Э�̣���ֹͣ�����Է��µ���Ϣ���ɵ�Э�����
        if (currentDisplayCoroutine != null)
        {
            StopCoroutine(currentDisplayCoroutine);
        }

        // �����µ�Э������ʾ������ı�
        currentDisplayCoroutine = StartCoroutine(DisplayAndFadeText(message, duration));
    }

    // ����Э�̣���ʾ�ı�����ָ��ʱ������
    private IEnumerator DisplayAndFadeText(string message, float duration)
    {
        responseText.text = message; // ��ʾ��Ϣ

        if (duration > 0) // �������ʱ�����0����ȴ������
        {
            yield return new WaitForSeconds(duration); // �ȴ�ָ������
            responseText.text = ""; // ����ı�
            currentDisplayCoroutine = null; // ���Э������
        }
        // ���durationΪ0�����ı���һֱ��ʾ��ֱ����һ�ε���SetResponseTextAndFade
    }

    void OnApplicationQuit()
    {
        if (client != null)
        {
            if (stream != null) stream.Close();
            if (reader != null) reader.Close();
            if (writer != null) writer.Close();
            client.Close();
        }
        isConnected = false;
    }
}
