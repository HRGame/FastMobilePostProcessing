using FxProNS;
using MorePPEffects;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class Switchers : MonoBehaviour
{
	public FastMobilePostProcessing.FastMobileBloomDOF fastMobileBloomDOF;

	public UnityStandardAssets.ImageEffects.BloomOptimized unitybloomOptimized;
	public UnityStandardAssets.ImageEffects.DepthOfField unityDepthOfField;
	public UnityStandardAssets.ImageEffects.ColorCorrectionLookup unityCLUT;

	public FxPro fxPro;

	public PostProcessingBehaviour postProcessingStack;

	public RadialBlur morePPEffectsRadialBlur;

	public Text bloomText;
	public Text dofText;
	public Text colorLUTText;
	public Text radialBlurText;

	DOFHelperParams fxProDOFParam;

	int bloomIndex = 0;
	int dofIndex = 0;
	int clutIndex = 0;
	int radialBlurIndex = 0;

	void Start()
	{
		fxPro.BloomEnabled = false;

		fxProDOFParam = fxPro.DOFParams;
		fxPro.DOFEnabled = false;
	}

	public void OnBloomButtonClicked()
	{
		bloomIndex = ( bloomIndex + 1 ) % 4;
		switch( bloomIndex )
		{
		case 1: // FastMobile
			{
				fastMobileBloomDOF.EnableBloom = true;
				unitybloomOptimized.enabled = false;
				postProcessingStack.profile.bloom.enabled = false;

				bloomText.text = "FastMobileBloom";
			}
			break;
		case 2: // Unity
			{
				fastMobileBloomDOF.EnableBloom = false;
				unitybloomOptimized.enabled = true;
				postProcessingStack.profile.bloom.enabled = false;

				bloomText.text = "BloomOptimized";
			}
			break;
		case 3: // PPS
			{
				fastMobileBloomDOF.EnableBloom = false;
				unitybloomOptimized.enabled = false;
				postProcessingStack.profile.bloom.enabled = true;

				bloomText.text = "PPS Bloom";
			}
			break;
		default:
			{
				fastMobileBloomDOF.EnableBloom = false;
				unitybloomOptimized.enabled = false;
				postProcessingStack.profile.bloom.enabled = false;

				bloomText.text = "No Bloom";
			}
			break;
		}
		fastMobileBloomDOF.enabled = fastMobileBloomDOF.EnableBloom
		                             || fastMobileBloomDOF.EnableDOF
		                             || fastMobileBloomDOF.EnableRadialBlur;
		postProcessingStack.enabled = postProcessingStack.profile.bloom.enabled
		                              || postProcessingStack.profile.depthOfField.enabled
		                              || postProcessingStack.profile.userLut.enabled
		                              || postProcessingStack.profile.colorGrading.enabled;
	}

	public void OnDoFButtonClicked()
	{
		dofIndex = ( dofIndex + 1 ) % 5;
		switch( dofIndex )
		{
		case 1: // FastMobile
			{
				fastMobileBloomDOF.EnableDOF = true;
				unityDepthOfField.enabled = false;
				fxPro.enabled = false;
				fxPro.DOFEnabled = false;
				postProcessingStack.profile.depthOfField.enabled = false;

				dofText.text = "FastMobileDoF";
			}
			break;
		case 2: // Unity
			{
				fastMobileBloomDOF.EnableDOF = false;
				unityDepthOfField.enabled = true;
				fxPro.enabled = false;
				fxPro.DOFEnabled = false;
				postProcessingStack.profile.depthOfField.enabled = false;

				dofText.text = "DepthOfField";
			}
			break;
		case 3: // FxPro
			{
				fastMobileBloomDOF.EnableDOF = false;
				unityDepthOfField.enabled = false;
				fxPro.DOFParams = fxProDOFParam;
				fxPro.DOFEnabled = true;
				fxPro.enabled = true;
				postProcessingStack.profile.depthOfField.enabled = false;

				dofText.text = "FxPro DoF";
			}
			break;
		case 4: // PPS
			{
				fastMobileBloomDOF.EnableDOF = false;
				unityDepthOfField.enabled = false;
				fxPro.enabled = false;
				fxPro.DOFEnabled = false;
				postProcessingStack.profile.depthOfField.enabled = true;

				dofText.text = "PPS DoF";
			}
			break;
		default: // None
			{
				fastMobileBloomDOF.EnableDOF = false;
				unityDepthOfField.enabled = false;
				fxPro.enabled = false;
				fxPro.DOFEnabled = false;
				postProcessingStack.profile.depthOfField.enabled = false;

				dofText.text = "No DoF";
			}
			break;
		}
		fastMobileBloomDOF.enabled = fastMobileBloomDOF.EnableBloom
		                             || fastMobileBloomDOF.EnableDOF
		                             || fastMobileBloomDOF.EnableRadialBlur;
		postProcessingStack.enabled = postProcessingStack.profile.bloom.enabled
		                              || postProcessingStack.profile.depthOfField.enabled
		                              || postProcessingStack.profile.userLut.enabled
		                              || postProcessingStack.profile.colorGrading.enabled;
	}

	public void OnCLUTButtonClicked()
	{
		clutIndex = ( clutIndex + 1 ) % 4;
		switch( clutIndex )
		{
			case 1: // Unity
				{
					unityCLUT.enabled = true;
					postProcessingStack.profile.userLut.enabled = false;
					postProcessingStack.profile.colorGrading.enabled = false;

					colorLUTText.text = "Unity Color LUT";
				}
				break;
			case 2: // PPS LUT
				{
					unityCLUT.enabled = false;
					postProcessingStack.profile.userLut.enabled = true;
					postProcessingStack.profile.colorGrading.enabled = false;

					colorLUTText.text = "PPS Color LUT";
				}
				break;
			case 3: // PPS Color Grading
			{
				unityCLUT.enabled = false;
				postProcessingStack.profile.userLut.enabled = false;
				postProcessingStack.profile.colorGrading.enabled = true;

				colorLUTText.text = "PPS Color Grading";
			}
				break;
			default: // None
				{
					unityCLUT.enabled = false;
					postProcessingStack.profile.userLut.enabled = false;
					postProcessingStack.profile.colorGrading.enabled = false;

					colorLUTText.text = "No Color LUT";
				}
				break;
		}
		postProcessingStack.enabled = postProcessingStack.profile.bloom.enabled
		                              || postProcessingStack.profile.depthOfField.enabled
		                              || postProcessingStack.profile.userLut.enabled
									  || postProcessingStack.profile.colorGrading.enabled;
	}

	public void OnRadialBlurButtonClicked()
	{
		radialBlurIndex = ( radialBlurIndex + 1 ) % 3;
		switch( radialBlurIndex )
		{
			case 1: // FastMobile
			{
				fastMobileBloomDOF.EnableRadialBlur = true;
				morePPEffectsRadialBlur.enabled = false;

				radialBlurText.text = "FastMobileRadialBlur";
			}
				break;
			case 2: // MorePPEffects
			{
				fastMobileBloomDOF.EnableRadialBlur = false;
				morePPEffectsRadialBlur.enabled = true;

				radialBlurText.text = "MPPE Radial Blur";
			}
				break;
			default: // None
			{
				fastMobileBloomDOF.EnableRadialBlur = false;
				morePPEffectsRadialBlur.enabled = false;

				radialBlurText.text = "No Radial Blur";
			}
				break;
		}
		fastMobileBloomDOF.enabled = fastMobileBloomDOF.EnableBloom
		                             || fastMobileBloomDOF.EnableDOF
		                             || fastMobileBloomDOF.EnableRadialBlur;
	}
}
