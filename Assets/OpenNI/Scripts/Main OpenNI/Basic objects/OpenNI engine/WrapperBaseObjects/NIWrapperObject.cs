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
abstract public class NIWrapperObject
{
    // public accessors
    /// Abstract property which returns true if the wrapped object is ready for use and false otherwise.
    public abstract bool Valid 
    {
        get;
    }

    /// Accessor to @ref m_Logger
    public NIEventLogger EventLogger
    {
        get { return m_Logger;  }
    }

    // public methods

    /// @brief Initialize the logger
    /// 
    /// This method is responsible for initialization of the logger. 
    /// @note It is assumed that all inheriting objects will have an "Init" method which will call this method.
    /// @param logger the logger object we will enter logs into
    /// @return true on success, false on failure. 
    protected bool InitLogger(NIEventLogger logger)
    {
        if(logger!=null)
            logger.Log("In " + this.GetType() + ":Init(" + logger + ")", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects, NIEventLogger.VerboseLevel.Verbose);
        m_Logger = logger;
        return true;
	}

    /// @brief Releases all internal data
    /// 
    /// This will make sure the relevant internal data is released
    public virtual void Dispose()
    {
        Log("Disposing of " + GetType(), NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects, NIEventLogger.VerboseLevel.Verbose);
    }

    /// @brief safe calling to the logger
    /// 
    /// This call the logger in a safe manner checking if it is null first.
    /// @param str the string we want to log
    /// @param category the category of the log
    /// @param source the source of the log
    /// @param logLevel what level to show and how to show it
    /// @note See @ref NIEventLogger.Log
    public void Log(string str, NIEventLogger.Categories category, NIEventLogger.Sources source,NIEventLogger.VerboseLevel logLevel)
    {
        if (m_Logger != null)
            m_Logger.Log(str, category,source,logLevel);
    }
    
    /// A logger used for internal logging
    protected NIEventLogger m_Logger;
}
