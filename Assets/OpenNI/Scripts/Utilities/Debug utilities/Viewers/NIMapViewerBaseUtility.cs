/****************************************************************************
*                                                                           *
*  OpenNI Unity Toolkit                                                     *
*  Copyright (C) 2011 PrimeSense Ltd.                                       *
*                                                                           *
*                                                                           *
*  OpenNI is free software: you can redistribute it and/or modify           *
*  it under the terms of the GNU Lesser General Public License as published *
*  by the Free Software Foundation, either version 3 of the License, or     *
*  (at your option) any later version.                                      *
*                                                                           *
*  OpenNI is distributed in the hope that it will be useful,                *
*  but WITHOUT ANY WARRANTY; without even the implied warranty of           *
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the             *
*  GNU Lesser General Public License for more details.                      *
*                                                                           *
*  You should have received a copy of the GNU Lesser General Public License *
*  along with OpenNI. If not, see <http://www.gnu.org/licenses/>.           *
*                                                                           *
****************************************************************************/
using UnityEngine;
using OpenNI;

/// @brief a base class for debug utilities which show a texture (such as depth and map).
/// 
/// This class is aimed at being extended. It represents a viewer utility which
/// can be extended to show a window with any kind of texture (such as image map, depth map etc.)
/// @ingroup OpenNIViewerUtilities
public class NIMapViewerBaseUtility : MonoBehaviour 
{
    /// a link to the object with the NI context
    public OpenNISettingsManager m_context;
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



    /// This is the factor in which we will reduce the received image. A factor of 1
    /// means we use all pixels, a factor of 2 means we use only half the pixels in each
    /// direction.
    public int m_factor;

    /// the texture we will use to build the image into.
    protected Texture2D m_mapTexture;

    /// this is where we will put the texture values before inserting them to the texture itself.
    protected Color[] m_mapPixels;

    /// how many pixels in the x axis
    protected int XRes;
    /// how many pixels in the y axis
    protected int YRes;

    /// holds true if we are valid
    protected bool m_valid=false;

    /// this method should be overridden to set the internal texture and size
    /// (this will be entered into imageMapTexture, XRes and YRes). 
    /// @param refText (output) The texture created
    /// @param[out] xSize (output) the calculated size along the x axis
    /// @param[out] ySize (output) the calculated size along the y axis
    /// @return true on success and false on failure.
    /// @note this is a good place to put tests if externals are valid.
    protected virtual bool InitTexture(out Texture2D refText, out int xSize, out int ySize)
    {
        refText=null;
        xSize=-1;
        ySize=-1;
        return true;
    }

    /// A method for initialization. It is called from the mono-behavior start method.
    /// @note if an extending class overwrite start, they should either call base.Start or simply call this 
    /// method!
    protected virtual void InternalStart()
    {
        if (m_context == null)
            m_context = FindObjectOfType(typeof(OpenNISettingsManager)) as OpenNISettingsManager;
        if (m_context==null || m_context.Valid == false)
        {
            string str = "Context is invalid";
            if (m_context == null)
                str = "Context is null";
            Debug.Log(str);
            m_valid = false;
            return;
        }
        if(InitTexture(out m_mapTexture, out XRes, out YRes)==false || m_mapTexture==null)
        {
            m_context.m_Logger.Log("Failed to init texture", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects, NIEventLogger.VerboseLevel.Errors);
            m_valid=false;
            return;
        }
        m_mapPixels = new Color[XRes * YRes];
        m_valid = true;
    }

    /// @brief Mono behavior start.
    public void Start()
    {
        InternalStart();
    }

    /// Internal calculates the texture for the current frame and write it to the internal texture
    protected virtual void CalcTexture()
    {

    }

    /// Update is called once per frame by mono-behavior
    /// @note if an extending class create a new Update method they should either call base.Update or call
    /// CalcTexture.
    public void Update()
    {
        if (m_valid)
            CalcTexture();
    }

    /// used to draw the texture
    public void OnGUI()
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
        if(m_valid)
            GUI.DrawTexture(posToPut, m_mapTexture, ScaleMode.StretchToFill);
    }
}
