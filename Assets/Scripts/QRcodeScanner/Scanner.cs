using System;
using UnityEngine;
using PickAR.Managers;
using System.Collections.Generic;

public class Scanner : MonoBehaviour
{
    public Transform textMeshObject;
    public int itemID = -1;

    private void Start()
    {
        this.textMesh = this.textMeshObject.GetComponent<TextMesh>();
        OnReset();
        //InvokeRepeating("OnScan", 2.0f, 5.0f);
    }
    public void OnScan()
    {
        this.textMesh.text = "scanning...";

#if !UNITY_EDITOR
    MediaFrameQrProcessing.Wrappers.ZXingQrCodeScanner.ScanFirstCameraForQrCode(
        result =>
        {
          UnityEngine.WSA.Application.InvokeOnAppThread(() =>
          {
            this.textMesh.text = result?.Text ?? "-1";
            Int32.TryParse(this.textMesh.text, out this.itemID);
          }, 
          false);
        },
        TimeSpan.FromSeconds(5));
#endif

    }

    public void OnReset()
    {
        if (itemID != -1)
        {
            JobManager.instance.SelectItem(itemID);
        }
        this.textMesh.text = "say scan to start";
    }
    TextMesh textMesh;
}