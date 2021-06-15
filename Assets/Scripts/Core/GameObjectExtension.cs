using UnityEngine;

namespace Core
{
    public static class GameObjectExtension
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            if (null == component)
            {
                return gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}