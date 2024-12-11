using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UINavigation : MonoBehaviour
{
    private Selectable[] selectables;
    private void Start()
    {
        selectables = GetComponentsInChildren<Selectable>(true);

    }

    private void Update()
    {
        
    }
}