using UnityEngine;
using UnityEngine.UI;

public class ImageGray : MonoBehaviour
{
    private Image[] _imgs;
    private void Awake()
    {
        _imgs = GetComponentsInChildren<Image>(true);
    }

    private void OnEnable()
    {
        //foreach (var img in _imgs)
        //{
        //    GlobalFunction.SetImageGray(img, true);
        //}
    }

    private void OnDisable()
    {
        //foreach (var img in _imgs)
        //{
        //    GlobalFunction.SetImageGray(img, false);
        //}
    }
}
