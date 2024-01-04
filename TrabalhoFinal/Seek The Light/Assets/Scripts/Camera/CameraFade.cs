using UnityEngine;

public class CameraFade : MonoBehaviour
{
    public float SpeedScale = 0.1f;
    public Color FadeColor = Color.black;
    public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 1),
        new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));

    private float m_alpha = 0f;
    private Texture2D m_texture;
    private int m_direction = 0;
    private float m_time = 0f;

    private void Start()
    {
        StartFadeIn();

        m_texture = new Texture2D(1, 1);
        m_texture.SetPixel(0, 0, new Color(FadeColor.r, FadeColor.g, FadeColor.b, m_alpha));
        m_texture.Apply();

        GameManager.Player.OnPlayerHit += (_) => { StartFadeIn(); };
    }

    private void StartFadeIn()
    {
        m_alpha = 1f;
        m_time = 0f;
        m_direction = 1;
    }

    public void OnGUI()
    {
        if (m_alpha > 0f) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_texture);
        if (m_direction != 0)
        {
            m_time += m_direction * Time.deltaTime * SpeedScale;
            m_alpha = Curve.Evaluate(m_time);
            m_texture.SetPixel(0, 0, new Color(FadeColor.r, FadeColor.g, FadeColor.b, m_alpha));
            m_texture.Apply();
            if (m_alpha <= 0f || m_alpha >= 1f) m_direction = 0;
        }
    }
}