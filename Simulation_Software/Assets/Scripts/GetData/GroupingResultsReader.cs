using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GroupingResultsReader : MonoBehaviour
{
    public Button groupingResultsButton;
    public GameObject tablePanel; // TablePanel对象（需挂VerticalLayoutGroup+ContentSizeFitter）
    public GameObject headerPanel; // 新增：表头Panel
    public GameObject cellPrefab; // 单元格预制体（需挂LayoutElement）
    public GameObject resultBG;
    public string csvFilePath = @"D:\project\Matlab\Test\2025_7_17_cluster_collaboration\data\clustering_results.csv";

    void Start()
    {
        if (resultBG != null)
        {
            resultBG.SetActive(false); // 启动时隐藏背景
        }
            
        groupingResultsButton.onClick.AddListener(OnGroupingResultsClicked);

        // 设置数据区的VerticalLayoutGroup属性
        var vlg = tablePanel.GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
        {
            vlg.spacing = 2f;
            vlg.padding.top = 0; // 表格整体上方留白
            vlg.padding.bottom = 0;
            vlg.childForceExpandHeight = false;
            vlg.childControlHeight = true;
        }
    }

    void OnGroupingResultsClicked()
    {
        if (resultBG != null)
        {
            // 切换显示/隐藏
            bool isActive = resultBG.activeSelf;
            resultBG.SetActive(!isActive);

            // 如果隐藏则不刷新表格内容
            if (!resultBG.activeSelf)
                return;
        }
        // 清空旧表格内容
        foreach (Transform child in tablePanel.transform)
            Destroy(child.gameObject);

        // 清空旧表头
        foreach (Transform child in headerPanel.transform)
            Destroy(child.gameObject);

        if (File.Exists(csvFilePath))
        {
            string[] lines = File.ReadAllLines(csvFilePath);
            if (lines.Length == 0) return;

            string[] headers = lines[0].Split(',');
            int colCount = headers.Length;
            List<List<string>> allRows = new List<List<string>>();
            allRows.Add(new List<string>(headers));
            for (int i = 1; i < lines.Length; i++)
            {
                string[] cells = lines[i].Split(',');
                allRows.Add(new List<string>(cells));
            }

            // 1. 计算每一列的最大宽度（先用表头内容测量）
            float[] maxColWidths = new float[colCount];

            // 临时生成一个Canvas用于测量
            GameObject tempCanvasObj = new GameObject("TempCanvas");
            Canvas tempCanvas = tempCanvasObj.AddComponent<Canvas>();
            tempCanvas.renderMode = RenderMode.WorldSpace;

            float cellPadding = 24f; // 单元格左右padding，可根据实际UI调整

            for (int col = 0; col < colCount; col++)
            {
                // 用表头内容初始化
                GameObject tempCell = Instantiate(cellPrefab, tempCanvasObj.transform);
                Text tempText = tempCell.GetComponent<Text>();
                tempText.text = headers[col].Trim();
                tempText.fontStyle = FontStyle.Bold;
                tempText.horizontalOverflow = HorizontalWrapMode.Overflow;
                tempText.verticalOverflow = VerticalWrapMode.Overflow;
                tempText.resizeTextForBestFit = false;
                tempCell.SetActive(true);
                tempText.enabled = true;
                tempText.rectTransform.ForceUpdateRectTransforms();
                float width = tempText.preferredWidth + cellPadding;
                maxColWidths[col] = width;
                Destroy(tempCell);
            }

            // 用数据行内容更新最大宽度
            for (int row = 1; row < allRows.Count; row++)
            {
                var cells = allRows[row];
                for (int col = 0; col < colCount && col < cells.Count; col++)
                {
                    GameObject tempCell = Instantiate(cellPrefab, tempCanvasObj.transform);
                    Text tempText = tempCell.GetComponent<Text>();
                    tempText.text = cells[col].Trim();
                    tempText.fontStyle = FontStyle.Normal;
                    tempText.horizontalOverflow = HorizontalWrapMode.Overflow;
                    tempText.verticalOverflow = VerticalWrapMode.Overflow;
                    tempText.resizeTextForBestFit = false;
                    tempCell.SetActive(true);
                    tempText.enabled = true;
                    tempText.rectTransform.ForceUpdateRectTransforms();
                    float width = tempText.preferredWidth + cellPadding;
                    if (width > maxColWidths[col])
                        maxColWidths[col] = width;
                    Destroy(tempCell);
                }
            }
            Destroy(tempCanvasObj);

            // 2. 生成表头到headerPanel
            CreateRow(headers, true, maxColWidths, headerPanel);

            // 3. 生成数据行到tablePanel
            for (int i = 1; i < allRows.Count; i++)
            {
                CreateRow(allRows[i].ToArray(), false, maxColWidths, tablePanel);
            }
        }
        else
        {
            // 显示错误信息
            GameObject row = new GameObject("Row", typeof(RectTransform));
            row.transform.SetParent(tablePanel.transform, false);
            var hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.spacing = 10f; // 保证错误信息也有间隙
            var csf = row.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            GameObject cellObj = Instantiate(cellPrefab, row.transform);
            Text cellText = cellObj.GetComponent<Text>();
            if (cellText != null)
                cellText.text = "文件未找到: " + csvFilePath;
        }
    }

    // 新增 parent 参数，支持表头和数据行分别生成
    GameObject CreateRow(string[] cells, bool isHeader, float[] colWidths, GameObject parent)
    {
        GameObject row = new GameObject(isHeader ? "HeaderRow" : "Row", typeof(RectTransform));
        row.transform.SetParent(parent.transform, false);

        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.spacing = 4f;
        var csf = row.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        for (int i = 0; i < colWidths.Length; i++)
        {
            string cellTextValue = (i < cells.Length) ? cells[i].Trim() : "";
            GameObject cellObj = Instantiate(cellPrefab, row.transform);
            Text cellText = cellObj.GetComponent<Text>();
            if (cellText != null)
            {
                cellText.text = cellTextValue;
                cellText.alignment = TextAnchor.MiddleCenter;
                if (isHeader)
                    cellText.fontStyle = FontStyle.Bold;
            }
            var layout = cellObj.GetComponent<LayoutElement>();
            if (layout != null)
            {
                layout.preferredWidth = colWidths[i];
            }
        }
        return row;
    }
}
