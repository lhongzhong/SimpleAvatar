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
using OpenNI;


/// @brief Node generation for the Image node
/// 
/// This class is responsible for abstracting the image generator node. 
/// @ingroup OpenNIBasicObjects
public class NIImage : NIWrapperContextDependant
{
    /// Holds true if the image is valid
    public override bool Valid
    {
        get { return base.Valid && m_image!=null; }
    }

    /// Accessor to @ref m_image
    public ImageGenerator Image
    {
        get { return m_image; }
    }

    /// Accessor to the singleton (@ref m_singleton)
    public static NIImage Instance
    {
        get { return m_singleton; }
    }

    /// @brief Initialize the image node.
    /// 
    /// This method initializes the image node. It assumes the context is already valid (otherwise we fail).
    /// @note - Since we use a singleton pattern, multiple initializations will simply delete the old
    /// node and create a new one!
    /// @note - It is the responsibility of the initializer to call @ref Dispose.
    /// @param context the context used
    /// @param logger the logger object we will enter logs into
    /// @return true on success, false on failure. 
    public bool Init(NIEventLogger logger, NIContext context)
    {
        Dispose(); // to make sure we are not initialized
        if (InitWithContext(logger, context) == false)
            return false; 
        m_image = context.CreateNode(NodeType.Image) as ImageGenerator;
        if(m_image==null)
        {
            Dispose();
            return false;
        }
        return true;
    }

    /// @brief Release a previously initialize image node.
    /// 
    /// This method releases a previously initialized image node.
    /// @note - Since we use a singleton pattern, only one place should do a release, otherwise the
    /// result could become problematic for other objects
    /// @note - It is the responsibility of the whoever called @ref Init to do the release. 
    /// @note - The release should be called BEFORE releasing the context. If the context is invalid, the result is
    /// undefined.
    public override void Dispose()
    {
        if (m_image != null && m_context!=null)
            m_context.ReleaseNode(m_image); // we call this even if the context is invalid because it is a singleton which will release stuff
        m_image = null;
        base.Dispose();
    }


    // protected members

    /// An internal object using the the OpenNI basic image node (not to be confused with NIImage,
    /// m_image represent the image node received from the OpenNI dll.
    protected ImageGenerator m_image;
    /// @brief The singleton itself
    /// 
    /// NIImage uses a singleton pattern. There is just ONE NIImage object which is used by all.
    protected static NIImage m_singleton = new NIImage();

    // private methods

    /// @brief private constructor
    /// 
    /// This is part of the singleton pattern, a protected constructor cannot be called externally (although
    /// it can be inherited for extensions. In which case the extender should also replace the singleton!
    private NIImage()
    {
        m_image = null;
    }
}
