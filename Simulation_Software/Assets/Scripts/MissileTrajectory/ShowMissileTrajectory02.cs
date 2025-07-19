using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShowMissileTrajectory02 : MonoBehaviour
{
    public GameObject missilePrefab;
    public GameObject flickerPrefab;

    // 新增爆炸预制体引用
    public GameObject explosionPrefab;

    // 控制仿真是否运行的标志
    public bool isSimulationRunning = false;

    // 用于在Inspector中设置的簇颜色数组
    // 确保在Inspector中为每个簇（总共3簇）设置颜色
    public Color[] clusterColors;

    // 新增仿真速度乘数
    // 默认值为1.0f，表示正常速度。
    // 设为2.0f表示2倍速，0.5f表示半速。
    public float simulationSpeedMultiplier = 10.0f;

    private class MissileFrame
    {
        public float time;
        public Vector3 position;
        public float pitch; // 新增：俯仰角 (绕X轴)
        public float yaw;   // 新增：偏航角 (绕Y轴)
    }
    // missileTrajectories 和 missiles 列表将保持0-based索引，大小为 missileCount
    private List<List<MissileFrame>> missileTrajectories = new List<List<MissileFrame>>();
    private List<GameObject> missiles = new List<GameObject>();
    private float elapsedTime = 0f;
    private int missileCount = 30; // 导弹数量是30个，ID范围是1到30

    // 新增闪烁物体列表和爆炸状态列表
    private List<GameObject> flickerObjects = new List<GameObject>(); // 存储每个导弹对应的闪烁物体
    private List<bool> missileExplodedStatus = new List<bool>();     // 跟踪每个导弹是否已爆炸

    // 存储每个导弹的簇ID，同样是0-based索引
    private List<int> missileClusterIDs;

    void Start()
    {
        // Start方法现在只做初始化，不自动启动仿真
        // 仿真将在按钮点击后通过 OnStartSimulationButtonClick() 方法启动
    }

    void Update()
    {
        // 只有当 isSimulationRunning 为 true 时才执行仿真逻辑
        if (!isSimulationRunning)
        {
            return;
        }

        // 应用仿真速度乘数
        // elapsedTime 的增长速度现在受 simulationSpeedMultiplier 控制
        elapsedTime += Time.deltaTime * simulationSpeedMultiplier;

        // 获取父对象（MissilePool）的世界坐标
        GameObject missilePool = GameObject.Find("MissilePool");
        Vector3 parentPos = missilePool != null ? missilePool.transform.position : Vector3.zero;

        for (int i = 0; i < missileCount; i++) // i 是0-based索引
        {
            // 检查导弹是否已爆炸，如果已爆炸则跳过更新
            if (missileExplodedStatus.Count > i && missileExplodedStatus[i])
            {
                continue; // 导弹已爆炸，不再更新其位置和旋转
            }

            // 检查当前导弹是否有轨迹数据，以及对应的GameObject是否已经创建且有效
            if (missileTrajectories.Count <= i || missileTrajectories[i].Count == 0) continue; // 没有轨迹数据则跳过
            if (missiles.Count <= i || missiles[i] == null) continue; // 导弹对象未创建或为null则跳过

            var frames = missileTrajectories[i];

            // 查找当前时间点前后的两个帧，做插值
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
            // 新增：插值俯仰角和偏航角
            float interpolatedPitch = Mathf.Lerp(prev.pitch, next.pitch, t);
            float interpolatedYaw = Mathf.Lerp(prev.yaw, next.yaw, t);
            // 新增：应用旋转
            missiles[i].transform.rotation = Quaternion.Euler(-interpolatedPitch, -interpolatedYaw + 90, 0);

            // 爆炸效果触发逻辑
            // 直接检查 elapsedTime 是否达到或超过轨迹的最后一帧时间
            if (frames.Count > 0 && elapsedTime >= frames[frames.Count - 1].time && !missileExplodedStatus[i])
            {
                // 确保有爆炸预制体
                if (explosionPrefab != null)
                {
                    // 在导弹当前位置实例化爆炸效果
                    Instantiate(explosionPrefab, missiles[i].transform.position, Quaternion.identity);
                    Debug.Log($"导弹ID {i + 1} 到达终点并爆炸！");
                }
                else
                {
                    Debug.LogWarning("explosionPrefab 未赋值，无法播放爆炸效果。");
                }

                // 标记该导弹已爆炸
                missileExplodedStatus[i] = true;

                // 销毁导弹模型
                if (missiles[i] != null)
                {
                    Destroy(missiles[i]);
                    missiles[i] = null;
                }

                // 销毁闪烁物体
                if (flickerObjects.Count > i && flickerObjects[i] != null)
                {
                    Destroy(flickerObjects[i]);
                    flickerObjects[i] = null;
                }
            }
        }
    }

    /// <summary>
    /// 当UI按钮点击时调用此方法来启动仿真。
    /// </summary>
    public void OnStartSimulationButtonClick()
    {
        if (isSimulationRunning)
        {
            Debug.LogWarning("仿真已在运行中。");
            return;
        }

        Debug.Log("开始加载导弹轨迹数据、簇结果和创建导弹...");
        LoadTrajectoryData();
        LoadClusterResults(); // 加载簇结果
        CreateMissiles();
        elapsedTime = 0f; // 重置时间，确保从头开始仿真
        isSimulationRunning = true; // 启动仿真
        Debug.Log("仿真已启动！");
    }


    void LoadTrajectoryData()
    {
        missileTrajectories.Clear();
        // 初始化列表，确保有 missileCount 个槽位，索引从0到missileCount-1
        for (int i = 0; i < missileCount; i++)
        {
            missileTrajectories.Add(new List<MissileFrame>());
        }

        // 设置导弹数据文件夹的路径
        string folderPath = @"D:\project\Matlab\Test\2025_7_17_cluster_collaboration\data\";

        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("找不到文件夹: " + folderPath);
            return;
        }

        // 获取文件夹中所有符合 "missile_data_*.csv" 命名规则的文件
        string[] missileFiles = Directory.GetFiles(folderPath, "missile_data_*.csv");

        if (missileFiles.Length == 0)
        {
            Debug.LogWarning("在指定文件夹中未找到任何导弹数据文件: " + folderPath);
            return;
        }

        foreach (string filePath in missileFiles)
        {
            // 从文件名中提取导弹ID (例如："missile_data_1.csv" -> "1")
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string idString = fileName.Replace("missile_data_", "");

            if (!int.TryParse(idString, out int missileID_1Based))
            {
                Debug.LogWarning("无法从文件名中解析导弹ID，跳过文件: " + filePath);
                continue;
            }

            // 检查1-based导弹ID是否在有效范围内 (1 到 missileCount)
            if (missileID_1Based < 1 || missileID_1Based > missileCount)
            {
                Debug.LogWarning($"导弹ID {missileID_1Based} 超出预设范围 [1, {missileCount}]，跳过文件: " + filePath);
                continue;
            }

            // 将1-based ID转换为0-based索引
            int missileIndex_0Based = missileID_1Based - 1;

            var lines = File.ReadAllLines(filePath);
            if (lines.Length < 2) // 至少要有表头和一行数据
            {
                Debug.LogWarning($"文件数据不足 (至少需要表头和一行数据)，跳过文件: {filePath}");
                continue;
            }

            for (int i = 1; i < lines.Length; i++) // 跳过表头
            {
                var cols = lines[i].Split(',');
                // 检查列数：时间, x, y, z, pitch, yaw (共6列)
                if (cols.Length < 6)
                {
                    Debug.LogWarning($"文件 {filePath} 中第 {i} 行的列数不足 (至少需要6列)，跳过该行。");
                    continue;
                }

                float time = float.Parse(cols[0]);
                float x = -float.Parse(cols[1]) / 200; // 第二列是x
                float y = float.Parse(cols[2]) / 200; // 第三列是y
                float z = float.Parse(cols[3]) / 200;  // 第四列是z
                float pitch = float.Parse(cols[4]);  // 新增：第五列是俯仰角
                float yaw = float.Parse(cols[5]);    // 新增：第六列是偏航角

                missileTrajectories[missileIndex_0Based].Add(new MissileFrame
                {
                    time = time,
                    position = new Vector3(x, y, z),
                    pitch = pitch, // 存储俯仰角
                    yaw = yaw      // 存储偏航角
                });
            }
        }
        Debug.Log("导弹轨迹数据加载完成。");
    }

    /// <summary>
    /// 加载簇结果
    /// 假设 clustering_results.csv 文件的格式为：
    /// missile_id, feature1, feature2, ..., cluster_id
    /// 其中 missile_id 是第一列 (1-based)，cluster_id 是最后一列。
    /// </summary>
    void LoadClusterResults()
    {
        // 初始化列表，大小为 missileCount，索引从0到missileCount-1
        missileClusterIDs = new List<int>(new int[missileCount]);

        string clusterResultsPath = @"D:\project\Matlab\Test\2025_7_17_cluster_collaboration\data\clustering_results.csv";

        if (!File.Exists(clusterResultsPath))
        {
            Debug.LogError("找不到簇结果文件: " + clusterResultsPath);
            return;
        }

        var lines = File.ReadAllLines(clusterResultsPath);
        if (lines.Length < 2) // 至少要有表头和一行数据
        {
            Debug.LogWarning("簇结果文件数据不足 (至少需要表头和一行数据)。");
            return;
        }

        for (int i = 1; i < lines.Length; i++) // 跳过表头
        {
            var cols = lines[i].Split(',');
            if (cols.Length < 2) // 至少需要导弹ID和簇ID
            {
                Debug.LogWarning($"簇结果文件 {clusterResultsPath} 中第 {i} 行的列数不足，跳过该行。");
                continue;
            }

            if (!int.TryParse(cols[0], out int missileID_1Based)) // 假设第一列是导弹ID (1-based)
            {
                Debug.LogWarning($"无法从簇结果文件 {clusterResultsPath} 中解析导弹ID，跳过该行: {lines[i]}");
                continue;
            }

            if (!int.TryParse(cols[cols.Length - 1], out int clusterID)) // 假设最后一列是簇ID
            {
                Debug.LogWarning($"无法从簇结果文件 {clusterResultsPath} 中解析簇ID，跳过该行: {lines[i]}");
                continue;
            }

            // 检查1-based导弹ID是否在有效范围内 (1 到 missileCount)
            if (missileID_1Based < 1 || missileID_1Based > missileCount)
            {
                Debug.LogWarning($"簇结果文件中导弹ID {missileID_1Based} 超出预设范围 [1, {missileCount}]，跳过该行。");
                continue;
            }

            // 检查簇ID是否在有效颜色范围内
            if (clusterID < 0 || clusterID >= clusterColors.Length)
            {
                Debug.LogWarning($"簇结果文件中簇ID {clusterID} 超出预设颜色数组范围 [0, {clusterColors.Length - 1}]，该导弹将使用默认颜色 (簇ID 0)。");
                clusterID = 0; // 默认使用第一个颜色
            }

            // 将1-based ID转换为0-based索引
            int missileIndex_0Based = missileID_1Based - 1;
            missileClusterIDs[missileIndex_0Based] = clusterID;
        }
        Debug.Log("簇结果数据加载完成。");
    }


    void CreateMissiles()
    {        
        missiles.Clear(); // 清空之前的导弹实例

        // 清空并初始化闪烁物体列表和爆炸状态列表
        flickerObjects.Clear();
        missileExplodedStatus.Clear();
        for (int i = 0; i < missileCount; i++)
        {
            flickerObjects.Add(null); // 预留位置
            missileExplodedStatus.Add(false); // 初始都未爆炸
        }

        GameObject missilePool = GameObject.Find("MissilePool");
        if (missilePool == null)
        {
            Debug.LogError("MissilePool 未找到，请确保场景中有名为 MissilePool 的GameObject。");
            return;
        }
        // Quaternion y90 = Quaternion.Euler(0, 90, 0);
        for (int i = 0; i < missileCount; i++) // i 是0-based索引
        {
            // 检查是否有当前导弹ID的轨迹数据
            if (missileTrajectories.Count <= i || missileTrajectories[i].Count == 0)
            {
                Debug.LogWarning($"内部索引 {i} (对应导弹ID {i + 1}) 没有加载到轨迹数据，跳过创建该导弹。");
                missiles.Add(null); // 添加一个null占位符，保持missiles列表大小与missileCount一致
                continue;
            }

            // 生成导弹
            var missile = Instantiate(missilePrefab, missileTrajectories[i][0].position, Quaternion.Euler(-missileTrajectories[i][0].pitch, -missileTrajectories[i][0].yaw + 90, 0), missilePool.transform);
            missile.name = "Missile_" + (i + 1); // 命名为1-based ID
            missiles.Add(missile);

            // 生成闪烁物体
            if (flickerPrefab != null)
            {
                var flickerObj = Instantiate(flickerPrefab, missile.transform.position + new Vector3(0, 5, 0), Quaternion.identity, missilePool.transform);
                var flickerScript = flickerObj.GetComponent<ObjectFlicker>();
                if (flickerScript != null)
                {
                    flickerScript.target = missile.transform; // 闪烁物体跟随导弹移动
                }

                // 存储闪烁物体引用
                flickerObjects[i] = flickerObj;

                // 根据簇ID设置闪烁物体颜色
                if (missileClusterIDs != null && i < missileClusterIDs.Count)
                {
                    int clusterID = missileClusterIDs[i]; // i 是0-based索引
                    if (clusterID >= 0 && clusterID < clusterColors.Length)
                    {
                        Renderer flickerRenderer = flickerObj.GetComponent<Renderer>();
                        if (flickerRenderer != null)
                        {
                            flickerRenderer.material.color = clusterColors[clusterID];
                        }
                        else
                        {
                            Debug.LogWarning($"闪烁物体 {flickerObj.name} 没有 Renderer 组件，无法设置颜色。");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"内部索引 {i} (对应导弹ID {i + 1}) 的簇ID {clusterID} 无效或超出颜色数组范围，使用默认颜色。");
                    }
                }
                else
                {
                    Debug.LogWarning($"内部索引 {i} (对应导弹ID {i + 1}) 没有对应的簇ID信息，闪烁物体使用默认颜色。");
                }
            }
            else
            {
                Debug.LogWarning("flickerPrefab 未赋值，未生成闪烁物体。");
            }
        }
    }
}
