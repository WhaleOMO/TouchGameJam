Shader "Unlit/CharacterUnlit"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "black" {}
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

            int _ColorIsChanging;
            fixed _ChangeProgress;
            fixed3 _OriginCol;
            fixed3 _ClothCol;
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 noiseUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.noiseUV = TRANSFORM_TEX(v.uv, _NoiseTex);
                return o;
            }

            fixed4 frag(v2f i): SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                fixed noise = tex2D(_NoiseTex, i.noiseUV);
                fixed3 oriCol = c.r;
                fixed clothMask = c.g;
                fixed3 clothCol = _ClothCol;
                if(_ColorIsChanging == 1)
                {
                    fixed lerpFac = clamp(noise - 0.5 + _ChangeProgress * 2.0, 0, 1);
                    clothCol = lerp(_OriginCol, _ClothCol, lerpFac);
                }
                fixed3 finalCol = lerp(oriCol, clothCol, clothMask);
                return fixed4(finalCol, c.a);
            }
            ENDCG
        }
    }
}