using System.Diagnostics;
using System.IO;
using UnityEngine;

public class MatlabRunner : MonoBehaviour
{
    // 点击按钮时调用此方法
    public void RunMatlabScript()
    {
        // MATLAB 脚本路径（确保文件名没有扩展名）
        string scriptName = "main_gpt02";
        string matlabScriptFolder = @"D:\project\Matlab\Test\2025_7_17_cluster_collaboration";

        // 构造启动 MATLAB 的进程信息
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "matlab"; // 默认假设系统环境变量已配置好 matlab.exe 路径
        startInfo.Arguments = $"-sd \"{matlabScriptFolder}\" -batch \"{scriptName}\"";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        try
        {
            using (Process matlabProcess = new Process())
            {
                matlabProcess.StartInfo = startInfo;
                matlabProcess.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log("MATLAB: " + args.Data);
                matlabProcess.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError("MATLAB ERROR: " + args.Data);

                matlabProcess.Start();
                matlabProcess.BeginOutputReadLine();
                matlabProcess.BeginErrorReadLine();

                // 可选：等待 MATLAB 执行完毕（如果想异步运行可删除下面这行）
                matlabProcess.WaitForExit();
                UnityEngine.Debug.Log("MATLAB 脚本执行完成！");
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("运行 MATLAB 脚本失败: " + ex.Message);
        }
    }
}
