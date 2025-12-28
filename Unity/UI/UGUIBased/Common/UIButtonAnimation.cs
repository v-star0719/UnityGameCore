using System;
using System.Collections;
using GameCore.Unity.Tweener;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public Vector3 press = new Vector3(1.05f, 1.05f, 1.05f);
    public float duration = 0.1f;
    public GameObject selectGo;//如果用了tween，正向是显示

    private float timer;
    private Vector3 startScale;
    private Vector3 endScale;
    private Vector3 initScale;
    private bool isWorking;//enable=false的话收不到消息
    private TweenPlayer selectGoTweener;

    public void Awake()
    {
        if (selectGo != null)
        {
            selectGoTweener = selectGo.GetComponent<TweenPlayer>();
            if(selectGoTweener != null)
            {
                selectGoTweener.Reset(true);
            }
            else
            {
                selectGo.SetActive(false);
            }
        }
    }

    public void Start()
    {
        initScale = transform.localScale;
    }

    public void Update()
    {
        if(!isWorking)
        {
            return;
        }

        timer += Time.deltaTime;
        if(timer > duration)
        {
            isWorking = false;
            transform.localScale = endScale;
        }
        else
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, timer / duration);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Play(Vector3.Scale(initScale, press));
        StopAllCoroutines();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Play(initScale);
        StopAllCoroutines();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(selectGo != null)
        {
            if(selectGoTweener != null)
            {
                selectGoTweener.Play(true);
            }
            else
            {
                selectGo.SetActive(true);
            }
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if(selectGo != null)
        {
            if(selectGoTweener != null)
            {
                selectGoTweener.Play(false);
            }
            else
            {
                selectGo.SetActive(false);
            }
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Play(Vector3.Scale(initScale, press));

        IEnumerator Recover()
        {
            yield return new WaitForSeconds(duration + 0.1f);
            Play(initScale);
        }

        if (gameObject.activeSelf)
        {
            StartCoroutine(Recover());
        }
    }

    private void Play(Vector3 targetScale)
    {
        timer = 0;
        startScale = transform.localScale;
        endScale = targetScale;
        isWorking = true;
    }
}