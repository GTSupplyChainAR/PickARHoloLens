using System;
using System.Collections;
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

        /// <summary> Keeps track of HoloLens photo capture. </summary>
        private PhotoCapture capture;
        /// <summary> Variables to pass into photo capture. </summary>
        private CameraParameters cameraParameters;

        /// <summary> The rate that scanning takes place at (seconds). </summary>
        private const float SCAN_RATE = 2.0f;
#if !UNITY_EDITOR
        /// <summary> The possible barcode formats that the scanner can read. </summary>
        private static BarcodeFormat[] formats = new BarcodeFormat[] { BarcodeFormat.CODE_128 };
#endif

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            textMesh = textMeshObject.GetComponent<TextMesh>();
            if (InputSwitcher.isHoloLens) {
                InvokeRepeating("StartScan", SCAN_RATE, SCAN_RATE);
            }
        }

        /// <summary>
        /// Initializes the photo capture on the HoloLens.
        /// </summary>
        private void StartScan() {
            if (!JobManager.instance.jobActive) {
                return;
            }
            textMesh.text = "scanning...";

            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        }

        /// <summary>
        /// Finds the best resolution and starts photo mode.
        /// </summary>
        /// <param name="capture">The camera feed.</param>
        private void OnPhotoCaptureCreated(PhotoCapture captureObject) {
            capture = captureObject;
            
            IEnumerable<Resolution> resolutions = PhotoCapture.SupportedResolutions;
            Resolution bestResolution = new Resolution();
            int bestResolutionNum = int.MaxValue;
            foreach (Resolution resolution in resolutions) {
                int currentNum = resolution.width * resolution.height;
                if (currentNum < bestResolutionNum) {
                    bestResolution = resolution;
                    bestResolutionNum = currentNum;
                }
            }

            cameraParameters.cameraResolutionWidth = bestResolution.width;
            cameraParameters.cameraResolutionHeight = bestResolution.height;

            capture.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
        }

        /// <summary>
        /// Takes a photo after photo mode starts.
        /// </summary>
        /// <param name="result">The result of the photo capture.</param>
        private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result) {
            if (result.success) {
                capture.TakePhotoAsync(OnCapturedPhotoToMemory);
            } else {
                Debug.LogError("Unable to start photo mode!");
            }
        }

        /// <summary>
        /// Reads a photo after it is captured.
        /// </summary>
        /// <param name="result">The result of the photo capture.</param>
        /// <param name="photoCaptureFrame">The photo capture data.</param>
        private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame) {
            if (result.success) {
                List<byte> imageBufferList = new List<byte>();
                // Copy the raw IMFMediaBuffer data into our empty byte list.
                photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList); 
                byte[] imageArray = imageBufferList.ToArray();

                string text = "Nothing";

#if !UNITY_EDITOR
                BarcodeReader reader = new BarcodeReader();
                reader.Options.PossibleFormats = formats;
                Result decodeResult = reader.Decode(imageArray, cameraParameters.cameraResolutionWidth, cameraParameters.cameraResolutionHeight, BitmapFormat.BGRA32);
                if (decodeResult != null) {
                    text = decodeResult.Text;
                }
#endif

                textMesh.text = text;
                // JobManager.instance.SelectItem(text);
            }
            capture.StopPhotoModeAsync(OnStoppedPhotoMode);
        }

        /// <summary>
        /// Cleans up photo capture mode after it is finished
        /// </summary>
        /// <param name="result">The result of the photo capture.</param>
        private void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result) {
            capture.Dispose();
            capture = null;
        }
    }
}