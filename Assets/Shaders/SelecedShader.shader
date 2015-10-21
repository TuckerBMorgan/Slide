Shader "Custom/SelecedShader" {
	SubShader{
		Pass{
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			float4 vert(float4 v: POSITION) : SV_POSITION
			{
				return mul(UNITY_MATRIX_MVP, v);
			}

			fixed4 frag() : SV_TARGET
			{
				return fixed4(sin(_Time.x * 100.0) / 10 ,sin(_Time.x * 100.0) / 10.0, sin(_Time.x * 100.0) / 10.0, 0.2);
			}
			ENDCG
		}
	}
}
