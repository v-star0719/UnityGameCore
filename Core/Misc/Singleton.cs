namespace GameCore.Core
{
    public class Singleton<T> where T : new()
    {
        private static T instance;

        public static T Inst
        {
            get
            {
                instance ??= new T();
                return instance;
            }
        }
    }
}
