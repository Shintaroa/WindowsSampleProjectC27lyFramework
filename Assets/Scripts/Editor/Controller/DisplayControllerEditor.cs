using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TFramework;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CustomEditor(typeof(Main))]
public class SystemControllerEditor : Editor
{
    //文件地址
    string systemPath = Application.streamingAssetsPath + "/Configs/system.ini";

    //系统配置参数
    string defaultSystemConfig = "##窗口偏移\n" + 
                                 "left@0\n" +
                                 "top@0\n\n" +
                                 "##分辨率\n" +
                                 "width@1920\n" +
                                 "height@1080\n\n" +
                                 "##鼠标\n" +
                                 "showmouse@false\n\n" +
                                 "##全屏 全屏=0 窗口=1 窗口无边框=2 最小化=3 \n" +
                                 "fullscreen@2\n\n" +
                                 "##置顶模式 置底=-1 普通=0 开启时置顶=1 永久置顶=2 \n" +
                                 "screendepth@2\n\n" +
                                 "##日志过滤 全部=0 Warning以上=1 Assert以上=2 Error以上=3 不输出=4\n" +
                                 "logtype@0\n\n" +
                                 "##日志\n" +
                                 "log@true\n\n" +
                                 "##帧数\n" +
                                 "framerate@30\n\n" +
                                 "##待机时间\n" +
                                 "backtime@120\n\n"+
                                 "##logo\n" +
                                 "logo@true";
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (GUILayout.Button("打开配置文件"))
        {
            if (!string.IsNullOrEmpty(systemPath) && File.Exists(systemPath))
            {
                Process.Start("notepad.exe", systemPath);
            }
            else
            {
                Debug.LogWarning("ini open faile, create new file :" + systemPath);
                
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Configs");
                FileStream fs = new FileStream(systemPath, FileMode.Create, FileAccess.ReadWrite);

                StreamWriter sw = new StreamWriter(fs);
                sw.Write(defaultSystemConfig);
                sw.Flush();
                sw.Close();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

}
