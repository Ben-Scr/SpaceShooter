using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceShooter
{
    public class DynamicObjectPool
    {
        private Dictionary<GameObject, Stack<GameObject>> pools = new();

        public int Count(GameObject prefab)
        {
            try
            {
                return pools[prefab].Count;
            }
            catch { return 0; }
        }

        public void PreWarm(GameObject prefab, int count)
        {
            if (!pools.TryGetValue(prefab, out var stack))
            {
                stack = new Stack<GameObject>(count);
                pools[prefab] = stack;
            }

            for (int i = 0; i < count; i++)
            {
                var go = Object.Instantiate(prefab);
                go.SetActive(false);
                stack.Push(go);
            }
        }


        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            if (pools.TryGetValue(prefab, out var stack) && stack.Count > 0)
            {
                var obj = stack.Pop();
                obj.transform.SetPositionAndRotation(pos, rot);
                obj.SetActive(true);
                return obj;
            }
            return Object.Instantiate(prefab, pos, rot);
        }
        public GameObject Get(GameObject prefab, Transform parent, bool worldPositionStays = true)
        {
            if (pools.TryGetValue(prefab, out var stack) && stack.Count > 0)
            {
                var obj = stack.Pop();
                obj.transform.SetParent(parent, worldPositionStays);
                obj.SetActive(true);
                return obj;
            }

            return Object.Instantiate(prefab, parent);
        }
        public GameObject Get(GameObject prefab, RectTransform rect, Transform parent)
        {
            if (pools.TryGetValue(prefab, out var stack) && stack.Count > 0)
            {
                var obj = stack.Pop();
                obj.transform.SetParent(parent);
                obj.SetActive(true);
                return obj;
            }

            var newObj = Object.Instantiate(prefab, rect);
            newObj.transform.SetParent(parent, true);
            return newObj;
        }

        public void Release(GameObject prefab, GameObject obj)
        {
            obj.SetActive(false);
            if (!pools.TryGetValue(prefab, out var stack))
            {
                stack = new Stack<GameObject>();
                pools[prefab] = stack;
            }
            stack.Push(obj);
        }

        public IEnumerator Release(GameObject prefab, GameObject obj, float t)
        {
            yield return new WaitForSeconds(t);
            Release(prefab, obj);
        }
    }
}