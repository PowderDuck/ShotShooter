using System.Collections.Generic;
using System.Linq;
using ShotShooter.Assets.Scripts.Entities;
using UnityEngine;

namespace ShotShooter.Assets.Scripts
{
    public class Pooler : Singleton<Pooler>
    {
        private Dictionary<object, List<Poolable>> _pool { get; } = new();

        public T GetOrCreateObject<T>(GameObject source)
        {
            if (_pool.TryGetValue(source, out var objects))
            {
                var availableObject = objects
                    .FirstOrDefault(obj => obj.IsAvailable);
                if (availableObject == null)
                {
                    availableObject = new(Instantiate(source));
                    objects.Add(availableObject);
                }

                return availableObject
                    .Lend()
                    .GetComponent<T>();
            }

            var newObject = new Poolable(Instantiate(source));
            _pool[source] = new() { newObject };

            return newObject
                .Lend()
                .GetComponent<T>();
        }

        public void ReturnToPool(GameObject target)
        {
            foreach (var pair in _pool)
            {
                var availableObject = pair.Value
                    .FirstOrDefault(obj => obj.Target == target);
                if (availableObject != null)
                {
                    availableObject.Return();
                    break;
                }
            }
        }
    }
}
