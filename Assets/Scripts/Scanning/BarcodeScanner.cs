using System;
using UnityEngine;
using UnityEngine.VR.WSA.WebCam;
using PickAR.Managers;
using System.Collections.Generic;
#if !UNITY_EDITOR
using ZXing;
#endif

namespace PickAR.Scanning {

	/// <summary>
	/// Scans barcodes to identify object.
	/// </summary>
	class BarcodeScanner : MonoBehaviour {

		/// <summary> The text object that displays the scanned barcode contents. </summary>
		[SerializeField]
		[Tooltip("The text object that displays the scanned barcode contents.")]
	    private Transform textMeshObject;
		/// <summary> The item ID that was last scanned. </summary>
		private int itemID = -1;
		/// <summary> The text mesh that contains barcode text. </summary>
		private TextMesh textMesh;

		/// <summary>
		/// Initializes the object.
		/// </summary>
	    private void Start() {
	        this.textMesh = this.textMeshObject.GetComponent<TextMesh>();
	        OnReset();
			if (InputSwitcher.isHoloLens) {
				InvokeRepeating("StartScan", 2.0f, 5.0f);
			}
	    }

		/// <summary>
		/// Initializes the photo capture on the HoloLens.
		/// </summary>
	    private void StartScan() {
			this.textMesh.text = "scanning...";

			PhotoCapture.CreateAsync(false, OnScan);
		}

		/// <summary>
		/// Scans the HoloLens camera feed for 
		/// </summary>
		private void OnScan() {


#if !UNITY_EDITOR
	        BarcodeReader reader = new BarcodeReader();
	        reader.Options.PossibleFormats = new BarcodeFormat[] { BarcodeFormat.CODE_128 };
	        Result result = reader.Decode();
	        String text = result.Text;
	  //      MediaFrameQrProcessing.Wrappers.ZXingQrCodeScanner.ScanFirstCameraForQrCode(
	  //          result => {
	  //              UnityEngine.WSA.Application.InvokeOnAppThread(() => {
	  //                this.textMesh.text = result.Text;
	  //                Int32.TryParse(this.textMesh.text, out this.itemID);
	  //            	},
	  //          false);
	  //          },
	  //          TimeSpan.FromSeconds(5)
			//);
#endif
	    }

		/// <summary>
		/// Resets the scanning state.
		/// </summary>
	    private void OnReset() {
	        if (itemID != -1) {
	            JobManager.instance.SelectItem(itemID);
	        }
	        this.textMesh.text = "say scan to start";
	    }
	}
}