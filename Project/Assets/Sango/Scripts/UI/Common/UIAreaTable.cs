using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
namespace Sango.Game.Render.UI
{

    public class UIAreaTable : MonoBehaviour
    {
        public RectTransform lt;


        public RectTransform ld;
        public RectTransform rt;
        public RectTransform rd;

        List<RectTransform> rt_instance_list = new List<RectTransform>();
        List<RectTransform> rd_instance_list = new List<RectTransform>();
        List<RectTransform> ld_instance_list = new List<RectTransform>();

        List<RectTransform> rt_pool_list = new List<RectTransform>();
        List<RectTransform> rd_pool_list = new List<RectTransform>();
        List<RectTransform> ld_pool_list = new List<RectTransform>();

        public List<int> col_width_list;
        public List<int> row_height_list;

        RectTransform Create(RectTransform rect)
        {
            List<RectTransform> pool_list;
            List<RectTransform> instance_list;
            if (rect == ld)
            {
                pool_list = ld_pool_list;
                instance_list = ld_instance_list;
            }
            else if (rect == rt)
            {
                pool_list = rt_pool_list;
                instance_list = rt_instance_list;
            }
            else if (rect == rd)
            {
                pool_list = rd_pool_list;
                instance_list = rd_instance_list;
            }
            else
                return null;

            if (pool_list.Count > 0)
            {
                RectTransform rectTransform = pool_list[0];
                pool_list.RemoveAt(0);
                instance_list.Add(rectTransform);
                rectTransform.gameObject.SetActive(true);
                return rectTransform;
            }
            else
            {
                RectTransform rectTransform = GameObject.Instantiate(rect.gameObject, rect.parent).GetComponent<RectTransform>();
                instance_list.Add(rectTransform);
                rectTransform.gameObject.SetActive(true);
                return rectTransform;
            }
        }

        public void Init()
        {
            int total_w = 16;
            int total_h = 16;

            int row_height = -16;
            for (int r = 0; r < row_height_list.Count; r++)
            {
                int height = row_height_list[r];
                int h = height + (r == 0 ? 0 : 7);
                RectTransform dest;
                dest = Create(ld);
                if (dest != null)
                {
                    dest.anchoredPosition = new Vector2(0, row_height);
                    dest.sizeDelta = new Vector2(16, h);
                }

                int col_width = 0;
                // 先生成列
                for (int c = 0; c < col_width_list.Count; c++)
                {
                    int width = col_width_list[c];
                    int w = width + (c == 0 ? 0 : 7);
                    Vector2 size = new Vector2(w, h);
                    int x = 16 + col_width + (c == 0 ? 0 : -7);
                    if (r == 0)
                    {
                        dest = Create(rt);
                        if (dest != null)
                        {
                            dest.anchoredPosition = new Vector2(x, 0);
                            dest.sizeDelta = new Vector2(w, 16);
                        }
                    }

                    dest = Create(rd);
                    if (dest != null)
                    {
                        dest.anchoredPosition = new Vector2(x, row_height);
                        dest.sizeDelta = size;
                    }
                    col_width += width;
                }
                total_w = col_width;
                row_height -= (h - 8);
            }
            total_h = row_height;
        
            GetComponent<RectTransform>().sizeDelta = new Vector2 (total_w + 16, -total_h + 8);
        }


        public void Init(List<int> col_width_list, List<int> row_height_list)
        {
            this.col_width_list = col_width_list;
            this.row_height_list = row_height_list;
            Init();
        }

        public void Clear()
        {
            rt_pool_list.AddRange(rt_instance_list);
            rd_pool_list.AddRange(rd_instance_list);
            ld_pool_list.AddRange(ld_instance_list);
            foreach (RectTransform r in rt_pool_list)
                r.gameObject.SetActive(false);
            foreach (RectTransform r in rd_pool_list)
                r.gameObject.SetActive(false);
            foreach (RectTransform r in ld_pool_list)
                r.gameObject.SetActive(false);
            rt_instance_list.Clear();
            rd_instance_list.Clear();
            ld_instance_list.Clear();
        }

        [UnityEngine.ContextMenu("test")]
        void Test()
        {
            List<int> list_c = new List<int>()
            {
                50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,
            };
            List<int> list_r = new List<int>()
            {
                30,30,30,
                30,30,30,
                30,30,30,
                30,30,30,
            };
            Init(list_c, list_r);
        }

        [UnityEngine.ContextMenu("test1")]
        void Test1()
        {
            Init();
        }
    }
}
