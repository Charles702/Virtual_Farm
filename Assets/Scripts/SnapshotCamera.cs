using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotCamera : MonoBehaviour
{
    public int resWidth = 1920;
    public int resHeight = 1080;
    public Camera SnapCam;

    private bool takeSnapshot = false;

    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            takeSnapshot = true;
        }

        if (takeSnapshot)
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            SnapCam.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            SnapCam.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);

            // Reset
            SnapCam.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            // save snap shot
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));

            takeSnapshot = false;
        }
    }
}
