using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using GameCore.Core;

public static class ShellUtils
{

#if UNITY_STANDALONE_WIN
    private static readonly string _SHELL_CODE_PATH = "./TempBat.bat";
#elif UNITY_STANDALONE_OSX
    private static readonly string _SHELL_CODE_PATH = "./TempBat.sh";
#else
    private static readonly string _SHELL_CODE_PATH = "";
#endif

    public static string[] ExecuteShellCommand(string command, string path = "./", bool isRelative = true, bool outputResult = false, bool mayTimeout = false)
    {
#if UNITY_STANDALONE_WIN
        File.WriteAllText(Path.GetFullPath(_SHELL_CODE_PATH), command);
#else
        File.WriteAllText(Path.GetFullPath(_SHELL_CODE_PATH), $"#!/bin/bash\n{command}");
#endif

        string workingDirectory = isRelative ? Path.GetFullPath(path) : path;
        string result = string.Empty;

        if (mayTimeout)
        {
            result = ExecuteShellScriptWaitForExit(Path.GetFullPath(_SHELL_CODE_PATH), workingDirectory);
        }
        else
        {
            result = ExecuteShellScript(Path.GetFullPath(_SHELL_CODE_PATH), workingDirectory);
        }

        if (outputResult)
        {
            LoggerX.Debug("EXECUTE CMD:\n{0}\n----------\nRESULT:\n{1}", command, result);
        }

        return result.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static string ExecuteShellScript(string path, string workingDirectory)
    {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.UseShellExecute = false;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.CreateNoWindow = true;
        psi.WorkingDirectory = workingDirectory;

#if UNITY_STANDALONE_WIN
        psi.FileName = path;
#elif UNITY_STANDALONE_OSX
        psi.FileName = "/bin/bash";
        psi.Arguments = path;
#endif

        using(Process p = Process.Start(psi))
        using (StreamReader sout = p.StandardOutput)
        using (StreamReader eout = p.StandardError)
        {
            string result = sout.ReadToEnd().Trim();
            string error = eout.ReadToEnd().Trim();

            eout.Close();
            sout.Close();
            p.Close();

            if (string.IsNullOrEmpty(result))
            {
                throw new Exception(error);
            }
            else
            {
                return result;
            }
        }
    }

    private static string ExecuteShellScriptWaitForExit(string path, string workingDirectory)
    {
        var process = new System.Diagnostics.Process();
        ProcessStartInfo psi = process.StartInfo;
        psi.UseShellExecute = false;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.CreateNoWindow = true;
        psi.WorkingDirectory = workingDirectory;

#if UNITY_STANDALONE_WIN
        psi.FileName = path;
#elif UNITY_STANDALONE_OSX
        psi.FileName = "/bin/bash";
        psi.Arguments = path;
#endif

        StringBuilder output = new StringBuilder();
        StringBuilder error = new StringBuilder();

        using (var outputWaitHandle = new System.Threading.AutoResetEvent(false))
        using (var errorWaitHandle = new System.Threading.AutoResetEvent(false))
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    outputWaitHandle.Set();
                }
                else
                {
                    output.AppendLine(e.Data);
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    errorWaitHandle.Set();
                }
                else
                {
                    error.AppendLine(e.Data);
                }
            };
            // http://stackoverflow.com/questions/139593/processstartinfo-hanging-on-waitforexit-why
            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            // Wait for the reading threads to complete.
            outputWaitHandle.WaitOne();
            errorWaitHandle.WaitOne();

            if (process.ExitCode != 0)
            {
                throw new Exception(error.ToString());
            }
            else
            {
                return output.ToString();
            }
        }
    }
}