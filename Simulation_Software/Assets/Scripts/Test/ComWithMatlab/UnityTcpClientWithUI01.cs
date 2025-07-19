using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System.Collections; // 引入协程命名空间

public class UnityTcpClientWithUI01 : MonoBehaviour
{
    // 在Inspector中拖拽对应的UI元素到这些公共变量上
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

    private Coroutine currentDisplayCoroutine; // 用于存储当前正在运行的协程引用

    void Start()
    {
        if (inputField == null || sendButton == null || responseText == null)
        {
            Debug.LogError("请在Unity Inspector中为Input Field, Button, 和 Response Text赋值！");
            return;
        }

        // 初始文本可以显示连接状态
        SetResponseTextAndFade("尝试连接MATLAB服务器...", 0); // 立即显示，不消失

        ConnectToServer();

        sendButton.onClick.AddListener(SendNumberToMatlab);
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            client.NoDelay = true; // 禁用Nagle算法

            stream = client.GetStream();
            reader = new StreamReader(stream, new UTF8Encoding(false)); // 避免BOM头
            writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

            isConnected = true;
            SetResponseTextAndFade("已连接到MATLAB服务器！", 3f); // 连接成功后显示3秒
            Debug.Log("已连接到MATLAB服务器！");
        }
        catch (SocketException e)
        {
            isConnected = false;
            SetResponseTextAndFade("连接失败: " + e.Message, 5f); // 连接失败显示5秒
            Debug.LogError("SocketException: " + e.Message);
        }
    }

    public void SendNumberToMatlab()
    {
        if (!isConnected)
        {
            SetResponseTextAndFade("未连接到MATLAB。请检查服务器是否运行。", 3f);
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
                SetResponseTextAndFade("正在发送: " + numberString + "...", 3f); // 发送中提示3秒
                Debug.Log("发送到MATLAB: " + numberString);

                string receivedData = reader.ReadLine();
                SetResponseTextAndFade(receivedData, 3f); // MATLAB回复显示3秒
                Debug.Log("从MATLAB接收到: " + receivedData);
            }
            catch (IOException e)
            {
                SetResponseTextAndFade("通信错误: " + e.Message, 5f);
                Debug.LogError("IOException during send/receive: " + e.Message);
                isConnected = false;
            }
            catch (SocketException e)
            {
                SetResponseTextAndFade("网络错误: " + e.Message, 5f);
                Debug.LogError("SocketException during send/receive: " + e.Message);
                isConnected = false;
            }
        }
        else
        {
            SetResponseTextAndFade("请输入一个有效的数字！", 3f); // 无效输入提示3秒
            Debug.LogWarning("无效输入: 不是数字。");
        }
    }

    // 新增方法：用于设置文本并启动自动消失协程
    private void SetResponseTextAndFade(string message, float duration)
    {
        // 如果有正在运行的协程，先停止它，以防新的消息被旧的协程清除
        if (currentDisplayCoroutine != null)
        {
            StopCoroutine(currentDisplayCoroutine);
        }

        // 启动新的协程来显示和清除文本
        currentDisplayCoroutine = StartCoroutine(DisplayAndFadeText(message, duration));
    }

    // 新增协程：显示文本并在指定时间后清除
    private IEnumerator DisplayAndFadeText(string message, float duration)
    {
        responseText.text = message; // 显示消息

        if (duration > 0) // 如果持续时间大于0，则等待并清除
        {
            yield return new WaitForSeconds(duration); // 等待指定秒数
            responseText.text = ""; // 清除文本
            currentDisplayCoroutine = null; // 清除协程引用
        }
        // 如果duration为0，则文本会一直显示，直到下一次调用SetResponseTextAndFade
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
