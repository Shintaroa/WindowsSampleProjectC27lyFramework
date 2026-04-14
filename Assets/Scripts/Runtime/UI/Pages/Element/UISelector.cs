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
            if (uiName == UIPath.UIExample3)
            {
                UIController.Instance.ChangeUI(uiName,5,1).Forget();
            }
            else
            {
                UIController.Instance.ChangeUI(uiName).Forget();
            }
        }
    }

    public List<Item> items = new List<Item>();

    protected override async UniTask OnOpenUI()
    {
        await base.OnOpenUI();
        foreach (var item in items)
        {
            item.button.onClick.AddListener(item.OnClick);
        }
    }
    
    protected override async UniTask OnCloseUI()
    {
        await base.OnCloseUI();
        foreach (var item in items)
        {
            item.button.onClick.RemoveListener(item.OnClick);
        }
    }
}
