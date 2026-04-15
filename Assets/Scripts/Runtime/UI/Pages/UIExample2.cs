using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[UI(UIPath.UIExample2)]
public class UIExample2 : UIBaseElement
{
    public GameObject whiteSquare;
    
    protected override async UniTask OnOpenUI()
    {
        if (backButton)
            this.backButton.onClick.AddListener(OnBackButtonClicked);
        await this.PlayOpenedAnimation();
        whiteSquare.SetActive(true);
    }
    
    protected override void OnCloseOverDisappearTimeUI()
    {
        base.OnCloseOverDisappearTimeUI();
        whiteSquare.SetActive(false);
    }
}
