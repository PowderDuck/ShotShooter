using System;
using UnityEngine;

namespace ShotShooter.Assets.Scripts
{
    public class Singleton<T> : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            Instance = Instance == null
                ? GetComponent<T>()
                : throw new ArgumentOutOfRangeException(
                    "Only one Instance of {typeof(T).Name is Allowed}");
        }
    }
}
