using UnityEngine;
using UnityEngine.Assertions;

namespace Plugins.Lanski
{
    public class RequiredSingleton<T>: MonoBehaviour
        where T: RequiredSingleton<T>
    {
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    Assert.IsNotNull(instance, $"Instance of required singleton {nameof(T)} not found in the scene");
                }

                return instance;
            }
        }
        
        static T instance;
    }
}