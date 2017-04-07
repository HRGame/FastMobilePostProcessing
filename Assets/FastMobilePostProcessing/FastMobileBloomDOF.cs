namespace FastMobilePostProcessing
{
	using UnityEngine;
	using UnityStandardAssets.ImageEffects;

	[ExecuteInEditMode]
	[RequireComponent( typeof( Camera ) )]
	[AddComponentMenu( "FastMobilePostProcessing/FastMobileBloom" )]
	public class FastMobileBloomDOF : PostEffectsBase
	{
		public bool EnableBloom = true;
		public bool EnableDOF = true;

		[Range( 0.25f, 5.5f )]
		public float BlurSize = 1.0f;
		[Range( 1, 4 )]
		public int BlurIterations = 2;
		[Range( 0.0f, 1.5f )]
		public float bloomThreshold = 0.25f;
		[Range( 0.0f, 2.5f )]
		public float BloomIntensity = 1.0f;

		public Transform FocalTransform;	// 聚焦物体
		public float FocalLength = 3f;		// 焦点距离（焦点到相机的距离）
		public float FocalSize = 0.2f;		// 景深大小
		public float Aperture = 2f;			// 光圈（景深系数，光圈越大景深越浅）

		Camera _camera;
		Shader _shader;
		Material _material;

		void OnEnable()
		{
			_camera = GetComponent<Camera>();
		}

		void OnDisable()
		{
			if( _material )
			{
				DestroyImmediate( _material );
			}
		}

		public override bool CheckResources()
		{
			if( !EnableBloom && !EnableDOF )
				return false;

			CheckSupport( EnableDOF );

			if( _shader == null )
			{
				_shader = Shader.Find( "Hidden/FastMobileBloom" );
			}
			_material = CheckShaderAndCreateMaterial( _shader, _material );

			if( !isSupported )
			{
				ReportAutoDisable();
			}

			return isSupported;
		}

		void OnRenderImage( RenderTexture source, RenderTexture destination )
		{
			if( CheckResources() == false )
			{
				Graphics.Blit( source, destination );
				return;
			}

			_camera.depthTextureMode = EnableDOF ? DepthTextureMode.Depth : DepthTextureMode.None;

			// Initial downsample
			RenderTexture rt = RenderTexture.GetTemporary( source.width / 4, source.height / 4, 0, source.format );
			rt.filterMode = FilterMode.Bilinear;

			_material.SetFloat( "_BlurSize", BlurSize );
			_material.SetFloat( "_BloomThreshold", bloomThreshold );
			_material.SetFloat( "_BloomIntensity", BloomIntensity );
			Graphics.Blit( source, rt, _material, 0 );

			// Downscale
			for( int i = 0; i < BlurIterations - 1; ++i )
			{
				RenderTexture rt2 = RenderTexture.GetTemporary( rt.width / 2, rt.height / 2, 0, source.format );
				rt2.filterMode = FilterMode.Bilinear;

				Graphics.Blit( rt, rt2, _material, 1 );

				RenderTexture.ReleaseTemporary( rt );
				rt = rt2;
			}
			// Upscale
			for( int i = 0; i < BlurIterations - 1; ++i )
			{
				RenderTexture rt2 = RenderTexture.GetTemporary( rt.width * 2, rt.height * 2, 0, source.format );
				rt2.filterMode = FilterMode.Bilinear;

				Graphics.Blit( rt, rt2, _material, 2 );

				RenderTexture.ReleaseTemporary( rt );
				rt = rt2;
			}

			_material.SetTexture( "_BlurredTex", rt );
			if( EnableDOF )
			{
				float focalDistance01;
				if (FocalTransform != null)
				{
					FocalLength = _camera.WorldToScreenPoint( FocalTransform.position ).z;
				}
				_material.SetFloat( "_FocalLength", FocalLength / _camera.farClipPlane );
				_material.SetFloat( "_FocalSize", FocalSize );
				_material.SetFloat( "_Aperture", Aperture );
			}
			if( EnableBloom && EnableDOF )
			{
				Graphics.Blit( source, destination, _material, 3 );
			}
			else if( EnableBloom )
			{
				Graphics.Blit( source, destination, _material, 4 );
			}
			else if( EnableDOF )
			{
				Graphics.Blit( source, destination, _material, 5 );
			}

			RenderTexture.ReleaseTemporary( rt );
		}
	}
}
