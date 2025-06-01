using System.Collections.Generic;
using UnityEngine;

namespace ShotShooter.Assets.Scripts
{
    public class Pooler : Singleton<Pooler>
    {
        private Dictionary<object, List<GameObject>> _pool { get; } = new();

        public T GetOrCreateObject<T>(GameObject target)
        {
            if (_pool.TryGetValue(target, out var objects))
            {
                return objects[0].GetComponent<T>();
            }
            else
            {
                var newObject = Instantiate(target);
                _pool[target] = new() { newObject };

                return newObject.GetComponent<T>();
            }
        }
    }
}
