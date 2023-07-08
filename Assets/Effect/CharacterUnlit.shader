Shader "Unlit/CharacterUnlit"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _ClothCol ("Cloth Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Pass
        {

            Cull Off
            Lighting Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed3 _ClothCol;
            sampler2D _MainTex;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i): SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                fixed3 oriCol = c.r;
                fixed clothMask = c.g;
                fixed3 finalCol = lerp(oriCol, _ClothCol, clothMask);
                // fixed alpha = c.a;
                // fixed3 border = c.r; // * alpha;
                // fixed3 cloth = max(c.b - c.r, 0) * _ClothCol;
                // fixed3 face = alpha * c.g;
                // fixed3 res = face + cloth + border;
                // fixed realAlpha = clamp(c.r + c.g + c.b, 0, 1);
                return fixed4(finalCol, c.a);
            }
            ENDCG
        }
    }
}