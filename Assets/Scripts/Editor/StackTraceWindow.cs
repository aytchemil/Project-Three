using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class StackTraceWindow : EditorWindow
{
    private Vector2 scrollPos;
    private List<string> stackTraceLines = new List<string>();

    [MenuItem("Window/Stack Trace Viewer")]
    public static void ShowWindow()
    {
        GetWindow<StackTraceWindow>("Stack Trace Viewer");
    }

    void OnEnable()
    {
        stackTraceLines.Clear();
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            stackTraceLines.Clear();
            // Add the error message at the bottom
            stackTraceLines.Add(logString);
            // Process stack trace in reverse order
            string[] lines = stackTrace.Split('\n');
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                string line = lines[i].Trim();
                if (!string.IsNullOrEmpty(line))
                {
                    stackTraceLines.Insert(0, line); // Insert at the top to maintain reverse order
                }
            }
            Repaint();
        }
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < stackTraceLines.Count; i++)
        {
            string line = stackTraceLines[i];
            Match match = Regex.Match(line, @"(Assets/[\w/]+\.cs):(\d+)");
            if (match.Success)
            {
                string filePath = match.Groups[1].Value;
                int lineNumber = int.Parse(match.Groups[2].Value);
                string scriptName = System.IO.Path.GetFileNameWithoutExtension(filePath);

                if (GUILayout.Button($"[{scriptName}] {line}"))
                {
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath, lineNumber);
                }
            }
            else
            {
                // Display non-file lines (e.g., internal calls or error message) as plain text
                GUILayout.Label(line);
            }
        }

        EditorGUILayout.EndScrollView();
    }
}