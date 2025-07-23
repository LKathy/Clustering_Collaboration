using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System.Collections; // ����Э�������ռ�

public class UnityTcpClientWithUI02 : MonoBehaviour
{
    // ��Inspector����ק��Ӧ��UIԪ�ص���Щ����������
    public InputField inputField;
    public Button sendButton;
    public Text responseText;

    // ���������ӺͶϿ���ť
    public Button connectButton;
    public Button disconnectButton;

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
        // �������UIԪ���Ƿ�����Inspector�и�ֵ
        if (inputField == null || sendButton == null || responseText == null ||
            connectButton == null || disconnectButton == null)
        {
            Debug.LogError("����Unity Inspector��Ϊ����Input Field, Button, �� Response Text��ֵ��");
            return;
        }

        // Ϊ��ť��ӵ���¼�������
        connectButton.onClick.AddListener(ConnectToServer);
        disconnectButton.onClick.AddListener(DisconnectFromServer);
        sendButton.onClick.AddListener(SendNumberToMatlab);

        // ��ʼ״̬��δ����
        UpdateUIState();
        SetResponseTextAndFade("������ť ������Matlabͨ�š� ����MATLABͨ�ţ�", 0); // ��ʼ��ʾ������ʧ
    }

    // ���ӵ�MATLAB������
    public void ConnectToServer()
    {
        if (isConnected)
        {
            SetResponseTextAndFade("�����ӵ��������������ظ����ӡ�", 2f);
            return;
        }

        SetResponseTextAndFade("��������MATLAB������...", 0); // ������ʾ������ʧ

        try
        {
            client = new TcpClient(serverIP, serverPort);
            client.NoDelay = true; // ����Nagle�㷨

            stream = client.GetStream();
            reader = new StreamReader(stream, new UTF8Encoding(false)); // ����BOMͷ
            writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

            isConnected = true;
            SetResponseTextAndFade("�����ӵ�MATLAB��������", 2f); // ���ӳɹ�����ʾ3��
            Debug.Log("�����ӵ�MATLAB��������");
            SetResponseTextAndFade("�����뵼�������������ȷ�ϡ���ť��ʼ������������ʱ��ͬ��������", 0); // ���ӳɹ�����ʾ3��
        }
        catch (SocketException e)
        {
            isConnected = false;
            SetResponseTextAndFade("����ʧ��: " + e.Message, 5f); // ����ʧ����ʾ5��
            Debug.LogError("SocketException: " + e.Message);
        }
        finally
        {
            UpdateUIState(); // ���°�ť״̬
        }
    }

    // �Ͽ���MATLAB������������
    public void DisconnectFromServer()
    {
        if (!isConnected)
        {
            SetResponseTextAndFade("δ���ӵ�������������Ͽ���", 3f);
            return;
        }

        try
        {
            if (client != null)
            {
                if (stream != null) stream.Close();
                if (reader != null) reader.Close();
                if (writer != null) writer.Close();
                client.Close();
            }
            isConnected = false;
            SetResponseTextAndFade("�ѶϿ���MATLAB�����������ӡ�", 3f);
            Debug.Log("�ѶϿ���MATLAB�����������ӡ�");
        }
        catch (IOException e)
        {
            SetResponseTextAndFade("�Ͽ�����ʱ��������: " + e.Message, 5f);
            Debug.LogError("IOException during disconnect: " + e.Message);
        }
        finally
        {
            UpdateUIState(); // ���°�ť״̬
        }
    }

    // �������ֵ�MATLAB
    public void SendNumberToMatlab()
    {
        if (!isConnected)
        {
            SetResponseTextAndFade("δ���ӵ�MATLAB���������ӷ�������", 3f);
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
                SetResponseTextAndFade("���ڷ���: " + numberString + "...", 3f);
                Debug.Log("���͵�MATLAB: " + numberString);

                //SetResponseTextAndFade("��ȷ�ϵ�������Ϊ��" + numberString + "...��ɳ�ʼ��", 2f);
                //SetResponseTextAndFade("������ť" + numberString, 2f);

                string receivedData = reader.ReadLine();
                SetResponseTextAndFade(receivedData, 3f);
                Debug.Log("��MATLAB���յ�: " + receivedData);
            }
            catch (IOException e)
            {
                SetResponseTextAndFade("ͨ�Ŵ���: " + e.Message + " (���ӿ����ѶϿ�)", 5f);
                Debug.LogError("IOException during send/receive: " + e.Message);
                isConnected = false; // ���������ѶϿ�
                UpdateUIState(); // ���°�ť״̬
            }
            catch (SocketException e)
            {
                SetResponseTextAndFade("�������: " + e.Message + " (���ӿ����ѶϿ�)", 5f);
                Debug.LogError("SocketException during send/receive: " + e.Message);
                isConnected = false; // ���������ѶϿ�
                UpdateUIState(); // ���°�ť״̬
            }
        }
        else
        {
            SetResponseTextAndFade("������һ����Ч�����֣�", 2f);
            Debug.LogWarning("��Ч����: �������֡�");
        }
    }

    // ��������״̬����UI��ť�Ŀ�����
    private void UpdateUIState()
    {
        connectButton.interactable = !isConnected;      // δ����ʱ�����Ӱ�ť����
        disconnectButton.interactable = isConnected;    // ������ʱ���Ͽ���ť����
        sendButton.interactable = isConnected;          // ������ʱ�����Ͱ�ť����
        inputField.interactable = isConnected;          // ������ʱ����������
    }

    // ���������ı��������Զ���ʧЭ��
    private void SetResponseTextAndFade(string message, float duration)
    {
        if (currentDisplayCoroutine != null)
        {
            StopCoroutine(currentDisplayCoroutine);
        }
        currentDisplayCoroutine = StartCoroutine(DisplayAndFadeText(message, duration));
    }

    // Э�̣���ʾ�ı�����ָ��ʱ������
    private IEnumerator DisplayAndFadeText(string message, float duration)
    {
        responseText.text = message;

        if (duration > 0)
        {
            yield return new WaitForSeconds(duration);
            responseText.text = "";
            currentDisplayCoroutine = null;
        }
    }

    void OnApplicationQuit()
    {
        // Ӧ�ó����˳�ʱȷ���Ͽ�����
        DisconnectFromServer();
    }
}
