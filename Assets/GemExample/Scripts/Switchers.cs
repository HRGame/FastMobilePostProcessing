using FxProNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switchers : MonoBehaviour
{
	public FastMobilePostProcessing.FastMobileBloomDOF fastMobileBloomDOF;
	public UnityStandardAssets.ImageEffects.BloomOptimized bloomOptimized;
	public Text bloomText;

	//public FastMobilePostProcessing.FastMobileDoF fastMobileDoF;
	public UnityStandardAssets.ImageEffects.DepthOfField dof;
	public Text dofText;

	public FxPro fxPro;

	DOFHelperParams fxProDOFParam;

	int bloomIndex = 0;
	int dofIndex = 0;

	void Start()
	{
		fxPro.BloomEnabled = false;

		fxProDOFParam = fxPro.DOFParams;
		fxPro.DOFEnabled = false;
	}

	public void OnBloomButtonClicked()
	{
		bloomIndex = ( bloomIndex + 1 ) % 3;
		switch( bloomIndex )
		{
		case 1:
			{
				fastMobileBloomDOF.enabled = true;
				fastMobileBloomDOF.EnableBloom = true;
				bloomOptimized.enabled = false;

				bloomText.text = "FastMobileBloom";
			}
			break;
		case 2:
			{
				fastMobileBloomDOF.enabled = false;
				fastMobileBloomDOF.EnableBloom = false;
				bloomOptimized.enabled = true;

				bloomText.text = "BloomOptimized";
			}
			break;
		default:
			{
				fastMobileBloomDOF.enabled = false;
				fastMobileBloomDOF.EnableBloom = false;
				bloomOptimized.enabled = false;

				bloomText.text = "No Bloom";
			}
			break;
		}
	}

	public void OnDoFButtonClicked()
	{
		dofIndex = ( dofIndex + 1 ) % 4;
		switch( dofIndex )
		{
		case 1:
			{
				fastMobileBloomDOF.enabled = true;
				fastMobileBloomDOF.EnableDOF = true;
				fxPro.enabled = false;
				fxPro.DOFEnabled = false;
				bloomOptimized.enabled = false;

				dofText.text = "FastMobileDoF";
			}
			break;
		case 2:
			{
				fastMobileBloomDOF.enabled = false;
				fastMobileBloomDOF.EnableDOF = false;
				fxPro.enabled = false;
				fxPro.DOFEnabled = false;
				dof.enabled = true;

				dofText.text = "DepthOfField";
			}
			break;
		case 3:
			{
				fastMobileBloomDOF.enabled = false;
				fastMobileBloomDOF.EnableDOF = false;
				fxPro.DOFParams = fxProDOFParam;
				fxPro.DOFEnabled = true;
				fxPro.enabled = true;
				bloomOptimized.enabled = false;

				dofText.text = "FxPro DoF";
			}
			break;
		default:
			{
				fastMobileBloomDOF.enabled = false;
				fastMobileBloomDOF.EnableDOF = false;
				fxPro.enabled = false;
				fxPro.DOFEnabled = false;
				dof.enabled = false;

				dofText.text = "No DoF";
			}
			break;
		}
	}
}
