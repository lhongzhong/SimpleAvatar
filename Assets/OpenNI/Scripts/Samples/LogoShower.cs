using UnityEngine;
using System.Collections;

/// @brief A simple utility to show a logo image.
/// 
/// @ingroup OpenNISamples
public class LogoShower : MonoBehaviour 
{
    /// the logo to draw
    public Texture2D m_TextureToShow;
    /// the image will appear inside this rectangle.
    public Rect m_placeToDraw;

    /// an enum to tell us where to snap the place to draw
    public enum ScreenSnap
    {
        UpperLeftCorner,    ///< the base position (x,y) is an offset from the upper left corner
        UpperRightCorner,   ///< the base position (x,y) is an offset from the upper right corner
        LowerLeftCorner,    ///< the base position (x,y) is an offset from the lower left corner
        LowerRightCorner    ///< the base position (x,y) is an offset from the lower right corner
    };

    /// tells us how to handle @ref m_placeToDraw. The nearest corner centeral position (x,y) of @ref m_placeToDraw
    /// is considered to be relative to the corner of the snap so that the relevant corner will be of that
    /// distance from the corner of the screen.
    public ScreenSnap m_snap;

    /// used to draw the texture
    void OnGUI()
    {
        Rect posToPut = m_placeToDraw;
        switch (m_snap)
        {
            case ScreenSnap.UpperRightCorner:
                {
                    posToPut.x = Screen.width - m_placeToDraw.x - m_placeToDraw.width;
                    break;
                }
            case ScreenSnap.LowerLeftCorner:
                {
                    posToPut.y = Screen.height - m_placeToDraw.y - m_placeToDraw.height;
                    break;
                }
            case ScreenSnap.LowerRightCorner:
                {
                    posToPut.x = Screen.width - m_placeToDraw.x - m_placeToDraw.width;
                    posToPut.y = Screen.height - m_placeToDraw.y - m_placeToDraw.height;
                    break;
                }

        }
        GUI.DrawTexture(posToPut, m_TextureToShow, ScaleMode.ScaleToFit);
    }
}
