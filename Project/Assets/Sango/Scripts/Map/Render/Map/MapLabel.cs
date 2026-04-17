using UnityEngine;
using Sango.Core;
using Sango.Tools;

namespace Sango.Render
{
    /// <summary>
    /// 地图标注类
    /// </summary>
    public class MapLabel : MonoBehaviour, IMapManageObject
    {
        /// <summary>
        /// 地图渲染管理器
        /// </summary>
        public MapRender manager { get; set; }

        /// <summary>
        /// 边界
        /// </summary>
        public Sango.Tools.Rect bounds { get; set; }

        /// <summary>
        /// 世界边界
        /// </summary>
        public Sango.Tools.Rect worldBounds
        {
            get
            {
                Vector3 pos = position;
                return new Tools.Rect(pos.z + bounds.x - bounds.width / 2,
                pos.x + bounds.y - bounds.height / 2,
                bounds.width, bounds.height);
            }
        }



        /// <summary>
        /// 对象ID
        /// </summary>
        public int objId { get; set; }

        /// <summary>
        /// 对象类型
        /// </summary>
        public int objType { get; set; } = 99; // 标注类型

        /// <summary>
        /// 绑定ID
        /// </summary>
        public int bindId { get; set; }

        /// <summary>
        /// 模型资源
        /// </summary>
        public string modelAsset { get; set; }

        /// <summary>
        /// 是否已经添加
        /// </summary>
        public bool isAdded { get; set; }

        /// <summary>
        /// 模型ID
        /// </summary>
        public int modelId { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        private bool _visible = true;
        public bool visible 
        {
            get { return _visible; }
            set 
            {
                if (_visible != value)
                {
                    _visible = value;
                    
                    if (_visible)
                    {
                        // 从false变为true，创建资源
                        CreateLabelResource();
                    }
                    else
                    {
                        // 从true变为false，销毁资源
                        DestroyLabelResource();
                    }
                }
            }
        }

        /// <summary>
        /// 是否静态
        /// </summary>
        public bool isStatic { get; set; } = true;

        /// <summary>
        /// 是否可选择
        /// </summary>
        public bool selectable { get; set; } = false;

        /// <summary>
        /// 是否保持在视图中
        /// </summary>
        public bool remainInView { get; set; } = true;

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public Vector3 rotation
        {
            get { return transform.rotation.eulerAngles; }
            set { transform.rotation = Quaternion.Euler(value); }
        }

        /// <summary>
        /// 前向方向
        /// </summary>
        public Vector3 forward
        {
            get { return transform.forward; }
            set { transform.forward = value; }
        }

        /// <summary>
        /// 缩放
        /// </summary>
        public Vector3 scale
        {
            get { return transform.localScale; }
            set { transform.localScale = value; }
        }

        /// <summary>
        /// 坐标
        /// </summary>
        public Vector2Int coords { get; set; }

        /// <summary>
        /// 标注文字
        /// </summary>
        public string labelText { get; set; }

        /// <summary>
        /// 文字颜色
        /// </summary>
        public Color32 textColor { get; set; }

        /// <summary>
        /// 字号大小
        /// </summary>
        public int fontSize { get; set; }

        /// <summary>
        /// 文本Mesh组件
        /// </summary>
        private TextMesh textMesh;
        
        /// <summary>
        /// 标注资源对象
        /// </summary>
        private GameObject labelResource;
        
        /// <summary>
        /// Text组件
        /// </summary>
        private UnityEngine.UI.Text textComponent;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="map">地图渲染器</param>
        /// <param name="text">标注文字</param>
        /// <param name="pos">位置</param>
        /// <param name="color">文字颜色</param>
        /// <param name="size">字号大小</param>
        public void Initialize(MapRender map, string text, Vector3 pos, Color32 color, int size)
        {
            manager = map;
            labelText = text;
            position = pos;
            textColor = color;
            fontSize = size;
            bounds = new Sango.Tools.Rect(0, 0, 20, 20);

            // 创建文本Mesh
            //textMesh = gameObject.AddComponent<TextMesh>();
            //textMesh.text = labelText;
            //textMesh.color = textColor;
            //textMesh.fontSize = fontSize;
            //textMesh.anchor = TextAnchor.MiddleCenter;
            //textMesh.alignment = TextAlignment.Center;

            //// 确保文本面向相机
            //transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

            // 如果可见，创建资源
            if (visible)
            {
                CreateLabelResource();
            }
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        public void OnClick() { }

        /// <summary>
        /// 检查是否与矩形重叠
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <returns>是否重叠</returns>
        public bool Overlaps(Sango.Tools.Rect rect)
        {
            return worldBounds.Overlaps(rect);
        }

        /// <summary>
        /// 鼠标进入事件
        /// </summary>
        public void OnPointerEnter() { }

        /// <summary>
        /// 鼠标退出事件
        /// </summary>
        public void OnPointerExit() { }

        /// <summary>
        /// 设置轮廓显示
        /// </summary>
        /// <param name="material">材质</param>
        public void SetOutlineShow(Material material) { }

        /// <summary>
        /// 编辑器显示
        /// </summary>
        /// <param name="b">是否显示</param>
        public void EditorShow(bool b) { }

        /// <summary>
        /// 设置父对象
        /// </summary>
        /// <param name="parent">父对象</param>
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        /// <summary>
        /// 设置父对象
        /// </summary>
        /// <param name="parent">父对象</param>
        /// <param name="worldPositionStays">是否保持世界位置</param>
        public void SetParent(Transform parent, bool worldPositionStays)
        {
            transform.SetParent(parent, worldPositionStays);
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        public void Destroy()
        {
            // 销毁标注资源
            DestroyLabelResource();
            
            if (gameObject != null)
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        /// <summary>
        /// 获取游戏对象
        /// </summary>
        /// <returns>游戏对象</returns>
        public GameObject GetGameObject()
        {
            return gameObject;
        }

        /// <summary>
        /// 创建模型
        /// </summary>
        /// <param name="meshFile">网格文件</param>
        /// <param name="textureFile">纹理文件</param>
        /// <param name="shaderName">着色器名称</param>
        /// <param name="isShareMat">是否共享材质</param>
        public void CreateModel(string meshFile, string textureFile, string shaderName, bool isShareMat = true) { }

        /// <summary>
        /// 创建模型
        /// </summary>
        /// <param name="packagePath">包路径</param>
        /// <param name="assetName">资源名称</param>
        public void CreateModel(string packagePath, string assetName) { }

        /// <summary>
        /// 创建模型
        /// </summary>
        /// <param name="assetName">资源名称</param>
        public void CreateModel(string assetName) { }

        /// <summary>
        /// 创建模型
        /// </summary>
        /// <param name="modelObj">模型对象</param>
        public void CreateModel(UnityEngine.Object modelObj) { }

        /// <summary>
        /// 更改模型
        /// </summary>
        /// <param name="newAsset">新资源</param>
        public void ChangeModel(string newAsset) { }

        /// <summary>
        /// 重新加载模型
        /// </summary>
        /// <param name="checkAsset">是否检查资源</param>
        public void ReLoadModels(bool checkAsset = true) { }

        /// <summary>
        /// 清除模型
        /// </summary>
        public void ClearModels() { }

        /// <summary>
        /// 重新检查可见性
        /// </summary>
        public void ReCheckVisible() { }

        /// <summary>
        /// 更新标注
        /// </summary>
        /// <param name="text">新的文字</param>
        /// <param name="color">新的颜色</param>
        /// <param name="size">新的字号</param>
        public void UpdateLabel(string text, Color32 color, int size)
        {
            labelText = text;
            textColor = color;
            fontSize = size;
            
            if (textMesh != null)
            {
                textMesh.text = labelText;
                textMesh.color = textColor;
                textMesh.fontSize = fontSize;
            }
            
            if (textComponent != null)
            {
                textComponent.text = labelText;
                textComponent.color = textColor;
                textComponent.fontSize = fontSize;
            }
        }
        
        /// <summary>
        /// 创建标注资源
        /// </summary>
        private void CreateLabelResource()
        {
            if (labelResource == null)
            {
                const string poolKey = "MapLabel";
                
                // 尝试从对象池获取
                labelResource = Sango.PoolManager.Get(poolKey);
                
                if (labelResource == null)
                {
                    // 加载Resources:MapLabel资源
                    UnityEngine.Object prefab = Resources.Load("MapLabel");
                    if (prefab != null)
                    {
                        // 添加到对象池
                        Sango.PoolManager.Add(poolKey, prefab);
                        
                        // 从对象池获取
                        labelResource = Sango.PoolManager.Get(poolKey);
                    }
                }
                
                if (labelResource != null)
                {
                    // 设置父对象
                    labelResource.transform.parent = transform;
                    labelResource.transform.localPosition = Vector3.zero;
                    labelResource.transform.localRotation = Quaternion.identity;
                    labelResource.transform.localScale = Vector3.one;
                    labelResource.SetActive(true);
                    
                    // 获取Text组件
                    textComponent = labelResource.GetComponentInChildren<UnityEngine.UI.Text>(true);
                    if (textComponent != null)
                    {
                        // 赋值
                        textComponent.text = labelText;
                        textComponent.color = textColor;
                        textComponent.fontSize = fontSize;
                    }
                }
            }
        }
        
        /// <summary>
        /// 销毁标注资源
        /// </summary>
        private void DestroyLabelResource()
        {
            if (labelResource != null)
            {
                // 还回对象池
                Sango.PoolManager.Recycle(labelResource);
                labelResource = null;
                textComponent = null;
            }
        }
    }
}