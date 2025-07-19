using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMissile01 : MonoBehaviour
{
    public GameObject missilePrefab; // ����Ԥ����
    public Transform missilePool; // �����صĸ�����
    public float spawnRadius = 1.0f; // ������ɵİ뾶
    public float missileRadius = 0.3f; // ������ײ�뾶�������ʵ�ʵ�����

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            int missileCount = GetMissilesNum.MissileCount;
            List<Vector2> spawnPositions = new List<Vector2>();

            int maxAttempts = 100; // ÿ����������Դ�������ֹ��ѭ��

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
                    Debug.LogWarning("δ��Ϊ���е����ҵ����ʵ�����λ�ã������ɲ��ֵ�����");
                    break;
                }
            }
            Vector3 cameraPos = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(mousePos.x, mousePos.y, cameraPos.z);
        }
    }
}
