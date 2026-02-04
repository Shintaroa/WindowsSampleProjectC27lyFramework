using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

[UI(UIPath.UISelector)]
public class UISelector : UIBaseElement
{
    [Serializable]
    public class Item
    {
        public Button button;
        public string uiName;
        internal void OnClick()
        {
            UIController.Instance.ChangeUI(uiName).Forget();
        }
    }

    public List<Item> items = new List<Item>();

    protected override void OnOpenUI()
    {
        base.OnOpenUI();
        foreach (var item in items)
        {
            item.button.onClick.AddListener(item.OnClick);
        }
    }
    
    protected override void OnCloseUI()
    {
        base.OnCloseUI();
        foreach (var item in items)
        {
            item.button.onClick.RemoveListener(item.OnClick);
        }
    }
}
