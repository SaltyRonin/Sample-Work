Shader "Hidden/Pixelate"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;	
			float2 count;
			float2 size;

			fixed4 frag (v2f_img i) : SV_Target
			{
				float2 pos = floor(i.uv * count);
				float2 center = pos * size + size * 0.5;

				float4 tex = tex2D(_MainTex, center);
				return tex;
			}
			ENDCG
		}
	}
}
