Shader "Unlit/NpcEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("Noise",2D) = "white" {}
        
        _Speed ("Speed", float) = 1
        _NoiseSpeed("Noise Speed",float) = (1.0,1.0,1.0,1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" 
            "DisableBatching" = "True" }

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

            float _Speed;
            sampler2D _MainTex;
            
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            half2 _NoiseSpeed;

            fixed GetCurrentProgress()
            {
                return abs(sin(frac(_Time * _Speed) * UNITY_HALF_PI));
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.xyz = v.vertex.xyz * max(GetCurrentProgress(),0.7);
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv.xy = frac(uv.xy+ _NoiseSpeed*GetCurrentProgress());
                fixed noise = tex2D(_NoiseTex,uv);
                fixed4 col = tex2D(_MainTex, i.uv);
                noise = smoothstep(0.2,0.3,noise);
                fixed alpha = col.a * lerp(GetCurrentProgress()*noise, 1.0, 0.01);
                return fixed4(col.rgb, alpha);
            }
            ENDCG
        }
    }
}
