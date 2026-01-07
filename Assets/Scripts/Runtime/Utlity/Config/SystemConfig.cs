using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using TFramework;
using UnityEngine;
using UnityEngine.Networking;

public class SystemConfig : Singleton<SystemConfig>
{
	#region 基础参数
	public APP_STYLE AppWindowStyle;
	public Z_DEPTH ScreenDepth;
	public int windowLeft;
	public int windowTop;
	public int windowWidth = Screen.width;
	public int windowHeight = Screen.height;
	public bool CursorVisible = false;
	public int FrameRate = 60;
	
	public const uint SWP_SHOWWINDOW = 64U;
	public const int GWL_STYLE = -16;
	public const int WS_BORDER = 1;
	public const int SW_SHOWRESTORE = 1;
	public const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}
	public const int SW_SHOWMAXIMIZED = 3;
	public const int GWL_EXSTYLE = -20;
	public const int WS_CAPTION = 12582912;
	public const int WS_POPUP = 8388608;
	
	public IntPtr HWND_BOTTOM = new IntPtr(1);
	public IntPtr HWND_NORMAL = new IntPtr(-2);
	public IntPtr HWND_TOP = new IntPtr(0);
	public IntPtr HWND_TOPMOST = new IntPtr(-1);
	
	public const int SM_CXSCREEN = 0;
	public const int SM_CYSCREEN = 1;
	
	public bool logo = true;
	public bool log = true;
	public LogFile.LOG_TYPE logType { get; private set; } = LogFile.LOG_TYPE.All;
	public float backTime { get; private set; } = 120;
	    
	public enum APP_STYLE
	{
		WindowedFullScreen,				//全屏
		Windowed,						//窗口化
		WindowedWithoutBorder,			//窗口化无边框
		WindowMin						//最小化
	}

	public enum Z_DEPTH
	{
		Bottom = -1,					//最底部
		Normal,							//正常
		Top,							//启动时置顶
		TopMost							//永久置顶
	}
	#endregion
	
	public async UniTask Init()
	{
		string systemPath = Application.streamingAssetsPath + "/Configs/system.ini";
		if (File.Exists(systemPath)) 
		{
			UnityWebRequest www = await UnityWebRequest.Get("file://" + systemPath).SendWebRequest();
			if (string.IsNullOrEmpty(www.error))
			{
				string[] infoArray = www.downloadHandler.text.Split('\n');

				if (infoArray.Length > 0)
				{
					Dictionary<string, string> infoDic = new ();
					foreach (string str in infoArray)
					{
						if (!str.Contains("@"))
							continue;
						string[] allArray = str.Split('@');
						string id = allArray[0];
						string info = allArray[1].Replace("\r", "");
						infoDic.Add(id, info);
					}
					SetDefaultValue(infoDic);
				}
			}
			else
			{
				Debug.Log(www.error);
			}
		}
		/*
		 *
		 * //整屏缩放，以1920*1080为基准
		if (root != null)
			root.localScale = new Vector3(this.windowWidth / 1920.0f, this.windowHeight/ 1080.0f, 1);
		// todo: 读取完毕之后使用unitask来调用systemcontroller来实现初始化(已完成)
		Init();*/
	}

	private void SetDefaultValue(Dictionary<string, string> infoDic)
	{
		//窗口偏移
		windowLeft = int.Parse(infoDic["left"]);
		windowTop = int.Parse(infoDic["top"]);
		//分辨率
		windowWidth = int.Parse(infoDic["width"]);
		windowHeight = int.Parse(infoDic["height"]);
		//鼠标
		CursorVisible = infoDic["showmouse"] == "true" ? true : false;
		//全屏 全屏=0 窗口=1 窗口无边框=2 最小化=3
		AppWindowStyle = (APP_STYLE)(int.Parse(infoDic["fullscreen"]));
		//置顶 置底=-1 普通=0 开启时置顶=1 永久置顶=2
		ScreenDepth = (Z_DEPTH)(int.Parse(infoDic["screendepth"]));
		//日志过滤 全部=0 Warning以上=1 Assert以上=2 Error以上=3 不输出=4
		logType = (LogFile.LOG_TYPE)(int.Parse(infoDic["logtype"]));
		//日志
		log = infoDic["log"] == "true" ? true : false;
		//帧数
		FrameRate = int.Parse(infoDic["framerate"]);
		//待机时间
		backTime = int.Parse(infoDic["backtime"]);
		//logo
		logo = infoDic["logo"] == "true" ? true : false;
	}
}
