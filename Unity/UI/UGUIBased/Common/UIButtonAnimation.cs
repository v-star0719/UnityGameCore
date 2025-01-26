using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    public Vector3 press = new Vector3(1.05f, 1.05f, 1.05f);
    public float duration = 0.2f;
    public GameObject selectGo;

    private float timer;
    private Vector3 startScale;
    private Vector3 endScale;
    private Vector3 initScale;
    private bool isWorking;//enable=false的话收不到消息

    public void Start()
    {
        initScale = transform.localScale;
    }

    public void Update()
    {
        if (!isWorking)
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
        timer = 0;
        startScale = transform.localScale;
        endScale = Vector3.Scale(initScale, press);
        isWorking = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        timer = 0;
        startScale = transform.localScale;
        endScale = initScale;
        isWorking = true;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (selectGo != null)
        {
            selectGo.SetActive(true);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if(selectGo != null)
        {
            selectGo.SetActive(false);
        }
    }
}
