using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // 引入文件操作命名空间

public class GetMissilesNum : MonoBehaviour
{
    public InputField input;
    public static int MissileCount { get; private set; }

    // 设置导弹产生位置
    public Transform spawnPoint;

    public void OnClick()
    {
        string missilesNum = input.text;
        Debug.Log("Missiles Num: " + missilesNum);

        // 校验输入是否为正整数  
        if (!int.TryParse(missilesNum, out int num) || num <= 0)
        {
            Debug.LogError("请输入一个正整数！");
            // 你也可以在这里给用户一个 UI 提示  
            return;
        }

        MissileCount = num; // 更新静态属性  

        // 定义文件路径  
        string filePath = "D:/project/unity/clustering_collaboration_project/Simulation_Software/Communication/MissilesNum.txt";

        // 将数据写入文件  
        try
        {
            File.WriteAllText(filePath, missilesNum);
            Debug.Log("数据已成功保存到: " + filePath);
        }
        catch (IOException e)
        {
            Debug.LogError("保存数据时发生错误: " + e.Message);
        }

        var missileCreator = FindObjectOfType<CreateMissile02>();
        Vector3 center = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        int count = MissileCount;
        missileCreator.CreateMissilesAtPosition(center, count);
    }
}
