using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sango
{

    public class UGUIWindow : MonoBehaviour
    {
        public Action OnCloseAction;
        protected UnityEngine.Canvas[] panels;
        public bool IsOpen { get; set; }
        protected virtual void Awake()
        {
        }

        /// <summary>
        /// 获取对象, 传入对象路径
        /// </summary>
        /// <param name="namePath"> e.g: (root/)UI/nameLabel</param>
        /// <returns>找到的Transform</returns>
        public Transform GetTransform(string namePath)
        {
            Transform rs = transform.Find(namePath);
            if (rs == null && Config.isDebug)
                Log.Warning("在 " + gameObject.name + " 中无法找到节点:" + namePath);
            return rs;
        }
        /// <summary>
        /// 获取对象, 传入对象路径
        /// </summary>
        /// <param name="namePath"> e.g: (root/)UI/nameLabel</param>
        /// <returns>找到的GameObject</returns>
        public GameObject GetObject(string namePath)
        {
            Transform trans = GetTransform(namePath);
            if (trans != null)
                return trans.gameObject;
            return null;
        }
        /// <summary>
        /// 获取对象上的T接口
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="namePath">e.g: (root/)UI/nameLabel</param>
        /// <returns>T接口</returns>
        public T GetComponent<T>(string namePath) where T : Component
        {
            Transform trans = GetTransform(namePath);
            if (trans != null)
                return trans.GetComponent<T>();
            return null;
        }
        /// <summary>
        /// 获取对象上的接口,传入接口type
        /// </summary>
        /// <param name="namePath">e.g: (root/)UI/nameLabel</param>
        /// <param name="typeName">类型字符串名字</param>
        /// <returns>接口</returns>

        public Component GetComponent(string namePath, string typeName)
        {
            Transform trans = GetTransform(namePath);
            if (trans != null)
                return trans.GetComponent(typeName);
            return null;
        }
        public Component GetComponent(string namePath, Type t)
        {
            Transform trans = GetTransform(namePath);
            if (trans != null)
                return trans.GetComponent(t);
            return null;
        }

        protected virtual void OnDestroy()
        {

        }

        /// <summary>
        /// 设置canvas的layer和order
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="order"></param>
        public void SetLayerAndOrder(int layer, int order)
        {
            if (panels == null)
                panels = GetComponentsInChildren<UnityEngine.Canvas>(true);

            if (panels == null || panels.Length == 0)
                return;

            int offset = order - panels[0].sortingOrder;
            for (int i = 0, count = panels.Length; i < count; ++i)
            {
                Canvas p = panels[i];
                if (p != null)
                {
                    p.overrideSorting = true;
                    p.sortingLayerID = layer;
                    p.sortingOrder += offset;
                }
            }
        }
        public void SetLayerNameAndOrder(string layer, int order)
        {
            if (panels == null)
                panels = GetComponentsInChildren<UnityEngine.Canvas>(true);

            if (panels == null || panels.Length == 0)
                return;

            int offset = order - panels[0].sortingOrder;
            for (int i = 0, count = panels.Length; i < count; ++i)
            {
                Canvas p = panels[i];
                if (p != null)
                {
                    p.overrideSorting = true;
                    p.sortingLayerName = layer;
                    p.sortingOrder += offset;
                }
            }
        }

        public virtual void Open()
        {
            IsOpen = true;
            //if (!this.gameObject.activeInHierarchy)
            {
                this.gameObject.SetActive(true);
            }
            OnOpen();
        }

        public virtual void Open(params object[] objects)
        {
            IsOpen = true;
            if (!this.gameObject.activeInHierarchy)
            {
                this.gameObject.SetActive(true);
            }
            OnOpen(objects);
        }

        public virtual void OnOpen(params object[] objects)
        {

        }

        public virtual void OnOpen()
        {

        }
        public virtual void OnRefresh()
        {

        }


        public virtual void Close()
        {
            if (this.gameObject.activeInHierarchy)
            {
                this.gameObject.SetActive(false);
            }
            OnClose();
            IsOpen = false;
        }

        public virtual void Refresh()
        {
            OnRefresh();
        }

        public virtual void OnClose()
        {
            OnCloseAction?.Invoke();
        }

        public void SetText(string path, string content)
        {
            Text text = GetComponent<Text>(path);
            if (text != null)
            { text.text = content; }
        }

        public void SetTexture(string path, string texturePath)
        {
            RawImage rawImage = GetComponent<RawImage>(path);
            if (rawImage != null)
            {
                rawImage.texture = Loader.ObjectLoader.LoadObject<Texture>(texturePath, true, false);
            }
        }

        public void SetImage(string path, string spritePath)
        {
            Image Image = GetComponent<Image>(path);
            if (Image != null)
            {
                Image.sprite = Loader.ObjectLoader.LoadObject<UnityEngine.Sprite>(spritePath);
            }
        }
    }
}
