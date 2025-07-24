using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // ���UI�����ռ�

public class ShowMissileTrajectory04 : MonoBehaviour
{
    public GameObject missilePrefab;
    public GameObject flickerPrefab;

    // ������ըԤ��������
    public GameObject explosionPrefab;

    // ���������ư�ť����
    [Header("���ư�ť")]
    public Button showInitialPositionButton;
    public Button showFlickersButton;
    public Button startSimulationButton;
    // ��������������İ�ť�����鿴�ִؽ������ť��
    public Button someOtherButtonToEnable;


    // �������ڲ����Ʊ�־
    private bool isInitialPositionsShown = false;
    private bool areFlickersShown = false;


    // ���Ʒ����Ƿ����еı�־
    public bool isSimulationRunning = false;

    // ���������Ʒ����Ƿ���ͣ�ı�־
    public bool isSimulationPaused = false;

    // ������Inspector�����õĴ���ɫ����
    // ȷ����Inspector��Ϊÿ���أ��ܹ�3�أ�������ɫ
    public Color[] clusterColors;

    // ���������ٶȳ���
    // Ĭ��ֵΪ1.0f����ʾ�����ٶȡ�
    // ��Ϊ2.0f��ʾ2���٣�0.5f��ʾ���١�
    public float simulationSpeedMultiplier = 1.0f;

    // ������UI�ؼ�����
    [Header("UI�ؼ�")]
    public Dropdown speedDropdown; // ����ѡ�������˵�
    public Button pauseResumeButton; // ��ͣ/������ť
    public Text pauseResumeButtonText; // ��ť�ı����

    // ������Ԥ����ı���ѡ��
    private float[] speedOptions = { 1.0f, 2.0f, 4.0f, 10.0f };

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

    // ������˸�����б�ͱ�ը״̬�б�
    private List<GameObject> flickerObjects = new List<GameObject>(); // �洢ÿ��������Ӧ����˸����
    private List<bool> missileExplodedStatus = new List<bool>();     // ����ÿ�������Ƿ��ѱ�ը

    // �洢ÿ�������Ĵ�ID��ͬ����0-based����
    private List<int> missileClusterIDs;

    void Start()
    {
        // ��ʼ��UI
        InitializeUI();
        // Start��������ֻ����ʼ�������Զ���������
        // ���潫�ڰ�ť�����ͨ�� OnStartSimulationButtonClick() ��������

        // �������󶨰�ť�¼�
        if (showInitialPositionButton != null)
            showInitialPositionButton.onClick.AddListener(OnShowInitialPositionsButtonClick);

        if (showFlickersButton != null)
            showFlickersButton.onClick.AddListener(OnShowFlickersButtonClick);

        if (startSimulationButton != null)
            startSimulationButton.onClick.AddListener(OnStartSimulationConfirmedButtonClick);

        if (someOtherButtonToEnable != null)
            someOtherButtonToEnable.interactable = false;
    }

    /// <summary>
    /// ��ʼ��UI�ؼ�
    /// </summary>
    void InitializeUI()
    {
        // ��ʼ�����������˵�
        if (speedDropdown != null)
        {
            speedDropdown.ClearOptions();
            List<string> speedOptionStrings = new List<string>();
            for (int i = 0; i < speedOptions.Length; i++)
            {
                speedOptionStrings.Add(speedOptions[i] + "x");
            }
            speedDropdown.AddOptions(speedOptionStrings);
            speedDropdown.value = 0; // Ĭ��ѡ��1x
            speedDropdown.onValueChanged.AddListener(OnSpeedChanged);
        }

        // ��ʼ����ͣ/������ť
        if (pauseResumeButton != null)
        {
            pauseResumeButton.onClick.AddListener(OnPauseResumeButtonClick);
            UpdatePauseResumeButtonText();
        }
    }

    /// <summary>
    /// �����������˵�ֵ�ı�ʱ����
    /// </summary>
    /// <param name="index">ѡ�е�ѡ������</param>
    public void OnSpeedChanged(int index)
    {
        if (index >= 0 && index < speedOptions.Length)
        {
            simulationSpeedMultiplier = speedOptions[index];
            Debug.Log($"���汶���Ѹ���Ϊ: {simulationSpeedMultiplier}x");
        }
    }

    /// <summary>
    /// ��ͣ/������ť����¼�
    /// </summary>
    public void OnPauseResumeButtonClick()
    {
        if (!isSimulationRunning)
        {
            Debug.LogWarning("����δ���У��޷���ͣ�������");
            return;
        }

        isSimulationPaused = !isSimulationPaused;
        UpdatePauseResumeButtonText();

        if (isSimulationPaused)
        {
            Debug.Log("��������ͣ");
        }
        else
        {
            Debug.Log("�����Ѽ���");
        }
    }

    /// <summary>
    /// ������ͣ/������ť���ı�
    /// </summary>
    void UpdatePauseResumeButtonText()
    {
        if (pauseResumeButtonText != null)
        {
            pauseResumeButtonText.text = isSimulationPaused ? "����" : "��ͣ";
        }
        else if (pauseResumeButton != null)
        {
            // ���û�е�����Text��������Ի�ȡ��ť�Դ���Text���
            Text buttonText = pauseResumeButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = isSimulationPaused ? "����" : "��ͣ";
            }
        }
    }

    void Update()
    {
        // ֻ�е� isSimulationRunning Ϊ true ʱ��ִ�з����߼�
        if (!isSimulationRunning || isSimulationPaused)
        {
            return;
        }

        // Ӧ�÷����ٶȳ���
        // elapsedTime �������ٶ������� simulationSpeedMultiplier ����
        elapsedTime += Time.deltaTime * simulationSpeedMultiplier;

        // ��ȡ������MissilePool������������
        GameObject missilePool = GameObject.Find("MissilePool");
        Vector3 parentPos = missilePool != null ? missilePool.transform.position : Vector3.zero;

        for (int i = 0; i < missileCount; i++) // i ��0-based����
        {
            // ��鵼���Ƿ��ѱ�ը������ѱ�ը����������
            if (missileExplodedStatus.Count > i && missileExplodedStatus[i])
            {
                continue; // �����ѱ�ը�����ٸ�����λ�ú���ת
            }

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

            // ��ըЧ�������߼�
            // ֱ�Ӽ�� elapsedTime �Ƿ�ﵽ�򳬹��켣�����һ֡ʱ��
            if (frames.Count > 0 && elapsedTime >= frames[frames.Count - 1].time && !missileExplodedStatus[i])
            {
                // ȷ���б�ըԤ����
                if (explosionPrefab != null)
                {
                    // �ڵ�����ǰλ��ʵ������ըЧ��
                    Instantiate(explosionPrefab, missiles[i].transform.position, Quaternion.identity);
                    Debug.Log($"����ID {i + 1} �����յ㲢��ը��");
                }
                else
                {
                    Debug.LogWarning("explosionPrefab δ��ֵ���޷����ű�ըЧ����");
                }

                // ��Ǹõ����ѱ�ը
                missileExplodedStatus[i] = true;

                // ���ٵ���ģ��
                if (missiles[i] != null)
                {
                    Destroy(missiles[i]);
                    missiles[i] = null;
                }

                // ������˸����
                if (flickerObjects.Count > i && flickerObjects[i] != null)
                {
                    Destroy(flickerObjects[i]);
                    flickerObjects[i] = null;
                }
            }
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
                float x = -float.Parse(cols[1]) / 200; // �ڶ�����x
                float y = float.Parse(cols[2]) / 200; // ��������y
                float z = float.Parse(cols[3]) / 200;  // ��������z
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

        // ��ղ���ʼ����˸�����б�ͱ�ը״̬�б�
        flickerObjects.Clear();
        missileExplodedStatus.Clear();
        for (int i = 0; i < missileCount; i++)
        {
            flickerObjects.Add(null); // Ԥ��λ��
            missileExplodedStatus.Add(false); // ��ʼ��δ��ը
        }

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

                // �洢��˸��������
                flickerObjects[i] = flickerObj;

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

    // ��ʾ��ʼλ�ð�ť����
    public void OnShowInitialPositionsButtonClick()
    {
        if (isInitialPositionsShown)
        {
            Debug.Log("������ʼλ������ʾ��");
            return;
        }

        Debug.Log("��ʾ������ʼλ��...");
        LoadTrajectoryData();
        LoadClusterResults();
        CreateMissilesWithoutFlickers();
        isInitialPositionsShown = true;

        // �������棬����ͣ״̬
        isSimulationRunning = true;
        isSimulationPaused = true;
        UpdatePauseResumeButtonText();
    }

    // ��ʾ��˸���尴ť����
    public void OnShowFlickersButtonClick()
    {
        if (!isInitialPositionsShown)
        {
            Debug.LogWarning("���ȵ������ʾ��ʼλ�á���");
            return;
        }

        if (areFlickersShown)
        {
            Debug.Log("��˸��������ʾ��");
            return;
        }

        ShowFlickersOnly();
        areFlickersShown = true;
    }

    // �������水ť����
    public void OnStartSimulationConfirmedButtonClick()
    {
        if (!isInitialPositionsShown)
        {
            Debug.LogWarning("������ʾ������ʼλ�á�");
            return;
        }

        if (!areFlickersShown)
        {
            Debug.LogWarning("��������ʾ��˸�����ٿ�ʼ���档");
        }

        Debug.Log("��ʼ����...");
        isSimulationPaused = false;
        isSimulationRunning = true;
        UpdatePauseResumeButtonText();
    }

    // ֻ���ɵ���������˸���壩
    void CreateMissilesWithoutFlickers()
    {
        missiles.Clear();
        flickerObjects.Clear();
        missileExplodedStatus.Clear();

        for (int i = 0; i < missileCount; i++)
        {
            flickerObjects.Add(null);
            missileExplodedStatus.Add(false);
        }

        GameObject missilePool = GameObject.Find("MissilePool");
        if (missilePool == null)
        {
            Debug.LogError("MissilePool δ�ҵ���");
            return;
        }

        Vector3 parentPos = missilePool.transform.position; // ���ϸ�����ƫ��

        for (int i = 0; i < missileCount; i++)
        {
            if (missileTrajectories.Count <= i || missileTrajectories[i].Count == 0)
            {
                missiles.Add(null);
                continue;
            }

            var missile = Instantiate(
                missilePrefab,
                parentPos + missileTrajectories[i][0].position, // ����λ��
                Quaternion.Euler(-missileTrajectories[i][0].pitch, -missileTrajectories[i][0].yaw + 90, 0),
                missilePool.transform
            );
            missile.name = "Missile_" + (i + 1);
            missiles.Add(missile);
        }
    }


    // Ϊ����������˸��
    void ShowFlickersOnly()
    {
        GameObject missilePool = GameObject.Find("MissilePool");
        if (missilePool == null)
        {
            Debug.LogError("MissilePool δ�ҵ���");
            return;
        }

        Vector3 parentPos = missilePool.transform.position; // ���ϸ�����ƫ��

        for (int i = 0; i < missileCount; i++)
        {
            if (missiles[i] == null) continue;

            if (flickerPrefab != null)
            {
                var flickerObj = Instantiate(
                    flickerPrefab,
                    missiles[i].transform.position + new Vector3(0, 5, 0), // ʹ���������꣬ȷ��һ����
                    Quaternion.identity,
                    missilePool.transform
                );

                var flickerScript = flickerObj.GetComponent<ObjectFlicker>();
                if (flickerScript != null)
                {
                    flickerScript.target = missiles[i].transform;
                }

                flickerObjects[i] = flickerObj;

                if (missileClusterIDs != null && i < missileClusterIDs.Count)
                {
                    int clusterID = missileClusterIDs[i];
                    if (clusterID >= 0 && clusterID < clusterColors.Length)
                    {
                        Renderer flickerRenderer = flickerObj.GetComponent<Renderer>();
                        if (flickerRenderer != null)
                        {
                            flickerRenderer.material.color = clusterColors[clusterID];
                        }
                    }
                }
            }
        }
        // ���������á��鿴�ִؽ������ť
        if (someOtherButtonToEnable != null)
        {
            someOtherButtonToEnable.interactable = true;
            Debug.Log("��ذ�ť�Ѽ��");
        }
    }


}
