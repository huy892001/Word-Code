using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using InfinityGame.DesignPattern.ObjectPooling;

namespace InfinityGame.DesignPattern.Factory
{
    public interface IFactory<TKey, TProduct>
    {
        List<TProduct> Create(TKey key, int amount = 1, Transform parent = null);
    }

    /// <summary>
    /// Base Factory Universal: Dùng chung cho mọi loại đối tượng (Component).
    /// TKey: Kiểu định danh (Enum, string, int...)
    /// TProduct: Kiểu đối tượng trả về (phải là Component)
    /// </summary>
    public abstract class Factory<TKey, TProduct> : SerializedMonoBehaviour, IFactory<TKey, TProduct>
        where TProduct : Component // <<< QUAN TRỌNG: Cần ràng buộc này
    {
        [Header("Factory Base Settings")] [SerializeField]
        protected Dictionary<TKey, TProduct> Pool = new Dictionary<TKey, TProduct>();

        /// <summary>
        /// Create List of Items
        /// </summary>
        public virtual List<TProduct> Create(TKey key, int amount = 1, Transform parent = null)
        {
            List<TProduct> results = new List<TProduct>();

            if (Pool.TryGetValue(key, out TProduct product))
            {
                for (int i = 0; i < amount; i++)
                {
                    // product là Component, cần truy cập .gameObject để lấy prefab
                    results.Add(ObjectPool.Instance.Spawn<TProduct>(product.gameObject, parent));
                }

                return results;
            }

            Debug.LogError($"[Factory] Key '{key}' not found in {gameObject.name}");
            return results;
        }

        /// <summary>
        /// Create Single Item
        /// </summary>
        public virtual TProduct Create(TKey key, Transform parent = null)
        {
            if (Pool.TryGetValue(key, out TProduct product))
            {
                // product là Component, cần truy cập .gameObject để lấy prefab
                return ObjectPool.Instance.Spawn<TProduct>(product.gameObject, parent);
            }

            Debug.LogError($"[Factory] Key '{key}' not found in {gameObject.name}");
            return null;
        }
    }
}