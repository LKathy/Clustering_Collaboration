using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GroupingResultsReader : MonoBehaviour
{
    public Button groupingResultsButton;
    public GameObject tablePanel; // TablePanel�������VerticalLayoutGroup+ContentSizeFitter��
    public GameObject headerPanel; // ��������ͷPanel
    public GameObject cellPrefab; // ��Ԫ��Ԥ���壨���LayoutElement��
    public GameObject resultBG;
    public string csvFilePath = @"D:\project\Matlab\Test\2025_7_17_cluster_collaboration\data\clustering_results.csv";

    void Start()
    {
        if (resultBG != null)
        {
            resultBG.SetActive(false); // ����ʱ���ر���
        }
            
        groupingResultsButton.onClick.AddListener(OnGroupingResultsClicked);

        // ������������VerticalLayoutGroup����
        var vlg = tablePanel.GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
        {
            vlg.spacing = 2f;
            vlg.padding.top = 0; // ��������Ϸ�����
            vlg.padding.bottom = 0;
            vlg.childForceExpandHeight = false;
            vlg.childControlHeight = true;
        }
    }

    void OnGroupingResultsClicked()
    {
        if (resultBG != null)
        {
            // �л���ʾ/����
            bool isActive = resultBG.activeSelf;
            resultBG.SetActive(!isActive);

            // ���������ˢ�±������
            if (!resultBG.activeSelf)
                return;
        }
        // ��վɱ������
        foreach (Transform child in tablePanel.transform)
            Destroy(child.gameObject);

        // ��վɱ�ͷ
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

            // 1. ����ÿһ�е�����ȣ����ñ�ͷ���ݲ�����
            float[] maxColWidths = new float[colCount];

            // ��ʱ����һ��Canvas���ڲ���
            GameObject tempCanvasObj = new GameObject("TempCanvas");
            Canvas tempCanvas = tempCanvasObj.AddComponent<Canvas>();
            tempCanvas.renderMode = RenderMode.WorldSpace;

            float cellPadding = 24f; // ��Ԫ������padding���ɸ���ʵ��UI����

            for (int col = 0; col < colCount; col++)
            {
                // �ñ�ͷ���ݳ�ʼ��
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

            // �����������ݸ��������
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

            // 2. ���ɱ�ͷ��headerPanel
            CreateRow(headers, true, maxColWidths, headerPanel);

            // 3. ���������е�tablePanel
            for (int i = 1; i < allRows.Count; i++)
            {
                CreateRow(allRows[i].ToArray(), false, maxColWidths, tablePanel);
            }
        }
        else
        {
            // ��ʾ������Ϣ
            GameObject row = new GameObject("Row", typeof(RectTransform));
            row.transform.SetParent(tablePanel.transform, false);
            var hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.spacing = 10f; // ��֤������ϢҲ�м�϶
            var csf = row.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            GameObject cellObj = Instantiate(cellPrefab, row.transform);
            Text cellText = cellObj.GetComponent<Text>();
            if (cellText != null)
                cellText.text = "�ļ�δ�ҵ�: " + csvFilePath;
        }
    }

    // ���� parent ������֧�ֱ�ͷ�������зֱ�����
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
