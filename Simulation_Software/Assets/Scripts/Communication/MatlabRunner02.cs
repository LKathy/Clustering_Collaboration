using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Collections; // ������������Э��

public class MatlabRunner02 : MonoBehaviour
{
    public Button runMatlabButton; // �󶨵�UI��ť
    private Process matlabProcess;
    public Text logText; // ���������󶨵�UI Text�����������ʾLog��Ϣ

    void Start()
    {
        // ��ť����¼�
        runMatlabButton.onClick.AddListener(RunMatlab);

        // ����������ʼ��ʱ����Log�ı�
        if (logText != null)
        {
            logText.gameObject.SetActive(false);
        }
    }

    void RunMatlab()
    {
        if (matlabProcess != null && !matlabProcess.HasExited)
        {
            UnityEngine.Debug.LogWarning("MATLAB�Ѿ��ɹ�����!");
            ShowLogMessage("MATLAB�Ѿ��ɹ�����!"); // ���޸ġ���ʾ��UI
            return;
        }

        // ���� MATLAB ����
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "matlab.exe";
        startInfo.Arguments = "-nosplash -nodesktop -r \"run('D:\\project\\Matlab\\Test\\2025_7_17_cluster_collaboration\\GetMissileNumFromUnity.m');\"";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        matlabProcess = new Process();
        matlabProcess.StartInfo = startInfo;
        matlabProcess.EnableRaisingEvents = true;
        matlabProcess.Exited += (sender, e) =>
        {
            UnityEngine.Debug.Log("MATLAB������������");
            ShowLogMessage("MATLAB������������"); // ���޸ġ���ʾ��UI
        };

        try
        {
            matlabProcess.Start();
            UnityEngine.Debug.Log("MATLAB�����ɹ���");
            ShowLogMessage("MATLAB�����ɹ���"); // ���޸ġ���ʾ��UI
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("����MATLABʧ��: " + e.Message);
            ShowLogMessage("����MATLABʧ��: " + e.Message); // ���޸ġ���ʾ��UI
        }
    }

    // ����������ʾLog��Ϣ��UI�ķ���
    void ShowLogMessage(string message)
    {
        if (logText != null)
        {
            logText.text = message;
            logText.gameObject.SetActive(true);

            // ֹ֮ͣǰ��Э�̣�����еĻ���
            StopAllCoroutines();

            // ����2������ص�Э��
            StartCoroutine(HideLogAfterDelay(2f));
        }
    }

    // ��������2�������Log��Э��
    IEnumerator HideLogAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (logText != null)
        {
            logText.gameObject.SetActive(false);
        }
    }

    //void OnDestroy()
    //{
    //    // �ر� MATLAB ���̣���ѡ��
    //    if (matlabProcess != null && !matlabProcess.HasExited)
    //    {
    //        matlabProcess.Kill();
    //    }
    //}
}