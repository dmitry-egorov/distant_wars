using UnityEngine;
using UnityEngine.Assertions;

namespace Plugins.Lanski.Behaviours
{
    public static class ComponentExtensions
    {
        public static T RequireComponent<T>(this Component p) where T : class
        {
            var c = p.GetComponent<T>();
            Assert.IsNotNull(c);
            return c;
        }
        public static bool HasComponent<T>(this Component c)
            where T: Component
        {
            return c.GetComponent<T>() != null;
        }
        
        public static bool TryGetComponent<T>(this Component c, out T component)
            where T: Component
        {
            component = c.GetComponent<T>();
            return component != null;
        }
    }
}