Shader "Custom/Unlit/ScreenClip"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BaseColor("BaseColor", Color) = (1, 1, 1, 1)
        _ScreenClip("ScreenClip", Vector) = (-1, -1, 0, 0)//0~1的坐标，xy左下角，zw右上角
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f 
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.uv = v.texcoord;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos (o.pos);
                return o;
            }

            sampler2D _MainTex;
			fixed4 _ScreenClip;
			fixed4 _BaseColor;
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenPos = i.screenPos.xy / i.screenPos.w;
                fixed4 col = tex2D(_MainTex, i.uv);
                if(_ScreenClip.x > -1 && _ScreenClip.y > -1)
                {
                    if (screenPos.x < _ScreenClip.x)//左
                    {
                        col.a = 0;
                    }
                    else if (screenPos.x > _ScreenClip.z)//右
                    {
                        col.a = 0;
                    }
                    else if (screenPos.y > _ScreenClip.w)//上
                    {
                        col.a = 0;
                    }
                    else if (screenPos.y < _ScreenClip.y)//下
                    {
                        col.a = 0;
                    }
                }

                // i.screenPos 在 [0,1] 区间，乘以_ScreenParams就是真实的屏幕坐标了
                //screenPos.xy *= _ScreenParams.xy;

                return col *_BaseColor;
            }
            ENDCG
        }
    }
}
