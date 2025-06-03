using GameCore.Core;
using System.Reflection;
using System;

namespace Assets.Kernel.Unity
{
    public class EditUtils
    {
        public static string GetEnumCommentText(Enum em)
        {
            Type type = em.GetType();
            var field = type.GetField(em.ToString(), BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
            if (field == null)
            {
                return em.ToString();
            }

            var ca = field.GetCustomAttribute<CommentAttribute>();
            if (ca != null)
            {
                return ca.Comment;
            }

            return em.ToString();
        }
    }
}