using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.UI
{
    public class CloseButton : MonoBehaviour
    {
        public void OnClick()
        {
            GetComponentInParent<UIPanelBaseCore>()?.Close();
        }
    }
}
