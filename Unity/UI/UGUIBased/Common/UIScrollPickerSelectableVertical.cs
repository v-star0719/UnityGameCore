using UnityEngine.EventSystems;

namespace GameCore.Unity.UGUIEx
{
    public class UIScrollPickerSelectableVertical : UIScrollPickerSelectableBase
    {
        public override void OnMove(AxisEventData eventData)
        {
            switch (eventData.moveDir)
            {
                case MoveDirection.Down:
                    if(picker.CurSelectedIndex < picker.ItemCount - 1)
                    {
                        picker.Select(picker.CurSelectedIndex + 1);
                        return;
                    }
                    break;
                case MoveDirection.Up:
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

