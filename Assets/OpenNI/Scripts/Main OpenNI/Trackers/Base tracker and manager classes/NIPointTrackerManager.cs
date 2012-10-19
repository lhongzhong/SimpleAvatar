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
using System.Collections.Generic;
using OpenNI;

/// @brief class to Manage users and skeletons
/// 
/// This class is responsible for managing the various users and mapping them to controlled skeletons.
/// @ingroup OpenNIPointTrackers
public class NIPointTrackerManager : MonoBehaviour
{

    /// the hand trackers we are using. Note m_trackers[i] refers to axis i (starting from 0!).
    public NIPointTracker[] m_trackers;

    /// the context we will track on.
    public OpenNISettingsManager m_context;



    /// mono-behavior awake (initialization)
    public void Awake()
    {
        if (m_context == null)
            m_context = FindObjectOfType(typeof(OpenNISettingsManager)) as OpenNISettingsManager;
        if (m_trackers.Length <= 0)
            return; // nothing to do, there are no trackers
        m_references=new int[m_trackers.Length];
        for(int i=0; i<m_trackers.Length; i++)
            m_references[i]=0;
    }

 


    /// Method to get the relevant hand tracker for a specific axis
    /// 
    /// @param trackerName the name of the tracker to use.
    /// @return the tracker (null if not found).
    public NIPointTracker GetTracker(string trackerName)
    {
        if (trackerName==null)
        {
            m_context.Log("tried to get tracker with a null name!", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.Trackers, NIEventLogger.VerboseLevel.Errors);
            return null; // illegal index.
        }
        for (int i = 0; i < m_trackers.Length; i++)
        {
            if(m_trackers[i]==null)
                continue; // an illegal tracker
            if (trackerName.CompareTo(m_trackers[i].GetTrackerType()) == 0)
            {
                // we found the tracker!
                if (m_references[i] == 0)
                {
                    // first time we access it, we need to init it...
                    if (m_trackers[i].InitTracking(m_context) == false)
                    {
                        m_context.Log("Failed to initialize axis " + trackerName, NIEventLogger.Categories.Initialization, NIEventLogger.Sources.Trackers, NIEventLogger.VerboseLevel.Errors);
                        return null;
                    }
                }
                m_references[i]++;
                return m_trackers[i];

            }
        }
        return null; // we didn't find it!
    }

    /// Method to release a previously requested tracker for a specific axis
    /// 
    /// @param trackerName the name of the tracker to use.
    public void ReleaseTracker(string trackerName)
    {
        if (trackerName == null)
        {
            m_context.Log("tried to get tracker with a null name!", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.Trackers, NIEventLogger.VerboseLevel.Errors);
            return; // illegal index.
        }
        for (int i = 0; i < m_trackers.Length; i++)
        {
            if (m_trackers[i] == null)
                continue; // an illegal tracker
            if (trackerName.CompareTo(m_trackers[i].GetTrackerType()) == 0)
            {
                // we found the tracker!
                if (m_references[i] <= 0)
                    return; // already released.
                m_references[i]--;
                if (m_references[i] == 0)
                    m_trackers[i].StopTracking();                
                return;

            }
        }
        // we didn't find it!
    }

    /// @brief makes sure we release everything on quit
    /// 
    /// This will make sure everything the context is released when quitting (important for playing and
    /// stopping in the editor).
    /// @note - This is based on the MonoBehavior.OnAppliactionQuit.
    public void OnApplicationQuit()
    {
        ReleaseAll();
    }

    /// @brief makes sure we release everything on destroy
    /// 
    /// This will make sure everything released when the object is destroyed
    /// (important for playing and stopping in the editor).
    /// @note - This is based on the MonoBehavior.OnDestroy.
    public void OnDestroy()
    {
        ReleaseAll();
    }

    /// protected members

    /// holds the number of references for each control.
    protected int[] m_references;


    // protected methods

    /// this method releases every tracker (useful for the end of the game...)
    protected void ReleaseAll()
    {
        for (int i = 0; i < m_trackers.Length; i++)
        {
            if (m_references[i] != 0)
                m_trackers[i].StopTracking();
            m_references[i] = 0;
        }
    }
}
