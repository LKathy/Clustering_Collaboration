using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Collections; // 【新增】用于协程

public class MatlabRunner02 : MonoBehaviour
{
    public Button runMatlabButton; // 绑定到UI按钮
    private Process matlabProcess;
    public Text logText; // 【新增】绑定到UI Text组件，用于显示Log信息

    void Start()
    {
        // 按钮点击事件
        runMatlabButton.onClick.AddListener(RunMatlab);

        // 【新增】初始化时隐藏Log文本
        if (logText != null)
        {
            logText.gameObject.SetActive(false);
        }
    }

    void RunMatlab()
    {
        if (matlabProcess != null && !matlabProcess.HasExited)
        {
            UnityEngine.Debug.LogWarning("MATLAB已经成功运行!");
            ShowLogMessage("MATLAB已经成功运行!"); // 【修改】显示到UI
            return;
        }

        // 启动 MATLAB 进程
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
            UnityEngine.Debug.Log("MATLAB进程已启动！");
            ShowLogMessage("MATLAB进程已启动！"); // 【修改】显示到UI
        };

        try
        {
            matlabProcess.Start();
            UnityEngine.Debug.Log("MATLAB启动成功！");
            ShowLogMessage("MATLAB启动成功！"); // 【修改】显示到UI
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("启动MATLAB失败: " + e.Message);
            ShowLogMessage("启动MATLAB失败: " + e.Message); // 【修改】显示到UI
        }
    }

    // 【新增】显示Log信息到UI的方法
    void ShowLogMessage(string message)
    {
        if (logText != null)
        {
            logText.text = message;
            logText.gameObject.SetActive(true);

            // 停止之前的协程（如果有的话）
            StopAllCoroutines();

            // 启动2秒后隐藏的协程
            StartCoroutine(HideLogAfterDelay(2f));
        }
    }

    // 【新增】2秒后隐藏Log的协程
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
    //    // 关闭 MATLAB 进程（可选）
    //    if (matlabProcess != null && !matlabProcess.HasExited)
    //    {
    //        matlabProcess.Kill();
    //    }
    //}
}