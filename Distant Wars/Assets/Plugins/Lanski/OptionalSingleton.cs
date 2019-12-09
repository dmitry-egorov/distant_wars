using UnityEngine;

namespace Plugins.Lanski
{
    public class OptionalSingleton<T>: MonoBehaviour
        where T: OptionalSingleton<T>
    {
        public static T Instance
        {
            get
            {
                if (instance_not_found)
                    return null;

                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        instance_not_found = true;
                    }
                }

                return instance;
            }
        }
        
        static T instance;
        static bool instance_not_found;
    }
}