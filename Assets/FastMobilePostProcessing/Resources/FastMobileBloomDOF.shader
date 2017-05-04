Shader "Hidden/FastMobilePostProcessing"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurredTex ("Bloom (RGB)", 2D) = "black" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform half4 _MainTex_TexelSize;
	uniform	half4 _MainTex_ST;

	uniform half _BlurSize;

	uniform sampler2D _BlurredTex;
	uniform half _BloomThreshold;
	uniform half _BloomIntensity;

	uniform half _FocalLength;
	uniform half _FocalSize;
	uniform half _Aperture;
	uniform sampler2D _CameraDepthTexture;

	uniform float _RadialBlurCenterX, _RadialBlurCenterY;
	uniform float _RadialBlurSampleDistance;
	uniform int _RadialBlurSamples;
	uniform float _RadialBlurStrength;
	//uniform half _SampleDist;
	//uniform half _SampleStrength;
	uniform sampler2D _RadialBlurredTex;

	struct v2fBlurDown
	{
		float4 pos  : SV_POSITION;
		half2  uv0  : TEXCOORD0;
		half4  uv12 : TEXCOORD1;
		half4  uv34 : TEXCOORD2;
	};

	struct v2fBlurUp
	{
		float4 pos  : SV_POSITION;
		half4  uv12 : TEXCOORD0;
		half4  uv34 : TEXCOORD1;
		half4  uv56 : TEXCOORD2;
		half4  uv78 : TEXCOORD3;
	};

	struct v2fCombineBloom
	{
		float4 pos : SV_POSITION; 
		half2  uv  : TEXCOORD0;
#if UNITY_UV_STARTS_AT_TOP
		half2  uv2 : TEXCOORD1;
#endif
	};	

	struct v2fRadialBlur
	{
		float4 pos : SV_POSITION;
		half2  uv  : TEXCOORD0;
	};

	v2fBlurDown vertBlurDown(appdata_img v)
	{
		v2fBlurDown o;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv0 = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy, _MainTex_ST);
		o.uv12.xy = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy + half2( 1.0h,  1.0h) * _MainTex_TexelSize.xy * _BlurSize, _MainTex_ST);
		o.uv12.zw = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy + half2(-1.0h,  1.0h) * _MainTex_TexelSize.xy * _BlurSize, _MainTex_ST);
		o.uv34.xy = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy + half2(-1.0h, -1.0h) * _MainTex_TexelSize.xy * _BlurSize, _MainTex_ST);
		o.uv34.zw = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy + half2( 1.0h, -1.0h) * _MainTex_TexelSize.xy * _BlurSize, _MainTex_ST);

		return o;
	}

	v2fBlurUp vertBlurUp(appdata_img v)
	{
		v2fBlurUp o;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv12.xy = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy + half2( 1.0h,  1.0h) * _MainTex_TexelSize.xy * _BlurSize, _MainTex_ST);
		o.uv12.zw = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy + half2(-1.0h,  1.0h) * _MainTex_TexelSize.xy * _BlurSize, _MainTex_ST);
		o.uv34.xy = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy + half2(-1.0h, -1.0h) * _MainTex_TexelSize.xy * _BlurSize, _MainTex_ST);
		o.uv34.zw = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy + half2( 1.0h, -1.0h) * _MainTex_TexelSize.xy * _BlurSize, _MainTex_ST);
		o.uv56.xy = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy + half2( 0.0h,  2.0h) * _MainTex_TexelSize.xy * _BlurSize, _MainTex_ST);
		o.uv56.zw = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy + half2( 0.0h, -2.0h) * _MainTex_TexelSize.xy * _BlurSize, _MainTex_ST);
		o.uv78.xy = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy + half2( 2.0h,  0.0h) * _MainTex_TexelSize.xy * _BlurSize, _MainTex_ST);
		o.uv78.zw = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy + half2(-2.0h,  0.0h) * _MainTex_TexelSize.xy * _BlurSize, _MainTex_ST);

		return o;
	}

	v2fCombineBloom vertCombine(appdata_img v)
	{
		v2fCombineBloom o;
	
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
#if UNITY_UV_STARTS_AT_TOP
		o.uv2 = o.uv;
		if (_MainTex_TexelSize.y < 0.0)
		{
			o.uv.y = 1.0 - o.uv.y;
		}
#endif

		return o;
	}

	v2fRadialBlur vertRadialBlur(appdata_img v)
	{
		v2fRadialBlur o;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);

		return o;
	}

/*
	fixed4 fragBlurDownFirstPass(v2fBlurDown i) : SV_Target
	{
		fixed4 col0 = tex2D(_MainTex, i.uv0);
		fixed4 col1 = tex2D(_MainTex, i.uv12.xy);
		fixed4 col2 = tex2D(_MainTex, i.uv12.zw);
		fixed4 col3 = tex2D(_MainTex, i.uv34.xy);
		fixed4 col4 = tex2D(_MainTex, i.uv34.zw);

		fixed4 col = col0 + col1 * 0.25 + col2 * 0.25 + col3 * 0.25 + col4 * 0.25;
		col = col * 0.5;
		col = max(col, 0.0);

		return col;
	}
*/

	fixed4 fragBlurDown(v2fBlurDown i) : SV_Target
	{
		fixed4 col0 = tex2D(_MainTex, i.uv0);
		fixed4 col1 = tex2D(_MainTex, i.uv12.xy);
		fixed4 col2 = tex2D(_MainTex, i.uv12.zw);
		fixed4 col3 = tex2D(_MainTex, i.uv34.xy);
		fixed4 col4 = tex2D(_MainTex, i.uv34.zw);

		fixed4 col = col0 + col1 * 0.25 + col2 * 0.25 + col3 * 0.25 + col4 * 0.25;
		col = col * 0.5;

		return col;
	}

	#define oneTwelve  0.0833333h
	#define oneSix     0.1666666h
	fixed4 fragBlurUp(v2fBlurUp i) : SV_Target
	{
		fixed4 col1 = tex2D(_MainTex, i.uv12.xy);
		fixed4 col2 = tex2D(_MainTex, i.uv12.zw);
		fixed4 col3 = tex2D(_MainTex, i.uv34.xy);
		fixed4 col4 = tex2D(_MainTex, i.uv34.zw);
		fixed4 col5 = tex2D(_MainTex, i.uv56.xy);
		fixed4 col6 = tex2D(_MainTex, i.uv56.zw);
		fixed4 col7 = tex2D(_MainTex, i.uv78.xy);
		fixed4 col8 = tex2D(_MainTex, i.uv78.zw);

		fixed4 col = col1 * oneSix + col2 * oneSix + col3 * oneSix + col4 * oneSix + col5 * oneTwelve + col6 * oneTwelve + col7 * oneTwelve + col8 * oneTwelve;

		return col;
	}

/*
	fixed4 fragRadialBlur(v2fRadialBlur i) : SV_Target
	{
		fixed2 dir = 0.5 - i.uv;
		fixed dist = length(dir);
		dir /= dist;
		dir *= _SampleDist;
  
		fixed4 sum = tex2D(_MainTex, i.uv - dir*0.01);
		sum += tex2D(_MainTex, i.uv - dir*0.02);
		sum += tex2D(_MainTex, i.uv - dir*0.03);
		sum += tex2D(_MainTex, i.uv - dir*0.05);
		sum += tex2D(_MainTex, i.uv - dir*0.08);
		sum += tex2D(_MainTex, i.uv + dir*0.01);
		sum += tex2D(_MainTex, i.uv + dir*0.02);
		sum += tex2D(_MainTex, i.uv + dir*0.03);
		sum += tex2D(_MainTex, i.uv + dir*0.05);
		sum += tex2D(_MainTex, i.uv + dir*0.08);
		sum *= 0.1;
	  
		return sum;
	}
*/

	fixed4 fragRadialBlur(v2fRadialBlur i) : SV_Target
	{
		float2 blurCenter = float2(_RadialBlurCenterX, _RadialBlurCenterY);
		float4 result = (float4)0;

		for(int n = 0; n < _RadialBlurSamples; ++n)
		{ 
			float scale = 1.0f - _RadialBlurSampleDistance * 0.1f * (n / (float)(_RadialBlurSamples - 1));
			result += tex2D(_MainTex, (i.uv - blurCenter) * scale + blurCenter);
		} 
		result /= _RadialBlurSamples;

		return result;
	}

	fixed4 fragCombineBloomAndDOF(v2fCombineBloom i) : SV_Target
	{
#if UNITY_UV_STARTS_AT_TOP
		fixed4 col = tex2D(_MainTex, i.uv2);
#else
		fixed4 col = tex2D(_MainTex, i.uv);
#endif
		fixed4 col2 = tex2D(_BlurredTex, i.uv);

		fixed weight = 0;
#if DOF_ON
		float depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv)).r;
		weight = _Aperture * abs(depth - _FocalLength) / (depth + 1e-5f);
		weight = saturate(max(0, weight - _FocalSize));
		col = lerp(col, col2, weight);
#endif
#if BLOOM_ON
		col = col + saturate(col2 - _BloomThreshold) * _BloomIntensity * (1 - weight);
#endif

		return col;
	}

	fixed4 fragCombineRadialBlur(v2fCombineBloom i) : SV_Target
	{
#if UNITY_UV_STARTS_AT_TOP
		half2 uv = i.uv2;
#else
		half2 uv = i.uv;
#endif
		fixed4 col = tex2D(_MainTex, uv);
		fixed dist = length(half2(_RadialBlurCenterX, _RadialBlurCenterY) - uv);
		fixed4 blur = tex2D(_RadialBlurredTex, uv);
		col = lerp(col, blur, saturate(dist * _RadialBlurStrength));

		return col;
	}

	ENDCG

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		// 0: downsample pass
		Pass
		{
			CGPROGRAM

			#pragma vertex vertBlurDown
			#pragma fragment fragBlurDown

			ENDCG
		}

		// 1: Upsample pass
		Pass
		{
			CGPROGRAM

			#pragma vertex vertBlurUp
			#pragma fragment fragBlurUp

			ENDCG
		}

		// 2: Radial Blur
		Pass
		{
			CGPROGRAM

			#pragma vertex vertRadialBlur
			#pragma fragment fragRadialBlur

			ENDCG
		}

		// 3: Final bloom and DoF
		Pass
		{
			CGPROGRAM

			#pragma vertex vertCombine
			#pragma fragment fragCombineBloomAndDOF

			#pragma multi_compile __ BLOOM_ON
			#pragma multi_compile __ DOF_ON
			#pragma multi_compile __ RADIAL_BLUR_ON

			ENDCG
		}

		// 4: Final Radial Blur
		Pass
		{
			CGPROGRAM

			#pragma vertex vertCombine
			#pragma fragment fragCombineRadialBlur

			ENDCG
		}
	}
}
