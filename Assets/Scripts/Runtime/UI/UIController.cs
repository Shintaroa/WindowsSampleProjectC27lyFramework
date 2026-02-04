using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
#if L2
using I2.Loc;
#endif
using UnityEngine;

//todo:这些代码要加一个正在切换的状态需要使用unitask进行优化
//todo:需要一个yooasset来进行加载和内存释放
public partial class UIController : MonoSingleton<UIController>
{
    private Dictionary<string, IUIElement> uis = new Dictionary<string, IUIElement>();

    public List<UIBaseElement> uiElements = new List<UIBaseElement>();

    public string defaultUI = UIPath.UIIdle;

    public List<string> defaultUIs = new ();
    
    //TODO:这里做一个ui在启动时就从ab包中缓存并且一直存在的列表 改造还需要继续2026/1/5

    public string CurrentUI => _currentUI;
    
    public string PreviousUI => _previousUI;
    
    private string _currentUI = "";
    
    private string _previousUI = "";

    private bool _isChangingCurrentUI = false;
    
    private HashSet<string> _openedUIs;

    private CancellationTokenSource _currentCancellationToken;
    
    public void Init()
    {
        _openedUIs = new HashSet<string>();
        string uiPath = UIPath.UIIdle;
        foreach (var ui in uiElements)
        {
            var attributes = ui.GetType().GetCustomAttributes(typeof(UI), false);
            if (attributes.Length > 0)
            {
                foreach (var attribute in attributes)
                {
                    var uiAttribute = (UI)attribute;
                    if (uiAttribute != null)
                    {
                        uiPath = uiAttribute.Name;
                    }
                }
            }
            //Debug.Log("name:"+ name);
            uis.Add(uiPath,ui);
            ((IUIElement)ui).Init();
        }
        this.ResetUI();
    }
    
    public void ResetUI()
    {
        _isChangingCurrentUI = false;
        CancelChangeUIToken();
        foreach (var ui in uis)
        {
           var contains = _openedUIs.Contains(ui.Key);
            if (contains)
            {
                CloseUI(ui.Key).Forget();
            }
            else
            {
                ui.Value.SetActive(false); 
            }
        }
        _previousUI = "";
        OpenUI(defaultUI,true);
        foreach (string ui in defaultUIs)
        {
            OpenUI(ui, false);
        }
        SignalSevices.OnResetUI?.Dispatch();
    }

    private void CancelChangeUIToken()
    {
        if (_currentCancellationToken != null)
        {
            _currentCancellationToken.Cancel();
        }
        _currentCancellationToken = new CancellationTokenSource();
    }

    public async UniTask ChangeUI(string uiName)
    {
        if (!uis.ContainsKey(uiName))
        {
            Debug.LogError(uiName + "UI不存在");
            return;
        }
        if (_isChangingCurrentUI)
        {
            CancelChangeUIToken();
            uis[_previousUI].OnCloseOverDisappearTimeUI();
            uis[_previousUI].SetActive(false);
            uis[_currentUI].SetActive(true);
        }
        _previousUI = _currentUI;
        _currentUI = uiName;
        _openedUIs.Add(_currentUI);
        if (_previousUI != "" && uis.ContainsKey(_previousUI))
        {
            _openedUIs.Remove(_previousUI);
            uis[_previousUI].OnCloseUI();
            _isChangingCurrentUI = true;
            await UniTask.Delay(TimeSpan.FromSeconds(uis[_currentUI].DisappearTime), ignoreTimeScale: false, cancellationToken: _currentCancellationToken.Token);
            _isChangingCurrentUI = false;
            uis[_previousUI].OnCloseOverDisappearTimeUI();
            uis[_previousUI].SetActive(false);
        }
        uis[_currentUI].SetActive(true);
        uis[_currentUI].OnOpenUI();
    }
    
    public async UniTask OpenUI(string uiName, float delay = 0)
    {
        if (delay <= 0)
        {
            OpenUI(uiName, false);
        }
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false);
            OpenUI(uiName, false);
        }
    }

    private void OpenUI(string uiName, bool isCurrentMainUI)
    {
        if (!CheckOutUI(uiName))
        {
            return;
        }
        if (isCurrentMainUI)
        {
            _currentUI = uiName;
        }
        _openedUIs.Add(uiName);
        uis[uiName].SetActive(true);
        uis[uiName].OnOpenUI();
    }

    public async UniTask CloseUI(string uiName)
    {
        if (!CheckOutUI(uiName))
        {
            return;
        }
        uis[uiName].OnCloseUI();
        _openedUIs.Remove(uiName);
        await UniTask.Delay(TimeSpan.FromSeconds(uis[uiName].DisappearTime), ignoreTimeScale: false);
        uis[uiName].SetActive(false);
    }
    
    public bool CheckOutUI(string uiName)
    {
        if (!uis.ContainsKey(uiName))
        {
            Debug.LogError(uiName + "UI不存在");
            return false;
        }
        return true;
    }

    public T GetUI<T>(string uiName) where T : UIBaseElement
    {
        if (uis.TryGetValue(uiName, out var ui))
        {
            return ui as T;
        }
        return null;
    }

    private void Update()
    {
#if UNITY_EDITOR && L2
        if (Input.GetKeyDown(KeyCode.C))
        {
            LocalizationManager.CurrentLanguage = AvailableLanguage.Chinese;
        }else if (Input.GetKeyDown(KeyCode.E))
        {
            LocalizationManager.CurrentLanguage = AvailableLanguage.English;
        }
#endif
    }

}

public struct UIPath
{
    public const string UIIdle = "UIIdle";
    public const string UIExample1 = "UIExample1";
    public const string UIExample2 = "UIExample2";
    public const string UIExample3 = "UIExample3";
    public const string UIExample4 = "UIExample4";
    public const string UISelector = "UISelector";
}

#if L2
public struct AvailableLanguage
{
    public const string Chinese = "Chinese";
    public const string English = "English";
}
#endif
