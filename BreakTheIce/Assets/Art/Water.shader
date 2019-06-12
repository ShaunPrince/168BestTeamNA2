Shader "Custom/Water"
{
    Properties
    {
        _MainTex ("Noise", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,1)
		_Step("Step of Foam", Range(0,1)) = 0.7
		_Speed("Speed", Vector) = (0.01, 0.05, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
		//Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float2 distort : TEXCOORD0;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _Color;
			float _Step;
			float2 _Speed;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.distort = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
				float2 dist = (tex2D(_MainTex, i.distort).xy - 1) * 0.2;
				float2 uvAnim = float2((i.uv.x + _Time.y * _Speed.x * 2) + dist.x, (i.uv.y + _Time.y * _Speed.x * 2) + dist.y);

				//float2 uvAnim = float2(i.uv.x + _Time.y * _Speed.x, i.uv.y + _Time.y * _Speed.y);
				fixed4 col = tex2D(_MainTex, uvAnim).r;
				
				col = col > _Step ? 1 : 0;
				col = _Color + col;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
