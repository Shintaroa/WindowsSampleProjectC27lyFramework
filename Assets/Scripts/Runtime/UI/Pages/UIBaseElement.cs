using System;
using System.Threading;
using Cysharp.Threading.Tasks;
#if MOREMOUNTAINS_FEEDBACKS
    using MoreMountains.Feedbacks;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public interface IUIElement
{
    float DisappearTime { get; }
    void Init(){}
    UniTask OnOpenUI();
    UniTask OnCloseUI();
    void OnCloseOverDisappearTimeUI();
    void SetActive(bool isActive);
}

public class UIBaseElement : MonoBehaviour,IUIElement
{
    public float disappearTime = 0.5f;

#if DOTWEEN
    public UIAnimator uiAnimator;
#endif
    
#if MOREMOUNTAINS_FEEDBACKS
    public AsyncMMFeedbacks openedAnimationMMFeedbacks;
    public MMFeedbacks closedAnimationMMFeedbacks;
#endif
    
    public Button backButton;
    
    public string backPageName = UIPath.UIIdle;

    public float DisappearTime => disappearTime;

    private CancellationTokenSource _cancellationToken = new();
    
    #region
    void IUIElement.Init()
    {
        Init();
    }
    async UniTask IUIElement.OnOpenUI()
    {
        await OnOpenUI();
    }
    async UniTask IUIElement.OnCloseUI()
    {
        await OnCloseUI();
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


    protected virtual void Init()
    {
        _cancellationToken = new();
    }
    
    protected virtual async UniTask OnOpenUI()
    {
        if (backButton)
            this.backButton.onClick.AddListener(OnBackButtonClicked);
        await this.PlayOpenedAnimation();
    }

    protected virtual async UniTask OnCloseUI()
    {
        if (backButton)
            this.backButton.onClick.RemoveListener(OnBackButtonClicked);
        await this.PlayClosedAnimation();
        _cancellationToken.Cancel();
        _cancellationToken = new();
    }

    protected virtual void OnCloseOverDisappearTimeUI()
    {
        
    }

    protected virtual async UniTask PlayOpenedAnimation()
    {
#if DOTWEEN
        if (uiAnimator && uiAnimator.isActiveAndEnabled)
        {
            float waitTime = uiAnimator.AnimateUIElements();
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime),ignoreTimeScale:false,cancellationToken:_cancellationToken.Token);
        }
#endif
#if MOREMOUNTAINS_FEEDBACKS
        if (openedAnimationMMFeedbacks != null)
        {
            await openedAnimationMMFeedbacks.PlayFeedbacksAsync();
        }
#endif
        await UniTask.Yield();
    }

    protected virtual async UniTask PlayClosedAnimation()
    {
#if DOTWEEN
        if (uiAnimator && uiAnimator.isActiveAndEnabled)
        {
            uiAnimator.AnimateUIElementsOut();
        }
#endif
#if MOREMOUNTAINS_FEEDBACKS
        closedAnimationMMFeedbacks?.PlayFeedbacks();
#endif
        await UniTask.Yield();
    }
    
    protected virtual void OnBackButtonClicked()
    {
        UIController.Instance.ChangeUI(backPageName).Forget();
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