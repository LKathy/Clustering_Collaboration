using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //��������������ռ�

public class SwitchToMain : MonoBehaviour
{
    //�������η�Ӧ���ǹ��еģ����ⲿ���Է��ʵ�
    public void STM()
    {
        SceneManager.LoadScene("MainScene06");  //����������Ҫ�л��ĳ�������
    }
}
