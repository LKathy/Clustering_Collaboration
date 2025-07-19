using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShowMissileTrajectory01 : MonoBehaviour
{
    public GameObject missilePrefab;
    public GameObject flickerPrefab; 
    private class MissileFrame
    {
        public float time;
        public Vector3 position;
    }
    private List<List<MissileFrame>> missileTrajectories = new List<List<MissileFrame>>();
    private List<GameObject> missiles = new List<GameObject>();
    private float elapsedTime = 0f;
    private int missileCount = 11;

    void Start()
    {        
        LoadTrajectoryData();
        CreateMissiles();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        // ��ȡ������MissilePool������������
        GameObject missilePool = GameObject.Find("MissilePool");
        Vector3 parentPos = missilePool != null ? missilePool.transform.position : Vector3.zero;

        for (int i = 0; i < missileCount; i++)
        {
            if (missileTrajectories.Count <= i) continue;
            var frames = missileTrajectories[i];
            if (frames.Count == 0) continue;
            if (missiles.Count <= i || missiles[i] == null) continue;

            // ���ҵ�ǰʱ���ǰ�������֡������ֵ
            MissileFrame prev = frames[0], next = frames[frames.Count - 1];
            foreach (var frame in frames)
            {
                if (frame.time <= elapsedTime) prev = frame;
                if (frame.time > elapsedTime)
                {
                    next = frame;
                    break;
                }
            }
            float t = (next.time - prev.time) > 0 ? (elapsedTime - prev.time) / (next.time - prev.time) : 0;
            Vector3 localPos = Vector3.Lerp(prev.position, next.position, t);
            missiles[i].transform.position = parentPos + localPos;
        }
    }


    void LoadTrajectoryData()
    {
        missileTrajectories.Clear();
        for (int i = 0; i < missileCount; i++)
            missileTrajectories.Add(new List<MissileFrame>());

        // ֱ��ʹ�þ���·��
        string filePath = @"D:\project\unity\clustering_collaboration_project\Simulation_Software\Assets\Data\Test\MissileTrajectory\missile_trajectories.csv";
        if (!File.Exists(filePath))
        {
            Debug.LogError("�Ҳ����ļ�: " + filePath);
            return;
        }
        var lines = File.ReadAllLines(filePath);
        if (lines.Length < 2) return; // ����Ҫ�б�ͷ��һ������

        for (int i = 1; i < lines.Length; i++) // ������ͷ
        {
            var cols = lines[i].Split(',');
            if (cols.Length < 1 + missileCount * 3) continue; // �������

            float time = float.Parse(cols[0]);
            for (int m = 0; m < missileCount; m++)
            {
                int baseIdx = 1 + m * 3;
                float x = - float.Parse(cols[baseIdx]) / 30;
                float y = float.Parse(cols[baseIdx + 1]) / 100;
                float z = float.Parse(cols[baseIdx + 2]) / 50;
                missileTrajectories[m].Add(new MissileFrame
                {
                    time = time,
                    position = new Vector3(x, y, z)
                });
            }
        }
    }


    void CreateMissiles()
    {
        missiles.Clear();
        GameObject missilePool = GameObject.Find("MissilePool");
        if (missilePool == null)
        {
            Debug.LogError("MissilePool δ�ҵ�����ȷ������������Ϊ MissilePool ��GameObject��");
            return;
        }
        Quaternion y90 = Quaternion.Euler(0, 90, 0);
        for (int i = 0; i < missileCount; i++)
        {
            // ���ɵ���
            var missile = Instantiate(missilePrefab, missileTrajectories[i][0].position, y90, missilePool.transform);
            missile.name = "Missile_" + i;
            missiles.Add(missile);

            // ������˸����
            if (flickerPrefab != null)
            {
                var flickerObj = Instantiate(flickerPrefab, missile.transform.position + new Vector3(0, 5, 0), Quaternion.identity, missilePool.transform);
                var flickerScript = flickerObj.GetComponent<ObjectFlicker>();
                if (flickerScript != null)
                {
                    flickerScript.target = missile.transform;
                }
            }
            else
            {
                Debug.LogWarning("flickerPrefab δ��ֵ��δ������˸���塣");
            }
        }
    }


}
