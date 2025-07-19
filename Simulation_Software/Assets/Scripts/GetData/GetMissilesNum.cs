using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // �����ļ����������ռ�

public class GetMissilesNum : MonoBehaviour
{
    public InputField input;
    public static int MissileCount { get; private set; }

    // ���õ�������λ��
    public Transform spawnPoint;

    public void OnClick()
    {
        string missilesNum = input.text;
        Debug.Log("Missiles Num: " + missilesNum);

        // У�������Ƿ�Ϊ������  
        if (!int.TryParse(missilesNum, out int num) || num <= 0)
        {
            Debug.LogError("������һ����������");
            // ��Ҳ������������û�һ�� UI ��ʾ  
            return;
        }

        MissileCount = num; // ���¾�̬����  

        // �����ļ�·��  
        string filePath = "D:/project/unity/clustering_collaboration_project/Simulation_Software/Communication/MissilesNum.txt";

        // ������д���ļ�  
        try
        {
            File.WriteAllText(filePath, missilesNum);
            Debug.Log("�����ѳɹ����浽: " + filePath);
        }
        catch (IOException e)
        {
            Debug.LogError("��������ʱ��������: " + e.Message);
        }

        var missileCreator = FindObjectOfType<CreateMissile02>();
        Vector3 center = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        int count = MissileCount;
        missileCreator.CreateMissilesAtPosition(center, count);
    }
}
