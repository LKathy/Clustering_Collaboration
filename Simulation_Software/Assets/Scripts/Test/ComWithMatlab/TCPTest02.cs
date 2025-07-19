using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Text; // For Encoding

public class TCPTest02 : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private StreamReader reader;
    private StreamWriter writer;

    private string serverIP = "127.0.0.1";
    private int serverPort = 55001;

    void Start()
    {
        ConnectToServer();
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
            reader = new StreamReader(stream, Encoding.UTF8); // 确保编码一致
            writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true }; // 自动刷新

            Debug.Log("Connected to MATLAB server!");

            // 发送数据给MATLAB
            writer.WriteLine("Hello MATLAB from Unity!"); // 自动添加换行符
            Debug.Log("Sent data to MATLAB.");

            // 接收MATLAB的数据
            string receivedData = reader.ReadLine(); // 读取一行数据，直到遇到换行符
            Debug.Log("Received from MATLAB: " + receivedData);
        }
        catch (SocketException e)
        {
            Debug.LogError("SocketException: " + e.Message);
        }
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
    }
}
