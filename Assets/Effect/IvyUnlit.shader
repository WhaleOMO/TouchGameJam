Shader "Unlit/IvyUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GrowthTex ("Growth Mask", 2D) = "black" {}
        [Range(0,1)]_GrowthAmount ("Growth Amount", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _GrowthAmount;
            sampler2D _MainTex;
            sampler2D _GrowthTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed growMask = tex2D(_GrowthTex, i.uv).r;
                growMask = step(growMask, _GrowthAmount);
                return fixed4(col.rgb, col.a*growMask);
            }
            ENDCG
        }
    }
}
