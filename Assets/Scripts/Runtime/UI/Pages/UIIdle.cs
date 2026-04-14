using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[UI(UIPath.UIIdle)]
public class UIIdle : UIBaseElement
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    protected override async UniTask OnOpenUI()
    {
        await base.OnOpenUI();
        button1.onClick.AddListener(OnButton1Click);
        button2.onClick.AddListener(OnButton2Click);
        button3.onClick.AddListener(OnButton3Click);
        button4.onClick.AddListener(OnButton4Click);
    }

    protected override async UniTask OnCloseUI()
    {
        await base.OnCloseUI();
        button1.onClick.RemoveListener(OnButton1Click);
        button2.onClick.RemoveListener(OnButton2Click);
        button3.onClick.RemoveListener(OnButton3Click);
        button4.onClick.RemoveListener(OnButton4Click);
    }

    private void OnButton1Click()
    {
        UIController.Instance.ChangeUI(UIPath.UIExample1).Forget();
    }
    
    private void OnButton2Click()
    {
        UIController.Instance.ChangeUI(UIPath.UIExample2).Forget();
    }
    
    private void OnButton3Click()
    {
        UIController.Instance.ChangeUI(UIPath.UIExample3).Forget();
    }
    
    private void OnButton4Click()
    {
        UIController.Instance.ChangeUI(UIPath.UIExample4).Forget();
    }
}
