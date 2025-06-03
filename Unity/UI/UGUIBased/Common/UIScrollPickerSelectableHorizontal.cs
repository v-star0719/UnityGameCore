using UnityEngine.EventSystems;

namespace GameCore.Unity.UGUIEx
{
    public class UIScrollPickerSelectableHorizontal : UIScrollPickerSelectableBase
    {
        public override void OnMove(AxisEventData eventData)
        {
            switch (eventData.moveDir)
            {
                case MoveDirection.Right:
                    if(picker.CurSelectedIndex < picker.ItemCount - 1)
                    {
                        picker.Select(picker.CurSelectedIndex + 1);
                        return;
                    }
                    break;
                case MoveDirection.Left:
                    if (picker.CurSelectedIndex > 0)
                    {
                        picker.Select(picker.CurSelectedIndex - 1);
                        return;
                    }
                    break;
            }
            base.OnMove(eventData);
        }
    }
}

