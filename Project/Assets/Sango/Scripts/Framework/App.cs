/*
'*******************************************************************
'Tank Framework
'*******************************************************************
*/
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace Sango
{
    /// <summary>
    /// Unity3D游戏主框架类
    /// </summary>
    public abstract class App 
    {
        protected static App _app_instance;
        public static App Instance => _app_instance;
        /// <summary>
        /// UI相机
        /// </summary>
        public Camera UICamera { get; set; }

        /// <summary>
        /// UI根节点
        /// </summary>
        public RectTransform UIRoot { get; set; }

        /// <summary>
        /// 根画布
        /// </summary>
        public Canvas RootCanvas { get; set; }

        /// <summary>
        /// 画布缩放器
        /// </summary>
        public CanvasScaler CanvasScaler { get; set; }

        /// <summary>
        /// 画布缩放因子
        /// </summary>
        public float CanvasScalerFactor { get; set; }

        /// <summary>
        /// 框架根游戏物体，脚本依赖此根，通常会挂到此游戏物体下
        /// </summary>
        protected MonoBehaviour rootBehaviour;
        /// <summary>
        /// 框架根行为脚本
        /// </summary>
        protected GameObject rootGameObject;
        /// <summary>
        /// 更新对象
        /// </summary>
        protected List<IUpdate> tickers = new List<IUpdate>(1024);

        public virtual void AddTick(IUpdate update)
        {
            tickers.Add(update);
        }

        public virtual void RemoveTick(IUpdate update)
        {
            tickers.Remove(update);
        }

        public virtual void Init(MonoBehaviour start, Platform.PlatformName targetPlatform)
        {
            rootBehaviour = start;
            rootGameObject = start.gameObject;

            Path.Init();
            Platform.targetPlatform = targetPlatform;
            Platform.Init();
        }

        public virtual void Update()
        {
            int count = tickers.Count;
            if (count == 0) return;
            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            for (int i = 0; i < count; ++i)
            {
                IUpdate update = tickers[i];
                if (!update.Update(deltaTime, unscaledDeltaTime))
                {
                    tickers.RemoveAt(i);
                    i--;
                }
            }
        }

        public virtual void Shutdown(){ }
        public virtual void Pause() { }
        public virtual void Resume() { }
        public Coroutine StartCoroutine(string methodName)
        {
            object value = null;
            return StartCoroutine(methodName, value);
        }
        public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
        {
            return rootBehaviour.StartCoroutine(methodName, value);
        }
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return rootBehaviour.StartCoroutine(routine);
        }
        public void StopCoroutine(IEnumerator routine)
        {
            rootBehaviour.StopCoroutine(routine);
        }
        public void StopCoroutine(Coroutine routine)
        {
            rootBehaviour.StopCoroutine(routine);
        }
        public void StopCoroutine(string methodName)
        {
            rootBehaviour.StopCoroutine(methodName);
        }
        public void StopAllCoroutines()
        {
            rootBehaviour.StopAllCoroutines();
        }
    }
}