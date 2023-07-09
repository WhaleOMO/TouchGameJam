Shader "Unlit/WaterfallUnlit"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "black" {}
        [HDR]_MainCol ("Main Color", Color) = (1,1,1,1)
		_Speed("Speed",float) = 1.0
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
            fixed4 _MainCol;
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
			half _Speed;

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
				i.uv.y = frac(i.uv.y + _Time.x*_Speed);
                fixed4 c = tex2D(_MainTex, i.uv);
				return c*_MainCol;
                //return fixed4(i.uv.y,i.uv.y,i.uv.y, c.a);
            }
            ENDCG
        }
    }
}