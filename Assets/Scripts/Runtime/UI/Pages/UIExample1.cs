using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[UI(UIPath.UIExample1)]
public class UIExample1 : UIBaseElement
{
    public GameObject whiteSquare;
    
    protected override async UniTask OnOpenUI()
    {
        if (backButton)
            this.backButton.onClick.AddListener(OnBackButtonClicked);
        await this.PlayOpenedAnimation();
        Debug.Log(" whiteSquare.SetActive(true);");
        whiteSquare.SetActive(true);
    }

    protected override async UniTask OnCloseUI()
    {
        await base.OnCloseUI();
        whiteSquare.SetActive(false);
    }
}
