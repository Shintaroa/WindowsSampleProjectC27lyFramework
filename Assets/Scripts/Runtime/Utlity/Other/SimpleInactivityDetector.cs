using UnityEngine;
using System.Collections;
using TFramework;

public class SimpleInactivityDetector : MonoBehaviour
{
    
    private float timer;
    private bool isIdle = false;

    private WaitForSeconds _waiter;
    private Coroutine _checkCoroutine;
    
    void Start()
    {
        _waiter = new WaitForSeconds(1f);
    }
    
    private void OnEnable()
    {
        _checkCoroutine = StartCoroutine(CheckInactivity());
    }

    private void OnDisable()
    {
        if (_checkCoroutine != null)
        {
            StopCoroutine(_checkCoroutine);
            _checkCoroutine = null;
        }
    }

    void Update()
    {
        if (Input.anyKey || Input.GetMouseButton(0) || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            ResetTimer();
            if (isIdle) ReturnToNormal();
        }
    }
    
    private IEnumerator CheckInactivity()
    {
        while (true)
        {
            yield return _waiter;
            timer += 1f;
            
            /*if (timer >= SystemController.Instance.backTime && !isIdle)
            {
                ResetUI();
            }*/
        }
    }
    
    private void ResetTimer()
    {
        timer = 0f;
    }
    
    private void ResetUI()
    {
        isIdle = true;
        UIController.Instance.ResetUI();
    }
    
    private void ReturnToNormal()
    {
        isIdle = false;
    }
}