#if DOTWEEN
using System;
using UnityEngine;
using DG.Tweening;

//TODO: dotween有unitask的拓展以后可以优化 2026/1/5
public class UIAnimator : MonoBehaviour
{
    public GameObject[] uiElements; // 在Inspector中设置要依次弹出的UI元素
    public AnimationType[] animationTypes; // 为每个元素设置动画类型
    public float duration = 0.5f; // 动画持续时间
    public float delayBetweenElements = 0.2f; // 元素之间的延迟
    public float flyDistance = 500f; // 从底部飞入的距离
    public bool hasScaleChange = false;
    public bool initOnAwake = true;
    private bool _isInit = false;
    
    // 保存原始位置和缩放状态
    private Vector3[] _originalPositions;
    private Vector3[] _originalScales;

    private Coroutine _disableCoroutine;

    public enum AnimationType // 定义动画类型
    {
        Pop,
        Fade,
        Bounce,
        FlyFromBottom,
        FlyFromTop,
        FlyFromLeft,
        FlyFromRight
    }

    private void Awake()
    {
        if (initOnAwake)
        {
            InitializeUIElements();
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AnimateUIElements();//入场
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            AnimateUIElementsOut();//退场
        }
    }
#endif    
    
    private void InitializeUIElements()
    {
        // 检查数组长度是否匹配
        if (uiElements.Length == 0 || animationTypes.Length != uiElements.Length)
        {
            Debug.LogError("UI元素和动画类型数组长度不匹配！");
            return;
        }

        // 初始化原始位置和缩放数组
        _originalPositions = new Vector3[uiElements.Length];
        _originalScales = new Vector3[uiElements.Length];

        for (int i = 0; i < uiElements.Length; i++)
        {
            var element = uiElements[i];
            _originalPositions[i] = element.transform.localPosition; // 保存原始位置
            _originalScales[i] = element.transform.localScale; // 保存原始缩放

            // 检查并添加 CanvasGroup 组件
            CanvasGroup canvasGroup = element.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = element.AddComponent<CanvasGroup>();
            }
            // 初始化为透明并缩放为0，并且隐藏所有元素
            if (hasScaleChange || animationTypes[i] == AnimationType.Bounce || animationTypes[i] == AnimationType.Pop)
            {
                element.transform.localScale = Vector3.zero;
            }
            canvasGroup.alpha = 0;
            element.SetActive(false); // 隐藏UI元素
        }
        _isInit = true;
    }

    public void AnimateUIElements()
    {
        if (!_isInit)
        {
            this.InitializeUIElements();
        }
        for (int i = 0; i < uiElements.Length; i++)
        {
            var element = uiElements[i];
            // 如果该元素已经激活并有动画在进行，跳出循环
            //if (element.activeSelf) continue;

            // 停止该元素的所有动画
            element.transform.DOKill();
            CanvasGroup canvasGroup = element.GetComponent<CanvasGroup>();

            // 重置状态为透明并缩放为初始状态
            if (hasScaleChange || animationTypes[i] == AnimationType.Bounce || animationTypes[i] == AnimationType.Pop)
            {
                element.transform.localScale = Vector3.zero;
            }
            canvasGroup.alpha = 0;

            // 激活UI元素
            element.SetActive(true);

            // 根据动画类型执行不同的动画
            AnimationType currentAnimation = animationTypes[i];

            switch (currentAnimation)
            {
                case AnimationType.Pop: // 弹出
                    element.transform.DOScale(_originalScales[i], duration).SetDelay(i * delayBetweenElements);
                    canvasGroup.DOFade(1, duration).SetDelay(i * delayBetweenElements);
                    break;

                case AnimationType.Fade: // 淡入
                    if (hasScaleChange)
                        element.transform.DOScale(_originalScales[i], duration).SetDelay(i * delayBetweenElements);
                    canvasGroup.DOFade(1, duration).SetDelay(i * delayBetweenElements);
                    break;

                case AnimationType.Bounce: // 弹跳
                    element.transform.DOScale(_originalScales[i], duration).SetEase(Ease.OutBounce).SetDelay(i * delayBetweenElements);
                    canvasGroup.DOFade(1, duration).SetDelay(i * delayBetweenElements);
                    break;

                case AnimationType.FlyFromBottom: // 从底部飞到原位置
                    element.transform.localPosition = _originalPositions[i] + new Vector3(0, -flyDistance, 0); // 从初始化位置偏移
                    element.transform.DOLocalMove(_originalPositions[i], duration).SetDelay(i * delayBetweenElements);
                    canvasGroup.DOFade(1, duration).SetDelay(i * delayBetweenElements);
                    if (hasScaleChange)
                        element.transform.DOScale(_originalScales[i], duration).SetDelay(i * delayBetweenElements);
                    break;
                case AnimationType.FlyFromTop:
                    element.transform.localPosition = _originalPositions[i] + new Vector3(0, flyDistance, 0); // 从初始化位置偏移
                    element.transform.DOLocalMove(_originalPositions[i], duration).SetDelay(i * delayBetweenElements);
                    canvasGroup.DOFade(1, duration).SetDelay(i * delayBetweenElements);
                    if (hasScaleChange)
                        element.transform.DOScale(_originalScales[i], duration).SetDelay(i * delayBetweenElements);
                    break;
                case AnimationType.FlyFromLeft:
                    element.transform.localPosition = _originalPositions[i] + new Vector3(-flyDistance, 0, 0); // 从初始化位置偏移
                    element.transform.DOLocalMove(_originalPositions[i], duration).SetDelay(i * delayBetweenElements);
                    canvasGroup.DOFade(1, duration).SetDelay(i * delayBetweenElements);
                    if (hasScaleChange)
                        element.transform.DOScale(_originalScales[i], duration).SetDelay(i * delayBetweenElements);
                    break;
                case AnimationType.FlyFromRight:
                    element.transform.localPosition = _originalPositions[i] + new Vector3(flyDistance, 0, 0); // 从初始化位置偏移
                    element.transform.DOLocalMove(_originalPositions[i], duration).SetDelay(i * delayBetweenElements);
                    canvasGroup.DOFade(1, duration).SetDelay(i * delayBetweenElements);
                    if (hasScaleChange)
                        element.transform.DOScale(_originalScales[i], duration).SetDelay(i * delayBetweenElements);
                    break;
            }
        }
    }

    public void AnimateUIElementsOut()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            var element = uiElements[i];
            // 如果该元素没有激活，则跳过
            if (!element.activeSelf) continue;

            // 停止该元素的所有动画
            element.transform.DOKill();
            CanvasGroup canvasGroup = element.GetComponent<CanvasGroup>();

            // 根据动画类型执行不同的退场动画
            AnimationType currentAnimation = animationTypes[i];

            switch (currentAnimation)
            {
                case AnimationType.Pop: // 弹出
                    if (hasScaleChange)
                        element.transform.DOScale(Vector3.zero, duration);
                    canvasGroup.DOFade(0, duration);
                    break;
                case AnimationType.Fade: // 淡出
                    if (hasScaleChange)
                        element.transform.DOScale(_originalScales[i], duration).SetDelay(i * delayBetweenElements);
                    canvasGroup.DOFade(0, duration);
                    break;

                case AnimationType.Bounce: // 弹跳
                    element.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBounce);
                    canvasGroup.DOFade(0, duration);
                    break;

                case AnimationType.FlyFromBottom: // 从底部飞出去
                    //element.transform.DOLocalMove(_originalPositions[i] + new Vector3(0, -flyDistance, 0), duration);
                    canvasGroup.DOFade(0, duration);
                    if (hasScaleChange)
                        element.transform.DOScale(Vector3.zero, duration);
                    break;
                case AnimationType.FlyFromTop: // 从底部飞出去
                    //element.transform.DOLocalMove(_originalPositions[i] + new Vector3(0, flyDistance, 0), duration);
                    canvasGroup.DOFade(0, duration);
                    if (hasScaleChange)
                        element.transform.DOScale(Vector3.zero, duration);
                    break;
                case AnimationType.FlyFromLeft: // 从底部飞出去
                    //element.transform.DOLocalMove(_originalPositions[i] + new Vector3(0, -flyDistance, 0), duration);
                    canvasGroup.DOFade(0, duration);
                    if (hasScaleChange)
                        element.transform.DOScale(Vector3.zero, duration);
                    break;
                case AnimationType.FlyFromRight: // 从底部飞出去
                    //element.transform.DOLocalMove(_originalPositions[i] + new Vector3(0, flyDistance, 0), duration);
                    canvasGroup.DOFade(0, duration);
                    if (hasScaleChange)
                        element.transform.DOScale(Vector3.zero, duration);
                    break;
            }

            if (gameObject.activeInHierarchy)
            {
                // 动画完成后禁用元素
                _disableCoroutine = StartCoroutine(DisableElementAfterAnimation(element, duration));
            }
        }
    }

    private System.Collections.IEnumerator DisableElementAfterAnimation(GameObject element, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        element.SetActive(false);
    }

    private void OnDisable()
    {
        if (_disableCoroutine != null)
        {
            StopCoroutine(_disableCoroutine); 
        }
    }
}
#endif