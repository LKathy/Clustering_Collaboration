using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShowMissileTrajectory03 : MonoBehaviour
{
    public GameObject missilePrefab;
    public GameObject flickerPrefab;

    // ���Ʒ����Ƿ����еı�־
    public bool isSimulationRunning = false;

    // ������Inspector�����õĴ���ɫ����
    // ȷ����Inspector��Ϊÿ���أ��ܹ�3�أ�������ɫ
    public Color[] clusterColors;

    private class MissileFrame
    {
        public float time;
        public Vector3 position;
        public float pitch; // ������������ (��X��)
        public float yaw;   // ������ƫ���� (��Y��)
    }
    // missileTrajectories �� missiles �б�����0-based��������СΪ missileCount
    private List<List<MissileFrame>> missileTrajectories = new List<List<MissileFrame>>();
    private List<GameObject> missiles = new List<GameObject>();
    private float elapsedTime = 0f;
    private int missileCount = 30; // ����������30����ID��Χ��1��30

    // �洢ÿ�������Ĵ�ID��ͬ����0-based����
    private List<int> missileClusterIDs;

    void Start()
    {
        // Start��������ֻ����ʼ�������Զ���������
        // ���潫�ڰ�ť�����ͨ�� OnStartSimulationButtonClick() ��������
    }

    void Update()
    {
        // ֻ�е� isSimulationRunning Ϊ true ʱ��ִ�з����߼�
        if (!isSimulationRunning)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        // ��ȡ������MissilePool������������
        GameObject missilePool = GameObject.Find("MissilePool");
        Vector3 parentPos = missilePool != null ? missilePool.transform.position : Vector3.zero;

        for (int i = 0; i < missileCount; i++) // i ��0-based����
        {
            // ��鵱ǰ�����Ƿ��й켣���ݣ��Լ���Ӧ��GameObject�Ƿ��Ѿ���������Ч
            if (missileTrajectories.Count <= i || missileTrajectories[i].Count == 0) continue; // û�й켣����������
            if (missiles.Count <= i || missiles[i] == null) continue; // ��������δ������Ϊnull������

            var frames = missileTrajectories[i];

            // ���ҵ�ǰʱ���ǰ�������֡������ֵ
            MissileFrame prev = frames[0];
            MissileFrame next = frames[frames.Count - 1];
            bool foundNext = false;
            for (int j = 0; j < frames.Count; j++)
            {
                if (frames[j].time <= elapsedTime)
                {
                    prev = frames[j];
                }
                if (frames[j].time > elapsedTime)
                {
                    next = frames[j];
                    foundNext = true;
                    break;
                }
            }
            if (!foundNext && frames.Count > 0)
            {
                prev = frames[frames.Count - 1];
                next = frames[frames.Count - 1];
            }

            float t = (next.time - prev.time) > 0 ? (elapsedTime - prev.time) / (next.time - prev.time) : 0;
            Vector3 localPos = Vector3.Lerp(prev.position, next.position, t);
            missiles[i].transform.position = parentPos + localPos;
            // ��������ֵ�����Ǻ�ƫ����
            float interpolatedPitch = Mathf.Lerp(prev.pitch, next.pitch, t);
            float interpolatedYaw = Mathf.Lerp(prev.yaw, next.yaw, t);
            // ������Ӧ����ת
            missiles[i].transform.rotation = Quaternion.Euler(-interpolatedPitch, -interpolatedYaw + 90, 0);
        }
    }

    /// <summary>
    /// ��UI��ť���ʱ���ô˷������������档
    /// </summary>
    public void OnStartSimulationButtonClick()
    {
        if (isSimulationRunning)
        {
            Debug.LogWarning("�������������С�");
            return;
        }

        Debug.Log("��ʼ���ص����켣���ݡ��ؽ���ʹ�������...");
        LoadTrajectoryData();
        LoadClusterResults(); // ���شؽ��
        CreateMissiles();
        elapsedTime = 0f; // ����ʱ�䣬ȷ����ͷ��ʼ����
        isSimulationRunning = true; // ��������
        Debug.Log("������������");
    }


    void LoadTrajectoryData()
    {
        missileTrajectories.Clear();
        // ��ʼ���б�ȷ���� missileCount ����λ��������0��missileCount-1
        for (int i = 0; i < missileCount; i++)
        {
            missileTrajectories.Add(new List<MissileFrame>());
        }

        // ���õ��������ļ��е�·��
        string folderPath = @"D:\project\Matlab\Test\2025_7_17_cluster_collaboration\data\";

        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("�Ҳ����ļ���: " + folderPath);
            return;
        }

        // ��ȡ�ļ��������з��� "missile_data_*.csv" ����������ļ�
        string[] missileFiles = Directory.GetFiles(folderPath, "missile_data_*.csv");

        if (missileFiles.Length == 0)
        {
            Debug.LogWarning("��ָ���ļ�����δ�ҵ��κε��������ļ�: " + folderPath);
            return;
        }

        foreach (string filePath in missileFiles)
        {
            // ���ļ�������ȡ����ID (���磺"missile_data_1.csv" -> "1")
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string idString = fileName.Replace("missile_data_", "");

            if (!int.TryParse(idString, out int missileID_1Based))
            {
                Debug.LogWarning("�޷����ļ����н�������ID�������ļ�: " + filePath);
                continue;
            }

            // ���1-based����ID�Ƿ�����Ч��Χ�� (1 �� missileCount)
            if (missileID_1Based < 1 || missileID_1Based > missileCount)
            {
                Debug.LogWarning($"����ID {missileID_1Based} ����Ԥ�跶Χ [1, {missileCount}]�������ļ�: " + filePath);
                continue;
            }

            // ��1-based IDת��Ϊ0-based����
            int missileIndex_0Based = missileID_1Based - 1;

            var lines = File.ReadAllLines(filePath);
            if (lines.Length < 2) // ����Ҫ�б�ͷ��һ������
            {
                Debug.LogWarning($"�ļ����ݲ��� (������Ҫ��ͷ��һ������)�������ļ�: {filePath}");
                continue;
            }

            for (int i = 1; i < lines.Length; i++) // ������ͷ
            {
                var cols = lines[i].Split(',');
                // ���������ʱ��, x, y, z, pitch, yaw (��6��)
                if (cols.Length < 6)
                {
                    Debug.LogWarning($"�ļ� {filePath} �е� {i} �е��������� (������Ҫ6��)���������С�");
                    continue;
                }

                float time = float.Parse(cols[0]);
                float x = -float.Parse(cols[1]) / 300; // �ڶ�����x
                float y = float.Parse(cols[2]) / 300; // ��������y
                float z = float.Parse(cols[3]) / 300;  // ��������z
                float pitch = float.Parse(cols[4]);  // �������������Ǹ�����
                float yaw = float.Parse(cols[5]);    // ��������������ƫ����

                missileTrajectories[missileIndex_0Based].Add(new MissileFrame
                {
                    time = time,
                    position = new Vector3(x, y, z),
                    pitch = pitch, // �洢������
                    yaw = yaw      // �洢ƫ����
                });
            }
        }
        Debug.Log("�����켣���ݼ�����ɡ�");
    }

    /// <summary>
    /// ���شؽ��
    /// ���� clustering_results.csv �ļ��ĸ�ʽΪ��
    /// missile_id, feature1, feature2, ..., cluster_id
    /// ���� missile_id �ǵ�һ�� (1-based)��cluster_id �����һ�С�
    /// </summary>
    void LoadClusterResults()
    {
        // ��ʼ���б���СΪ missileCount��������0��missileCount-1
        missileClusterIDs = new List<int>(new int[missileCount]);

        string clusterResultsPath = @"D:\project\Matlab\Test\2025_7_17_cluster_collaboration\data\clustering_results.csv";

        if (!File.Exists(clusterResultsPath))
        {
            Debug.LogError("�Ҳ����ؽ���ļ�: " + clusterResultsPath);
            return;
        }

        var lines = File.ReadAllLines(clusterResultsPath);
        if (lines.Length < 2) // ����Ҫ�б�ͷ��һ������
        {
            Debug.LogWarning("�ؽ���ļ����ݲ��� (������Ҫ��ͷ��һ������)��");
            return;
        }

        for (int i = 1; i < lines.Length; i++) // ������ͷ
        {
            var cols = lines[i].Split(',');
            if (cols.Length < 2) // ������Ҫ����ID�ʹ�ID
            {
                Debug.LogWarning($"�ؽ���ļ� {clusterResultsPath} �е� {i} �е��������㣬�������С�");
                continue;
            }

            if (!int.TryParse(cols[0], out int missileID_1Based)) // �����һ���ǵ���ID (1-based)
            {
                Debug.LogWarning($"�޷��Ӵؽ���ļ� {clusterResultsPath} �н�������ID����������: {lines[i]}");
                continue;
            }

            if (!int.TryParse(cols[cols.Length - 1], out int clusterID)) // �������һ���Ǵ�ID
            {
                Debug.LogWarning($"�޷��Ӵؽ���ļ� {clusterResultsPath} �н�����ID����������: {lines[i]}");
                continue;
            }

            // ���1-based����ID�Ƿ�����Ч��Χ�� (1 �� missileCount)
            if (missileID_1Based < 1 || missileID_1Based > missileCount)
            {
                Debug.LogWarning($"�ؽ���ļ��е���ID {missileID_1Based} ����Ԥ�跶Χ [1, {missileCount}]���������С�");
                continue;
            }

            // ����ID�Ƿ�����Ч��ɫ��Χ��
            if (clusterID < 0 || clusterID >= clusterColors.Length)
            {
                Debug.LogWarning($"�ؽ���ļ��д�ID {clusterID} ����Ԥ����ɫ���鷶Χ [0, {clusterColors.Length - 1}]���õ�����ʹ��Ĭ����ɫ (��ID 0)��");
                clusterID = 0; // Ĭ��ʹ�õ�һ����ɫ
            }

            // ��1-based IDת��Ϊ0-based����
            int missileIndex_0Based = missileID_1Based - 1;
            missileClusterIDs[missileIndex_0Based] = clusterID;
        }
        Debug.Log("�ؽ�����ݼ�����ɡ�");
    }


    void CreateMissiles()
    {
        missiles.Clear(); // ���֮ǰ�ĵ���ʵ��
        GameObject missilePool = GameObject.Find("MissilePool");
        if (missilePool == null)
        {
            Debug.LogError("MissilePool δ�ҵ�����ȷ������������Ϊ MissilePool ��GameObject��");
            return;
        }
        // Quaternion y90 = Quaternion.Euler(0, 90, 0);
        for (int i = 0; i < missileCount; i++) // i ��0-based����
        {
            // ����Ƿ��е�ǰ����ID�Ĺ켣����
            if (missileTrajectories.Count <= i || missileTrajectories[i].Count == 0)
            {
                Debug.LogWarning($"�ڲ����� {i} (��Ӧ����ID {i + 1}) û�м��ص��켣���ݣ����������õ�����");
                missiles.Add(null); // ���һ��nullռλ��������missiles�б��С��missileCountһ��
                continue;
            }

            // ���ɵ���
            var missile = Instantiate(missilePrefab, missileTrajectories[i][0].position, Quaternion.Euler(-missileTrajectories[i][0].pitch, -missileTrajectories[i][0].yaw + 90, 0), missilePool.transform);
            missile.name = "Missile_" + (i + 1); // ����Ϊ1-based ID
            missiles.Add(missile);

            // ������˸����
            if (flickerPrefab != null)
            {
                var flickerObj = Instantiate(flickerPrefab, missile.transform.position + new Vector3(0, 5, 0), Quaternion.identity, missilePool.transform);
                var flickerScript = flickerObj.GetComponent<ObjectFlicker>();
                if (flickerScript != null)
                {
                    flickerScript.target = missile.transform; // ��˸������浼���ƶ�
                }

                // ���ݴ�ID������˸������ɫ
                if (missileClusterIDs != null && i < missileClusterIDs.Count)
                {
                    int clusterID = missileClusterIDs[i]; // i ��0-based����
                    if (clusterID >= 0 && clusterID < clusterColors.Length)
                    {
                        Renderer flickerRenderer = flickerObj.GetComponent<Renderer>();
                        if (flickerRenderer != null)
                        {
                            flickerRenderer.material.color = clusterColors[clusterID];
                        }
                        else
                        {
                            Debug.LogWarning($"��˸���� {flickerObj.name} û�� Renderer ������޷�������ɫ��");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"�ڲ����� {i} (��Ӧ����ID {i + 1}) �Ĵ�ID {clusterID} ��Ч�򳬳���ɫ���鷶Χ��ʹ��Ĭ����ɫ��");
                    }
                }
                else
                {
                    Debug.LogWarning($"�ڲ����� {i} (��Ӧ����ID {i + 1}) û�ж�Ӧ�Ĵ�ID��Ϣ����˸����ʹ��Ĭ����ɫ��");
                }
            }
            else
            {
                Debug.LogWarning("flickerPrefab δ��ֵ��δ������˸���塣");
            }
        }
    }
}
