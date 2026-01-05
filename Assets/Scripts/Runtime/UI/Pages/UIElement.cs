using System;
#if MOREMOUNTAINS_FEEDBACKS
    using MoreMountains.Feedbacks;
#endif
using UnityEngine;
using UnityEngine.UI;

public interface IUIElement
{
    string SelfUIName { get; set; }
    float DisappearTime { get; }
    void Init(){}
    void OnOpenUI();
    void OnCloseUI();
    void OnCloseOverDisappearTimeUI();
    void SetActive(bool isActive);
}

public class UIElement : MonoBehaviour,IUIElement
{
    public float disappearTime = 0.5f;

#if DOTWEEN
    public UIAnimator uiAnimator;
#endif
    
#if MOREMOUNTAINS_FEEDBACKS
    public MMFeedbacks openedAnimationMMFeedbacks;
    public MMFeedbacks closedAnimationMMFeedbacks;
#endif
    
    public Button backButton;
    
    public string backPageName = UIPath.UIIdle;

    public float DisappearTime => disappearTime;
    public string SelfUIName { get; set; }

    #region 通过接口封装代码
    void IUIElement.Init()
    {
        Init();
    }
    void IUIElement.OnOpenUI()
    {
        OnOpenUI();
    }
    void IUIElement.OnCloseUI()
    {
        OnCloseUI();
    }
    void IUIElement.OnCloseOverDisappearTimeUI()
    {
        OnCloseOverDisappearTimeUI();
    }
    void IUIElement.SetActive(bool isActive)
    {
        SetActive(isActive);
    }
    #endregion


    protected virtual void Init(){}
    
    protected virtual void OnOpenUI()
    {
        this.PlayOpenedAnimation();
        if (backButton)
            this.backButton.onClick.AddListener(OnBackButtonClicked);
    }

    protected virtual void OnCloseUI()
    {
        this.PlayClosedAnimation();
        if (backButton)
            this.backButton.onClick.RemoveListener(OnBackButtonClicked);
    }

    protected virtual void OnCloseOverDisappearTimeUI()
    {
        
    }

    protected virtual void PlayOpenedAnimation()
    {
#if DOTWEEN
        if (uiAnimator && uiAnimator.isActiveAndEnabled)
        {
            uiAnimator?.AnimateUIElements();
        }
#endif
#if MOREMOUNTAINS_FEEDBACKS
        openedAnimationMMFeedbacks?.PlayFeedbacks();
#endif
    }

    protected virtual void PlayClosedAnimation()
    {
#if DOTWEEN
        if (uiAnimator && uiAnimator.isActiveAndEnabled)
        {
            uiAnimator?.AnimateUIElementsOut();
        }
#endif
#if MOREMOUNTAINS_FEEDBACKS
        closedAnimationMMFeedbacks?.PlayFeedbacks();
#endif
    }
    
    protected virtual void OnBackButtonClicked()
    {
        UIController.Instance.ChangeUI(backPageName);
    }


    protected void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class UI : Attribute
{
    public string Name { get; set; }

    public UI(string name)
    {
        Name = name;
    }
}