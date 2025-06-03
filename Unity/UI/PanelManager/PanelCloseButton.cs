using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity
{
    public class PanelCloseButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                GetComponentInParent<PanelBaseCore>()?.Close();
            });
        }
    }
}
