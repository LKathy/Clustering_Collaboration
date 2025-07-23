using UnityEngine;
using System.Diagnostics;
using System.IO;

public class MatlabRunner01 : MonoBehaviour
{
    public void OnClickRunMatlab()
    {
        // MATLAB 可执行程序路径（请根据你安装的 MATLAB 路径修改）
        string matlabPath = @"D:\software\matlab\bin\matlab.exe";

        // 要运行的 .m 脚本所在的文件夹
        string scriptDirectory = @"D:\project\Matlab\Test\2025_7_17_cluster_collaboration";

        // 要运行的脚本名（无扩展名）
        string scriptName = "GetMissileNumFromUnity";

        // 构造启动参数
        string arguments = $"-batch \"cd('{scriptDirectory}'); {scriptName}\"";

        // 启动 MATLAB
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = matlabPath;
        startInfo.Arguments = arguments;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true; // 设置为 false 可显示 MATLAB 界面
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        try
        {
            Process matlabProcess = Process.Start(startInfo);
            UnityEngine.Debug.Log("MATLAB 脚本已启动");           
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("启动 MATLAB 失败: " + ex.Message);
        }
    }
}
