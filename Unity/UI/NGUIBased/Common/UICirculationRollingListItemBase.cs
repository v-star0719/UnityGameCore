#if NGUI

using UnityEngine;
using System.Collections;

namespace GameCore.Unity.NGUIEx
{
    public class UICirculationRollingListItemBase : MonoBehaviour
    {
        public virtual void SetData(int dataIndex, object dataList)
        {
            Debug.LogFormat("UICirculationRollingListItemBase.SetData: dataIndex = {0}", dataIndex);
        }
    }
}

#endif