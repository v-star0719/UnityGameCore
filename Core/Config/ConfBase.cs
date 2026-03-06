using System;

namespace GameCore.Core.Config
{
    [Serializable]
    public class ConfBase
    {
        public int id;
        public string name;

        public override string ToString()
        {
            return id + " " + name;
        }

        public virtual void OnTraversal()
        {

        }
    }
}
