using System.Collections.Generic;

namespace GameCore.Unity
{
    public class EditDataFieldBundle
    {
        protected internal List<EditDataFieldBase> fields = new List<EditDataFieldBase>();

        public EditDataFieldBundle()
        {
        }

        public virtual void Load()
        {
            foreach (var field in fields)
            {
                field.Load();
            }
        }

        public virtual void Save()
        {
            foreach (var field in fields)
            {
                field.Save();
            }
        }

        public bool IsChanged()
        {
            foreach (var field in fields)
            {
                if (field.changed)
                {
                    return true;
                }
            }
            return false;
        }
    }
}