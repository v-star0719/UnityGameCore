using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
