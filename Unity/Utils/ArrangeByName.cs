using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Unity
{
    [ExecuteAlways]
    public class ArrangeByName : MonoBehaviour
    {
        public bool sortByName;
        public Vector3 gap;

        private bool? _sortByName;
        private Vector3 _gap;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(_sortByName != sortByName)
            {
                _sortByName = sortByName;
                Sort();
                return;
            }

            if(_gap != gap)
            {
                _gap = gap;
                Sort();
            }
        }

        private void Sort()
        {
            List<Transform> children = new List<Transform>();
            for(int i = 0; i < transform.childCount; i++)
            {
                children.Add(transform.GetChild(i));
            }

            if(sortByName)
            {
                children.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
            }

            for(int i = 0; i < children.Count; i++)
            {
                children[i].SetSiblingIndex(i);
                children[i].transform.localPosition = _gap * (i>>1);
            }
        }
    }
}