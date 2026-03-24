using System.Collections.Generic;
using UnityEngine;

namespace InfinityGame.DesignPattern.ObjectPooling
{
    public class PoolManager : MonoBehaviour
    {
        private Dictionary<int, Queue<GameObject>> _pools = new Dictionary<int, Queue<GameObject>>();
        private Dictionary<int, Transform> _parents = new Dictionary<int, Transform>();
        private Dictionary<int, Vector3> _prefabScales = new Dictionary<int, Vector3>(); // cache scale gốc

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null)
            {
                Debug.LogError("[PoolManager] Prefab is null!");
                return null;
            }

            int poolKey = prefab.GetInstanceID();
            GameObject obj;

            if (_pools.ContainsKey(poolKey) && _pools[poolKey].Count > 0)
            {
                obj = _pools[poolKey].Dequeue();
            }
            else
            {
                obj = Instantiate(prefab);

                var member = obj.GetComponent<PoolMember>();
                if (member == null) member = obj.AddComponent<PoolMember>();
                member.PoolKey = poolKey;

                // cache scale gốc lần đầu tạo
                if (!_prefabScales.ContainsKey(poolKey))
                    _prefabScales[poolKey] = prefab.transform.localScale;
            }

            if (parent != null)
            {
                obj.transform.SetParent(parent);
            }
            else
            {
                if (!_parents.ContainsKey(poolKey))
                {
                    GameObject container = new GameObject($"Pool_{prefab.name}");
                    container.transform.SetParent(this.transform);
                    _parents[poolKey] = container.transform;
                }

                obj.transform.SetParent(_parents[poolKey]);
            }

            // restore scale gốc sau khi set parent
            if (_prefabScales.ContainsKey(poolKey))
                obj.transform.localScale = _prefabScales[poolKey];

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }

        public GameObject Spawn(GameObject prefab, Transform parent = null)
        {
            return Spawn(prefab, Vector3.zero, prefab.transform.rotation, parent);
        }

        public void Despawn(GameObject obj)
        {
            if (obj == null) return;

            var member = obj.GetComponent<PoolMember>();
            if (member != null)
            {
                int key = member.PoolKey;

                if (!_pools.ContainsKey(key))
                    _pools[key] = new Queue<GameObject>();

                _pools[key].Enqueue(obj);

                if (_parents.ContainsKey(key))
                    obj.transform.SetParent(_parents[key]);
                else
                    obj.transform.SetParent(this.transform);
            }
            else
            {
                Debug.LogWarning(
                    $"[PoolManager] Object '{obj.name}' was not spawned via PoolManager. Destroying instead.");
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
        }

        public void DespawnAll(Transform parent)
        {
            if (parent.childCount <= 0) return;

            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in parent)
                children.Add(child.gameObject);

            foreach (var child in children)
                Despawn(child);
        }
    }

    /// <summary>
    /// Hidden component to track which pool an object belongs to.
    /// </summary>
    public class PoolMember : MonoBehaviour
    {
        public int PoolKey;
    }
}