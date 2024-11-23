Shader "CustomPostEffect/KawaseBlur"
{
	Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
		_Offset ("Float", Float) = 2
		_MainTex_TexelSize ("Vector", Vector) = (0.1, 0.1, 0, 0)
    }
	HLSLINCLUDE
    #include "UnityCG.cginc"
	
	struct appdata
    {
		float4 vertex: POSITION;
        float2 uv: TEXCOORD0;
    };
	
	struct VaryingsDefault
	{
		float4 pos: SV_POSITION;
		float2 uv: TEXCOORD0;
	};

	VaryingsDefault VertDefault(appdata v)
    {
         VaryingsDefault o;
         o.pos = UnityObjectToClipPos(v.vertex);
         o.uv = v.uv;
         return o;
    }
	
	uniform half _Offset;

	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	
	half4 KawaseBlur(sampler2D tex, float2 uv, float2 texelSize, half pixelOffset)
	{
		half4 o = 0;
		o += tex2D(tex, uv + float2(pixelOffset +0.5, pixelOffset +0.5) * texelSize); 
		o += tex2D(tex, uv + float2(-pixelOffset -0.5, pixelOffset +0.5) * texelSize); 
		o += tex2D(tex, uv + float2(-pixelOffset -0.5, -pixelOffset -0.5) * texelSize); 
		o += tex2D(tex, uv + float2(pixelOffset +0.5, -pixelOffset -0.5) * texelSize);
		return o * 0.25;
	}
	
	half4 Frag(VaryingsDefault i) : SV_Target
	{
		half4 clr = KawaseBlur(_MainTex, i.uv, _MainTex_TexelSize.xy, _Offset);
		clr.a = 1;
		return clr;
	}
	
	ENDHLSL
	
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		
		Pass
		{
			HLSLPROGRAM
			
			#pragma vertex VertDefault
			#pragma fragment Frag
			
			ENDHLSL
			
		}
	}
}


