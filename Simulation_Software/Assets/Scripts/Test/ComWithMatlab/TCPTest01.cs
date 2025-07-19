using UnityEngine;
using System.Net.Sockets;
using System;
using System.Text;

public class TCPTest01 : MonoBehaviour
{
    TcpClient client;

    void Start()
    {
        // ���������������ϵ�MATLAB������������
        StartCoroutine(ConnectToServer("localhost", 55001));
    }

    System.Collections.IEnumerator ConnectToServer(string ip, int port)
    {
        while (true)  // ���ֳ�������ֱ���ɹ�
        {
            try
            {
                client = new TcpClient(ip, port);
                Debug.Log("��������������");
                break;  // �ɹ����Ӻ��˳�ѭ��
            }
            catch (Exception e)
            {
                Debug.LogError("����ʧ��: " + e.Message);
            }
            // ������ȴ�һ���������  
            yield return new WaitForSeconds(1);
        }

        // ���ӳɹ��󣬷���һЩ����
        if (client != null && client.Connected)
        {
            var stream = client.GetStream();
            string message = "Hello, MATLAB!";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
            Debug.Log("�ѷ�����Ϣ: " + message);
        }

        // ����һ��Э�����������ݽ���
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
                        // �����ȡ�������ݳ���Ϊ0����ʾ���ӿ����Ѿ��ر�
                        break;
                    }

                    string data = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log("�ӷ��������յ�����: " + data);

                    // ������յ�������
                }
                catch (Exception e)
                {
                    Debug.LogError("��ȡ����ʱ��������: " + e.Message);
                    break;
                }
            }
        }

        // �رտͻ���
        client.Close();
        yield return null;
    }
}