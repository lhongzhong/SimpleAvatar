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
using System;


/// @brief An internal logger
///
/// This class is responsible logging events internal to OpenNi wrapper and filter them
/// @note it is mono-behavior so we can drag & drop it.
/// @ingroup OpenNIBasicObjects
public class NIEventLogger : MonoBehaviour
{
    /// @brief Defines legal categories
    public enum Categories
    {
        Initialization, ///< @brief Initialization related issues
        Callbacks,      ///< @brief Callback related issues
        Misc           ///< @brief everything else
    }

    /// @brief Used to control the categories we want to show 
    public bool[] m_categoriesToShow=null;

    /// @brief A setting for which verbose level to show
    /// 
    /// The logger will show logs of verbose levels higher or equal to the value of m_minLevelToShow
    public VerboseLevel m_minLevelToShow; 
    /// @brief Defines the sources of the messages
    public enum Sources
    {
        BaseObjects,  ///< @brief Relates to basic objects (context etc.)
        Image,        ///< @brief Image related issues
        Skeleton,     ///< @brief Skeleton and users related issues
        UserSelector, ///< @brief User selector related issues
        Hands,        ///< @brief Hand tracking and session manager related issues
        Input,        ///< @brief Input object related issues
        Trackers,     ///< @brief Tracker and gestures related issues
        GUI           ///< @brief GUI and controls related issues
    }


    /// used to control the sources we want to show 
    public bool[] m_sourcesToShow = new bool[Enum.GetNames(typeof(Sources)).Length];

    /// @brief Enum to describe the level of verbosness of the log.
    public enum VerboseLevel
    {
        Verbose = 0,        ///< Show everything
        Warning = 1,        ///< Show warnings and errors
        Errors = 2,         ///< Show only errors 
    }


    /// @brief Initializes the sources and categories array if relevant
    public void Awake()
    {
        InitCategoriesList();
        InitSourcesList();
        m_Initialized = true;

    }

    /// @brief makes sure we release everything on destroy
    /// 
    /// This will make sure the context is released when the object is destroyed
    /// (important for playing and stopping in the editor).
    /// @note - This is based on the MonoBehavior.OnDestroy.
    public void OnDestroy()
    {
        m_Initialized = false;
    }


    /// @brief Method to initialize the categories list
    ///     
    /// @return true if the categories list changed, false otherwise
    public bool InitCategoriesList()
    {
        if (m_categoriesToShow != null && m_categoriesToShow.Length == Enum.GetNames(typeof(NIEventLogger.Categories)).Length)
            return false; // it is ready.
        bool[] orig = null;
        if (m_categoriesToShow != null)
            orig = m_categoriesToShow;
        m_categoriesToShow = new bool[Enum.GetNames(typeof(NIEventLogger.Categories)).Length];
        for (int i = 0; i < orig.Length; i++)
        {
            m_categoriesToShow[i] = orig[i];
        }
        for (int i = orig.Length; i < m_categoriesToShow.Length; i++)
        {
            m_categoriesToShow[i] = true;
        }
        return true;
    }

    /// @brief Method to initialize the sources list
    /// 
    /// @return true if the sources list changed, false otherwise
    public bool InitSourcesList()
    {
        if (m_sourcesToShow != null && m_sourcesToShow.Length == Enum.GetNames(typeof(NIEventLogger.Sources)).Length)
            return false; // it is ready.
        bool[] orig = null;
        if (m_sourcesToShow != null)
            orig = m_sourcesToShow;
        m_sourcesToShow = new bool[Enum.GetNames(typeof(NIEventLogger.Sources)).Length];
        for (int i = 0; i < orig.Length; i++)
        {
            m_sourcesToShow[i] = orig[i];
        }
        for (int i = orig.Length; i < m_sourcesToShow.Length; i++)
        {
            m_sourcesToShow[i] = true;
        }
        return true;
    }

    /// does the logging.
    /// @param str the string to log
    /// @param category the category of the log
    /// @param source the source of the log
    /// @param logLevel what level to show and how to show it
    public virtual void Log(string str, Categories category, Sources source, VerboseLevel logLevel)
    {
        if (logLevel < m_minLevelToShow)
            return; // too low a level
        if (m_categoriesToShow[(int)category] && m_sourcesToShow[(int)source])
        {
            if (logLevel == VerboseLevel.Verbose)
                Debug.Log(str);
            else if (logLevel == VerboseLevel.Warning)
                Debug.LogWarning(str);
            else Debug.LogError(str);
        }
    }

    /// @brief Accessor to m_Initialized
    public bool Initialized
    {
        get { return m_Initialized; }
    }


    protected bool m_Initialized = false; ///< @brief internal member to check if initialized
}
