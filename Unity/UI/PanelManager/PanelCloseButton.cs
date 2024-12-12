using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameCore.Unity
{
    public class PanelCloseButton : MonoBehaviour
    {
        public void OnClick()
        {
            GetComponentInParent<PanelBaseCore>()?.Close();
        }
    }
}
