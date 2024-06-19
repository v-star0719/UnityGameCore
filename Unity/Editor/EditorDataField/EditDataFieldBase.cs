using System.Collections;
using UnityEngine;

namespace Kernel.Unity
{
    public abstract class EditDataFieldBase
    {
        public string key;
        public bool changed;

        public EditDataFieldBase(string key, EditDataFieldBundle editData)
        {
            this.key = key;
            editData.fields.Add(this);
        }

        public virtual void Load()
        {
        }

        public virtual void Save()
        {
            changed = false;
        }
    }
}