using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Sango
{
    public class CreatePool<T> where T : Component
    {
        T target;
        public List<T> instance_list = new List<T>();
        List<T> pool_list = new List<T>();

        public CreatePool(T target)
        {
            this.target = target;
        }

        public T Create()
        {
            return Create(target.transform.parent);
        }

        public T Create(Transform parent)
        {
            if (pool_list.Count > 0)
            {
                T rectTransform = pool_list[0];
                pool_list.RemoveAt(0);
                instance_list.Add(rectTransform);
                rectTransform.gameObject.SetActive(true);
                return rectTransform;
            }
            else
            {
                T rectTransform = GameObject.Instantiate(target.gameObject, parent).GetComponent<T>();
                instance_list.Add(rectTransform);
                rectTransform.gameObject.SetActive(true);
                return rectTransform;
            }
        }


        public void Recycle(T obj)
        {
            if (instance_list.Contains(obj))
            {
                instance_list.Remove(obj);
                pool_list.Add(obj);
                obj.gameObject.SetActive(false);
            }
        }

        public void Recycle(T obj, Transform parent)
        {
            if (instance_list.Contains(obj))
            {
                instance_list.Remove(obj);
                pool_list.Add(obj);
                obj.transform.SetParent(parent, false);
                obj.gameObject.SetActive(false);
            }
        }

        public void Reset()
        {
            pool_list.AddRange(instance_list);
            foreach (T r in pool_list)
                r.gameObject.SetActive(false);
            instance_list.Clear();
        }

        public T Get(int index)
        {
            if(index >= 0 && index < instance_list.Count)
                return instance_list[index];
            return null;
        }


        public void Clear()
        {
            Reset();
            foreach (T r in pool_list)
                GameObject.Destroy(r.gameObject);
            pool_list.Clear();
        }
    }
}
