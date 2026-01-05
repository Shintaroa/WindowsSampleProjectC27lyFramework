using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace TFramework
{
	/// <summary>
	/// todo: 通过unitask 来控制顺序 先执行systemconfig 再执行systemcontroller
	/// systemcontroller只负责屏幕位置 光标显示和帧率 logo图标和日志分离到其他地方
	/// </summary>
    public class DisplayController : MonoSingleton<DisplayController>
    {
	    #region User32.dll

	    [DllImport("user32.dll", CharSet = CharSet.Auto)]
	    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hPos, int X, int Y, int cx, int cy, uint uFlags);

	    [DllImport("user32.dll")]
	    public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childe, string strclass, string strname);

	    [DllImport("User32.dll", CharSet = CharSet.Auto)]
	    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	    [DllImport("User32.dll", CharSet = CharSet.Auto)]
	    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

	    [DllImport("User32.dll")]
	    private static extern int GetWindowLong(IntPtr hWnd, int dwNewLong);

	    [DllImport("User32.dll")]
	    private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);

	    [DllImport("user32.dll", CharSet = CharSet.Auto)]
	    public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

	    [DllImport("user32.dll", CharSet = CharSet.Auto)]
	    public static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wP, IntPtr IP);

	    [DllImport("user32.dll", CharSet = CharSet.Auto)]
	    public static extern IntPtr SetParent(IntPtr hChild, IntPtr hParent);

	    [DllImport("user32.dll", CharSet = CharSet.Auto)]
	    public static extern IntPtr GetParent(IntPtr hChild);

	    [DllImport("User32.dll")]
	    public static extern IntPtr GetSystemMetrics(int nIndex);

	    [DllImport("user32.dll")]
	    public static extern bool SetForegroundWindow(IntPtr hWnd);

	    [DllImport("user32.dll")]
	    static extern IntPtr GetForegroundWindow();
	    #endregion

	    private Rect _screenPosition;
	    
	    [Header("Logo 图标")] public GameObject objLogo;
	    private IntPtr _hWnd;

	    private SystemConfig _config;
	    
	    private int _xScreen;
	    private int _yScreen; 
	    
	    public void Init()
	    {
		    _config = SystemConfig.Instance;
		    this._hWnd = FindWindow(null, Application.productName);
		    SetConfigValue();
	    }

	    public void Reset()
	    {
		    SetConfigValue();
	    }

	    void Update()
	    {
		    //鼠标显示/隐藏
		    if (Input.GetKeyDown(KeyCode.C))
		    {
			    Cursor.visible = !Cursor.visible;
		    }
		    
		    //窗口左移-非全屏模式下
		    if (Input.GetKeyDown(KeyCode.LeftArrow))
		    {
			    ChangeScreenPosition(new Vector2(-5f, 0f));
		    }
		    
		    //窗口右移-非全屏模式下
		    if (Input.GetKeyDown(KeyCode.RightArrow))
		    {
			    ChangeScreenPosition(new Vector2(5f, 0f));
		    }
		    
		    //窗口上移-非全屏模式下
		    if (Input.GetKeyDown(KeyCode.UpArrow))
		    {
			    ChangeScreenPosition(new Vector2(0f, -5f));
		    }
		    
		    //窗口下移-非全屏模式下
		    if (Input.GetKeyDown(KeyCode.DownArrow))
		    {
			    ChangeScreenPosition(new Vector2(0f, 5f));
		    }
	    }

	    public void SetFrameRate(int frameRate)
	    {
		    Application.targetFrameRate = frameRate;
	    }


	    //初始化显示设置
	    private void SetConfigValue()
	    {
		    SetFrameRate(_config.FrameRate);
		    if (Application.isEditor || !IsWindowsPlatform()) { return; }

		    Cursor.visible = _config.CursorVisible;
			if(objLogo != null)
				objLogo.SetActive(_config.logo);
			_xScreen = (int)GetSystemMetrics(SystemConfig.SM_CXSCREEN);
			_yScreen = (int)GetSystemMetrics(SystemConfig.SM_CYSCREEN);
			SetWindowsStyle(_config.AppWindowStyle);
		    StartCoroutine(IE_SetWindows());
	    }

	    public void SetWindowsStyle(SystemConfig.APP_STYLE appStyle)
	    {
		    switch (_config.AppWindowStyle)
		    {
			    case SystemConfig.APP_STYLE.WindowedFullScreen:
				    Screen.SetResolution(_xScreen, _yScreen, true);
				    break;
			    case SystemConfig.APP_STYLE.Windowed:
				    Screen.SetResolution(_config.windowWidth, _config.windowHeight, false);
				    break;
			    case SystemConfig.APP_STYLE.WindowedWithoutBorder:
				    Screen.SetResolution(_config.windowWidth, _config.windowHeight, false);
				    _screenPosition = new Rect((float)_config.windowLeft, (float)_config.windowTop, (float)_config.windowWidth, (float)_config.windowHeight);
				    break;
			    case SystemConfig.APP_STYLE.WindowMin:
				    ShowWindow(GetForegroundWindow(), SystemConfig.SW_SHOWMINIMIZED);
				    break;
		    }
	    }


	    //变更窗体XY坐标
	    private void ChangeScreenPosition(Vector2 pos)
	    {
		    _screenPosition.x +=  pos.x;
		    _screenPosition.y +=  pos.y;
		    StartCoroutine(IE_SetWindows());
	    }
	    
	    //设置窗口
	    private IEnumerator IE_SetWindows()
	    {
		    yield return new WaitForEndOfFrame();
		    yield return new WaitForFixedUpdate();
		    SetWindows();
		    yield break;
	    }

	    private void SetWindows()
	    {
		    if (Application.isEditor || !IsWindowsPlatform()) { return; }
		    Vector2Int windowSize = CalculateWindowSize(_config.AppWindowStyle);
		    Vector2Int windowPos = CalculateWindowPosition(_config.AppWindowStyle);
		    
		    IntPtr hWndInsertAfter = GetWindowZOrderFlag(_config.ScreenDepth);
		    
		    if (_config.AppWindowStyle == SystemConfig.APP_STYLE.WindowedFullScreen || 
		        _config.AppWindowStyle == SystemConfig.APP_STYLE.WindowedWithoutBorder)
		    {
			    SetWindowLong(this._hWnd, -16, 369164288); // GWL_STYLE
		    }
		    
		    uint flags = (_config.AppWindowStyle == SystemConfig.APP_STYLE.Windowed) ? 35U : 64U;
		    SetWindowPos(this._hWnd, hWndInsertAfter, windowPos.x, windowPos.y, windowSize.x, windowSize.y, flags);
	    }
	    
	    private IntPtr GetWindowZOrderFlag(SystemConfig.Z_DEPTH depth)
	    {
		    // 将深度枚举映射到Windows API常量
		    switch (depth)
		    {
			    case SystemConfig.Z_DEPTH.Bottom:
				    return _config.HWND_BOTTOM; // 通常为 (IntPtr)1
			    case SystemConfig.Z_DEPTH.Normal:
				    return _config.HWND_NORMAL; // 通常为 IntPtr.Zero
			    case SystemConfig.Z_DEPTH.Top:
				    return _config.HWND_TOP; // 通常为 (IntPtr)0
			    case SystemConfig.Z_DEPTH.TopMost:
				    return _config.HWND_TOPMOST; // 通常为 (IntPtr)-1
			    default:
				    return _config.HWND_NORMAL;
		    }
	    }
	    
	    private Vector2Int CalculateWindowSize(SystemConfig.APP_STYLE style)
	    {
		    switch (style)
		    {
			    case SystemConfig.APP_STYLE.WindowedFullScreen:
				    return new Vector2Int(_xScreen, _yScreen);
			    case SystemConfig.APP_STYLE.Windowed:
				    return Vector2Int.zero;
			    case SystemConfig.APP_STYLE.WindowedWithoutBorder:
				    return new Vector2Int((int)_screenPosition.width, (int)_screenPosition.height);
			    default:
				    return Vector2Int.zero;
		    }
	    }
	    
	    private Vector2Int CalculateWindowPosition(SystemConfig.APP_STYLE style)
	    {
		    switch (style)
		    {
			    case SystemConfig.APP_STYLE.WindowedFullScreen:
				    return Vector2Int.zero;
			    case SystemConfig.APP_STYLE.Windowed:
				    return Vector2Int.zero;
			    case SystemConfig.APP_STYLE.WindowedWithoutBorder:
				    return new Vector2Int((int)_screenPosition.x, (int)_screenPosition.y);
			    default:
				    return Vector2Int.zero;
		    }
	    }
	    
	    
	    
	    private bool IsWindowsPlatform()
	    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
		    return true;
#else
    Debug.LogWarning("窗口设置功能仅支持Windows平台。");
    return false;
#endif
	    }

	    #region 设置窗口公开函数
		public void SetWindowMin()
	    {
		    IntPtr intPtr = this._hWnd;
		    if (!Application.isEditor)
		    {
			    ShowWindow(this._hWnd, 2);
		    } 
	    }
		
		public void SetWindowNormal()
		{
			IntPtr intPtr = this._hWnd;
			if (!Application.isEditor)
			{
				ShowWindow(this._hWnd, 1);
			}
		}
		
		public void SetWindowMax()
		{
			IntPtr intPtr = this._hWnd;
			if (!Application.isEditor)
			{
				ShowWindow(this._hWnd, 3);
			}
		}

		public void SetWindowBottom()
		{
			_config.ScreenDepth = SystemConfig.Z_DEPTH.Bottom;
			SetConfigValue();
		}

		public void SetWindowTop()
		{
			_config.ScreenDepth = SystemConfig.Z_DEPTH.Top;
			SetConfigValue();
		}

		public void SetWindowTopMost()
		{
			_config.ScreenDepth = SystemConfig.Z_DEPTH.TopMost;
			SetConfigValue();
		}
		#endregion
		
    }
}