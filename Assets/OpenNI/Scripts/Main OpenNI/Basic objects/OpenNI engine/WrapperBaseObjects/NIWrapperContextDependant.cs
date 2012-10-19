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


/// @brief A base object for a wrapper to OpenNI objects
///
/// This class is used as a base object to various wrappers of the OpenNI objects.
/// @ingroup OpenNIBasicObjects
abstract public class NIWrapperContextDependant : NIWrapperObject
{
    // public accessors

    public override bool Valid
    {
        get { return m_context != null && m_context.Valid; }
    }


    /// Accessor to @ref m_context
    public NIContext currentContext  
    {
        get { return m_context; }
    }

     // public methods

    /// @brief Initializer of the logger and context 
    /// 
    /// This method is responsible for initializing the context and logger (it should be used
    /// instead of the InitLogger method as it calls it internally!
    /// @note it will always return false doing nothing if the object is already valid!
    /// @note the context must be valid for this to work! If the context is invalid it will also invalidate this object
    /// @param logger the logger object we will enter logs into
    /// @param context the context this relates to
    /// @return true on success, false on failure. 
    public bool InitWithContext(NIEventLogger logger, NIContext context)
    {
        if(Valid)
            return false;
        if (context.Valid == false)
        {
            Dispose();
            return false;
        }
        if(InitLogger(logger)==false)
        {
            Dispose();
            return false;
        }
        Log("initializing with context", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects,NIEventLogger.VerboseLevel.Verbose);
        m_context = context;
        return true;
	}

    public override void Dispose()
    {
        base.Dispose();
        m_context = null; // we do not need the link anymore
    }

    /// An internal object to reference the context to use.
    protected NIContext m_context;

}
