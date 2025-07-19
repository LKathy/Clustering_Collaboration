using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMissile01 : MonoBehaviour
{
    public GameObject missilePrefab; // 导弹预制体
    public Transform missilePool; // 导弹池的父物体
    public float spawnRadius = 1.0f; // 随机生成的半径
    public float missileRadius = 0.3f; // 导弹碰撞半径（需根据实际调整）

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            int missileCount = GetMissilesNum.MissileCount;
            List<Vector2> spawnPositions = new List<Vector2>();

            int maxAttempts = 100; // 每个导弹最大尝试次数，防止死循环

            for (int i = 0; i < missileCount; i++)
            {
                bool found = false;
                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
                    Vector2 candidatePos = mousePos + randomOffset;

                    bool overlap = false;
                    foreach (var pos in spawnPositions)
                    {
                        if (Vector2.Distance(candidatePos, pos) < missileRadius * 2f)
                        {
                            overlap = true;
                            break;
                        }
                    }

                    if (!overlap)
                    {
                        spawnPositions.Add(candidatePos);
                        Instantiate(missilePrefab, candidatePos, Quaternion.identity, missilePool);
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
            Vector3 cameraPos = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(mousePos.x, mousePos.y, cameraPos.z);
        }
    }
}
