using System.Collections.Generic;
using UnityEngine;

public class CreateMissile02 : MonoBehaviour
{
    public GameObject missilePrefab; // ����Ԥ����
    public Transform missilePool; // �����صĸ�����
    public float spawnRadius = 1.0f; // ������ɵİ뾶
    public float missileRadius = 0.3f; // ������ײ�뾶

    /// <summary>
    /// ��ָ��λ�ø����������ָ�������ĵ���
    /// </summary>
    /// <param name="centerPos">�������ĵ�</param>
    /// <param name="missileCount">��������</param>
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
                Debug.LogWarning("δ��Ϊ���е����ҵ����ʵ�����λ�ã������ɲ��ֵ�����");
                break;
            }
        }
    }
}
