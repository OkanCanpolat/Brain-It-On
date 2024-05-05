using System.Collections;
using UnityEngine;

public class ScreenRecorder : MonoBehaviour, IScreenRecorderSprite
{
    private Sprite target;
    [SerializeField] private Transform minXY;
    [SerializeField] private Transform maxXY;

    public Sprite GetResult()
    {
        return target;
    }

    public void RecordScreen()
    {
        StartCoroutine(GetScreenShot());
    }

    private IEnumerator GetScreenShot()
    {
        yield return new WaitForEndOfFrame();

        Vector2 min = Camera.main.WorldToScreenPoint(minXY.position);
        Vector2 max = Camera.main.WorldToScreenPoint(maxXY.position);

        Texture2D a = ScreenCapture.CaptureScreenshotAsTexture();

        Rect rect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);

        target = Sprite.Create(a, rect, new Vector2(0, 0));
    }
}
