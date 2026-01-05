using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[UI(UIPath.UIIdle)]
public class UIIdle : UIElement
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    protected override void OnOpenUI()
    {
        base.OnOpenUI();
        button1.onClick.AddListener(OnButton1Click);
        button2.onClick.AddListener(OnButton2Click);
        button3.onClick.AddListener(OnButton3Click);
        button4.onClick.AddListener(OnButton4Click);
    }

    protected override void OnCloseUI()
    {
        base.OnCloseUI();
        button1.onClick.RemoveListener(OnButton1Click);
        button2.onClick.RemoveListener(OnButton2Click);
        button3.onClick.RemoveListener(OnButton3Click);
        button4.onClick.RemoveListener(OnButton4Click);
    }

    private void OnButton1Click()
    {
        UIController.Instance.ChangeUI(UIPath.UIExample1);
    }
    
    private void OnButton2Click()
    {
        UIController.Instance.ChangeUI(UIPath.UIExample2);
    }
    
    private void OnButton3Click()
    {
        UIController.Instance.ChangeUI(UIPath.UIExample3);
    }
    
    private void OnButton4Click()
    {
        UIController.Instance.ChangeUI(UIPath.UIExample4);
    }
}
