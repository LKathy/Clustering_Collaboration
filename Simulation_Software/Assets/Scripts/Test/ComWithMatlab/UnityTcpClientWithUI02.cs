using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System.Collections; // 引入协程命名空间

public class UnityTcpClientWithUI02 : MonoBehaviour
{
    // 在Inspector中拖拽对应的UI元素到这些公共变量上
    public InputField inputField;
    public Button sendButton;
    public Text responseText;

    // 新增：连接和断开按钮
    public Button connectButton;
    public Button disconnectButton;

    private TcpClient client;
    private NetworkStream stream;
    private StreamReader reader;
    private StreamWriter writer;

    private string serverIP = "127.0.0.1";
    private int serverPort = 55001;
    private bool isConnected = false;

    private Coroutine currentDisplayCoroutine; // 用于存储当前正在运行的协程引用

    void Start()
    {
        // 检查所有UI元素是否已在Inspector中赋值
        if (inputField == null || sendButton == null || responseText == null ||
            connectButton == null || disconnectButton == null)
        {
            Debug.LogError("请在Unity Inspector中为所有Input Field, Button, 和 Response Text赋值！");
            return;
        }

        // 为按钮添加点击事件监听器
        connectButton.onClick.AddListener(ConnectToServer);
        disconnectButton.onClick.AddListener(DisconnectFromServer);
        sendButton.onClick.AddListener(SendNumberToMatlab);

        // 初始状态：未连接
        UpdateUIState();
        SetResponseTextAndFade("请点击按钮 “开启Matlab通信” 开启MATLAB通信！", 0); // 初始显示，不消失
    }

    // 连接到MATLAB服务器
    public void ConnectToServer()
    {
        if (isConnected)
        {
            SetResponseTextAndFade("已连接到服务器，无需重复连接。", 2f);
            return;
        }

        SetResponseTextAndFade("尝试连接MATLAB服务器...", 0); // 立即显示，不消失

        try
        {
            client = new TcpClient(serverIP, serverPort);
            client.NoDelay = true; // 禁用Nagle算法

            stream = client.GetStream();
            reader = new StreamReader(stream, new UTF8Encoding(false)); // 避免BOM头
            writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

            isConnected = true;
            SetResponseTextAndFade("已连接到MATLAB服务器！", 2f); // 连接成功后显示3秒
            Debug.Log("已连接到MATLAB服务器！");
            SetResponseTextAndFade("请输入导弹数量并点击“确认”按钮初始化导弹并生成时空同步吸引域！", 0); // 连接成功后显示3秒
        }
        catch (SocketException e)
        {
            isConnected = false;
            SetResponseTextAndFade("连接失败: " + e.Message, 5f); // 连接失败显示5秒
            Debug.LogError("SocketException: " + e.Message);
        }
        finally
        {
            UpdateUIState(); // 更新按钮状态
        }
    }

    // 断开与MATLAB服务器的连接
    public void DisconnectFromServer()
    {
        if (!isConnected)
        {
            SetResponseTextAndFade("未连接到服务器，无需断开。", 3f);
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
            SetResponseTextAndFade("已断开与MATLAB服务器的连接。", 3f);
            Debug.Log("已断开与MATLAB服务器的连接。");
        }
        catch (IOException e)
        {
            SetResponseTextAndFade("断开连接时发生错误: " + e.Message, 5f);
            Debug.LogError("IOException during disconnect: " + e.Message);
        }
        finally
        {
            UpdateUIState(); // 更新按钮状态
        }
    }

    // 发送数字到MATLAB
    public void SendNumberToMatlab()
    {
        if (!isConnected)
        {
            SetResponseTextAndFade("未连接到MATLAB。请先连接服务器。", 3f);
            Debug.LogWarning("尝试在未连接状态下发送数据。");
            return;
        }

        string numberString = inputField.text;
        int number;

        if (int.TryParse(numberString, out number))
        {
            try
            {
                
                writer.WriteLine(numberString);
                SetResponseTextAndFade("正在发送: " + numberString + "...", 3f);
                Debug.Log("发送到MATLAB: " + numberString);

                //SetResponseTextAndFade("已确认导弹数量为：" + numberString + "...完成初始化", 2f);
                //SetResponseTextAndFade("请点击按钮" + numberString, 2f);

                string receivedData = reader.ReadLine();
                SetResponseTextAndFade(receivedData, 3f);
                Debug.Log("从MATLAB接收到: " + receivedData);
            }
            catch (IOException e)
            {
                SetResponseTextAndFade("通信错误: " + e.Message + " (连接可能已断开)", 5f);
                Debug.LogError("IOException during send/receive: " + e.Message);
                isConnected = false; // 假设连接已断开
                UpdateUIState(); // 更新按钮状态
            }
            catch (SocketException e)
            {
                SetResponseTextAndFade("网络错误: " + e.Message + " (连接可能已断开)", 5f);
                Debug.LogError("SocketException during send/receive: " + e.Message);
                isConnected = false; // 假设连接已断开
                UpdateUIState(); // 更新按钮状态
            }
        }
        else
        {
            SetResponseTextAndFade("请输入一个有效的数字！", 2f);
            Debug.LogWarning("无效输入: 不是数字。");
        }
    }

    // 根据连接状态更新UI按钮的可用性
    private void UpdateUIState()
    {
        connectButton.interactable = !isConnected;      // 未连接时，连接按钮可用
        disconnectButton.interactable = isConnected;    // 已连接时，断开按钮可用
        sendButton.interactable = isConnected;          // 已连接时，发送按钮可用
        inputField.interactable = isConnected;          // 已连接时，输入框可用
    }

    // 用于设置文本并启动自动消失协程
    private void SetResponseTextAndFade(string message, float duration)
    {
        if (currentDisplayCoroutine != null)
        {
            StopCoroutine(currentDisplayCoroutine);
        }
        currentDisplayCoroutine = StartCoroutine(DisplayAndFadeText(message, duration));
    }

    // 协程：显示文本并在指定时间后清除
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
        // 应用程序退出时确保断开连接
        DisconnectFromServer();
    }
}
