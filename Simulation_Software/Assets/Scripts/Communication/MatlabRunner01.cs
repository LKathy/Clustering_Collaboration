using UnityEngine;
using System.Diagnostics;
using System.IO;

public class MatlabRunner01 : MonoBehaviour
{
    public void OnClickRunMatlab()
    {
        // MATLAB ��ִ�г���·����������㰲װ�� MATLAB ·���޸ģ�
        string matlabPath = @"D:\software\matlab\bin\matlab.exe";

        // Ҫ���е� .m �ű����ڵ��ļ���
        string scriptDirectory = @"D:\project\Matlab\Test\2025_7_17_cluster_collaboration";

        // Ҫ���еĽű���������չ����
        string scriptName = "GetMissileNumFromUnity";

        // ������������
        string arguments = $"-batch \"cd('{scriptDirectory}'); {scriptName}\"";

        // ���� MATLAB
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = matlabPath;
        startInfo.Arguments = arguments;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true; // ����Ϊ false ����ʾ MATLAB ����
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        try
        {
            Process matlabProcess = Process.Start(startInfo);
            UnityEngine.Debug.Log("MATLAB �ű�������");           
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("���� MATLAB ʧ��: " + ex.Message);
        }
    }
}
