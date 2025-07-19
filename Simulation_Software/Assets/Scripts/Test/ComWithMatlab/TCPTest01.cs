using UnityEngine;
using System.Net.Sockets;
using System;
using System.Text;

public class TCPTest01 : MonoBehaviour
{
    TcpClient client;

    void Start()
    {
        // 建立到本地主机上的MATLAB服务器的连接
        StartCoroutine(ConnectToServer("localhost", 55001));
    }

    System.Collections.IEnumerator ConnectToServer(string ip, int port)
    {
        while (true)  // 保持尝试连接直到成功
        {
            try
            {
                client = new TcpClient(ip, port);
                Debug.Log("已连接至服务器");
                break;  // 成功连接后退出循环
            }
            catch (Exception e)
            {
                Debug.LogError("连接失败: " + e.Message);
            }
            // 在这里等待一秒后再重试  
            yield return new WaitForSeconds(1);
        }

        // 连接成功后，发送一些数据
        if (client != null && client.Connected)
        {
            var stream = client.GetStream();
            string message = "Hello, MATLAB!";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
            Debug.Log("已发送消息: " + message);
        }

        // 启动一个协程来处理数据接收
        StartCoroutine(ReceiveData());
    }

    private System.Collections.IEnumerator ReceiveData()
    {
        if (client != null && client.Connected)
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        // 如果读取到的数据长度为0，表示连接可能已经关闭
                        break;
                    }

                    string data = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log("从服务器接收到数据: " + data);

                    // 处理接收到的数据
                }
                catch (Exception e)
                {
                    Debug.LogError("读取数据时发生错误: " + e.Message);
                    break;
                }
            }
        }

        // 关闭客户端
        client.Close();
        yield return null;
    }
}