﻿using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Kernel.Unity
{
    public class EditDataFloatField : EditDataFieldBase
    {
        private float value;

        public float Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    changed = true;
                    this.value = value;
                }
            }
        }

        public EditDataFloatField(string key, EditDataFieldBundle editData) : base(key, editData)
        {
        }

        public override void Load()
        {
            value = EditorPrefs.GetFloat(key, 0);
        }

        public override void Save()
        {
            changed = false;
            EditorPrefs.SetFloat(key, value);
        }
    }

    public class EditDataIntField : EditDataFieldBase
    {
        private int value;

        public int Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    changed = true;
                    this.value = value;
                }
            }
        }

        public EditDataIntField(string key, EditDataFieldBundle editData) : base(key, editData)
        {
        }

        public override void Load()
        {
            value = EditorPrefs.GetInt(key, 0);
        }

        public override void Save()
        {
            changed = false;
            EditorPrefs.SetInt(key, value);
        }
    }

    public class EditDataStringField : EditDataFieldBase
    {
        private string value;

        public string Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    changed = true;
                    this.value = value;
                }
            }
        }

        public EditDataStringField(string key, EditDataFieldBundle editData) : base(key, editData)
        {
        }

        public override void Load()
        {
            value = EditorPrefs.GetString(key, null);
        }

        public override void Save()
        {
            changed = false;
            EditorPrefs.SetString(key, value);
        }
    }

    public class EditDataBoolField : EditDataFieldBase
    {
        private bool value;

        public bool Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    changed = true;
                    this.value = value;
                }
            }
        }

        public EditDataBoolField(string key, EditDataFieldBundle editData) : base(key, editData)
        {
        }

        public override void Load()
        {
            value = EditorPrefs.GetBool(key, false);
        }

        public override void Save()
        {
            changed = false;
            EditorPrefs.SetBool(key, value);
        }
    }
}