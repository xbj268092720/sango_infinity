Shader "Sango/ui_alpha_map"
{
    Properties
    {
        _MainTex("MainTex",2D) = "white"{}
        _AlphaMap("AlphaTex",2D) = "white"{}
        _Color("Color",Color) = (1,1,1,1)
    }

        SubShader
        {
            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag 

                fixed4 _Color;
                sampler2D _MainTex;
                sampler2D _AlphaMap;
                float4 _MainTex_TexelSize;

                float _Radius;
                struct a2v
                {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 uv     : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 uv    : TEXCOORD0;
                };

                v2f vert(a2v v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_TARGET
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    col.a = tex2D(_AlphaMap, i.uv).a;
                    col.rgb *= _Color.rgb;
                    return col;
                }

                ENDCG
            }
        }
}
