using System.Collections.Generic;
using UnityEngine;

namespace FCT.Gameplay
{
    /// <summary>
    /// Highly optimized Object Pool system. 
    /// Supports pre-warming, automatic IPoolable resets, and safe despawning.
    /// </summary>
    public class SimplePool : FCT.Utils.FCTSingleton<SimplePool>
    {
        private Dictionary<GameObject, Queue<GameObject>> _pools = new Dictionary<GameObject, Queue<GameObject>>();

        /// <summary>
        /// Pre-instantiates a number of objects to avoid FPS drops during gameplay.
        /// </summary>
        public void Prewarm(GameObject prefab, int count, Transform parent = null)
        {
            if (prefab == null) return;
            
            if (!_pools.ContainsKey(prefab))
                _pools.Add(prefab, new Queue<GameObject>());
                
            for(int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(prefab, parent != null ? parent : transform);
                obj.SetActive(false);
                
                var poolItem = obj.GetComponent<PoolItem>();
                if (poolItem == null) poolItem = obj.AddComponent<PoolItem>();
                poolItem.prefabSource = prefab;
                
                _pools[prefab].Enqueue(obj);
            }
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null) return null;

            if (!_pools.ContainsKey(prefab))
                _pools.Add(prefab, new Queue<GameObject>());

            GameObject obj = null;

            // Safe dequeue (handles if user destroyed pooled objects)
            while (_pools[prefab].Count > 0)
            {
                obj = _pools[prefab].Dequeue();
                if (obj != null) break;
            }

            if (obj == null)
            {
                obj = Instantiate(prefab, position, rotation, parent);
                var poolItem = obj.AddComponent<PoolItem>();
                poolItem.prefabSource = prefab;
            }
            else
            {
                obj.transform.SetParent(parent);
                obj.transform.SetPositionAndRotation(position, rotation);
                
                obj.transform.localScale = prefab.transform.localScale;

                var rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
#if UNITY_2023_3_OR_NEWER
                    rb.linearVelocity = Vector3.zero;
#else
                    rb.velocity = Vector3.zero;
#endif
                    rb.angularVelocity = Vector3.zero;
                }
                obj.layer = prefab.layer;
            }

            obj.SetActive(true);

            // Interface call
            var poolables = obj.GetComponentsInChildren<IPoolable>();
            foreach (var p in poolables) p.OnSpawn();

            return obj;
        }

        public void Despawn(GameObject obj)
        {
            if (obj == null) return;

            var poolables = obj.GetComponentsInChildren<IPoolable>();
            foreach (var p in poolables) p.OnDespawn();

            var poolItem = obj.GetComponent<PoolItem>();
            if (poolItem == null || poolItem.prefabSource == null)
            {
                Destroy(obj); // Not from pool
                return;
            }

            obj.SetActive(false);
            obj.transform.SetParent(transform); // Keeps scene hierarchy clean

            if (!_pools.ContainsKey(poolItem.prefabSource))
                _pools.Add(poolItem.prefabSource, new Queue<GameObject>());

            if (!_pools[poolItem.prefabSource].Contains(obj))
                _pools[poolItem.prefabSource].Enqueue(obj);
        }
        
        public void Clear()
        {
            foreach (var pool in _pools.Values)
            {
                while (pool.Count > 0)
                {
                    var obj = pool.Dequeue();
                    if (obj != null) Destroy(obj);
                }
            }
            _pools.Clear();
        }
    }

    public class PoolItem : MonoBehaviour
    {
        [HideInInspector] public GameObject prefabSource;
    }
}
