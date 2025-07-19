using System.Collections.Generic;
using UnityEngine;

public class CreateMissile02 : MonoBehaviour
{
    public GameObject missilePrefab; // 导弹预制体
    public Transform missilePool; // 导弹池的父物体
    public float spawnRadius = 1.0f; // 随机生成的半径
    public float missileRadius = 0.3f; // 导弹碰撞半径

    /// <summary>
    /// 在指定位置附近随机生成指定数量的导弹
    /// </summary>
    /// <param name="centerPos">生成中心点</param>
    /// <param name="missileCount">生成数量</param>
    public void CreateMissilesAtPosition(Vector3 centerPos, int missileCount)
    {
        List<Vector3> spawnPositions = new List<Vector3>();
        int maxAttempts = 100;

        for (int i = 0; i < missileCount; i++)
        {
            bool found = false;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                Vector3 candidatePos = centerPos + randomOffset;

                bool overlap = false;
                foreach (var pos in spawnPositions)
                {
                    if (Vector3.Distance(candidatePos, pos) < missileRadius * 2f)
                    {
                        overlap = true;
                        break;
                    }
                }

                if (!overlap)
                {
                    spawnPositions.Add(candidatePos);
                    Instantiate(missilePrefab, candidatePos, Quaternion.Euler(0, 90, 0), missilePool);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Debug.LogWarning("未能为所有导弹找到合适的生成位置，已生成部分导弹。");
                break;
            }
        }
    }
}
