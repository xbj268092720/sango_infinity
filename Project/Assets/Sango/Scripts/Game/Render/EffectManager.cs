using System.Collections.Generic;
using UnityEngine;

namespace Sango.Core
{
    public class EffectManager
    {
        private static EffectManager _instance;
        public static EffectManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EffectManager();
                }
                return _instance;
            }
        }

        private Dictionary<string, GameObject> _effectPrefabs = new Dictionary<string, GameObject>();
        private Dictionary<string, Queue<GameObject>> _effectPools = new Dictionary<string, Queue<GameObject>>();

        public void Initialize()
        {
            // 预加载常用特效
            LoadEffectPrefab("ArrowEffect");
            LoadEffectPrefab("StrategyEffect");
            LoadEffectPrefab("MeleeEffect");
            LoadEffectPrefab("RangeEffect");
        }

        private void LoadEffectPrefab(string effectName)
        {
            // 这里应该从资源文件夹加载特效预制体
            // 暂时使用占位符
            _effectPrefabs[effectName] = new GameObject(effectName);
            _effectPools[effectName] = new Queue<GameObject>();
        }

        public GameObject PlayEffect(string effectName, Vector3 position)
        {
            if (!_effectPrefabs.ContainsKey(effectName))
            {
                LoadEffectPrefab(effectName);
            }

            GameObject effect;
            if (_effectPools[effectName].Count > 0)
            {
                effect = _effectPools[effectName].Dequeue();
            }
            else
            {
                effect = UnityEngine.Object.Instantiate(_effectPrefabs[effectName]);
            }

            effect.transform.position = position;
            effect.SetActive(true);

            // 播放特效动画
            Animator animator = effect.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("play");
            }

            // 特效播放完成后回收
            MonoBehaviour.Destroy(effect, 2f);

            return effect;
        }

        public void RecycleEffect(string effectName, GameObject effect)
        {
            if (_effectPools.ContainsKey(effectName))
            {
                effect.SetActive(false);
                _effectPools[effectName].Enqueue(effect);
            }
        }
    }
}
