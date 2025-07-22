using System.Diagnostics;
using System.IO;
using UnityEngine;

public class MatlabRunner : MonoBehaviour
{
    // �����ťʱ���ô˷���
    public void RunMatlabScript()
    {
        // MATLAB �ű�·����ȷ���ļ���û����չ����
        string scriptName = "main_gpt02";
        string matlabScriptFolder = @"D:\project\Matlab\Test\2025_7_17_cluster_collaboration";

        // �������� MATLAB �Ľ�����Ϣ
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "matlab"; // Ĭ�ϼ���ϵͳ�������������ú� matlab.exe ·��
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

                // ��ѡ���ȴ� MATLAB ִ����ϣ�������첽���п�ɾ���������У�
                matlabProcess.WaitForExit();
                UnityEngine.Debug.Log("MATLAB �ű�ִ����ɣ�");
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("���� MATLAB �ű�ʧ��: " + ex.Message);
        }
    }
}
