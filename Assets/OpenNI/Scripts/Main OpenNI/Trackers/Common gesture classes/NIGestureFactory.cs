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

/// @brief base class for gesture factories.
/// 
/// This class is a base class for gesture factories. Its purpose is to
/// create an object which can be added to the GestureManager and create gesture detection objects
/// on the fly.
/// @ingroup OpenNIGestureTrackers
public abstract class NIGestureFactory : MonoBehaviour
{
    /// This method is called by the GestureManager to create the the tracker
    /// @note it it the responsibility of the caller to later call @ref ReleaseTracker on this object
    /// @param hand the hand tracker to work with
    /// @return The relevant tracker class @note INIGestureTracker is NOT mono-behavior.
    public virtual NIGestureTracker GetGestureTracker(NIPointTracker hand)
    {
        NIGestureTracker obj = GetNewTrackerObject();
        if (obj == null)
            return null; // we failed.
        if (obj.Init(hand) == false)
        {
            return null; // we failed to initialize
        }
        m_trackersList.Add(obj);
        return obj;
    }


    /// returns a unique name for the gesture type.
    /// @note this is what is used to identify the factory
    /// @return the unique name.
    public abstract string GetGestureType();

    /// method to release a previous received tracker.
    /// @note it it the responsibility of the caller of @ref GetGestureTracker to call this method.
    /// 
    /// @param tracker the tracker to release.
    public void ReleaseTracker(NIGestureTracker tracker)
    {
        for (int i = 0; i < m_trackersList.Count; i++)
        {
            if(m_trackersList[i]!=tracker)
                continue; // not the one
            // now we have in 'i' the relevant tracker, remove it.
            for (int j = i + 1; j < m_trackersList.Count; j++)
                m_trackersList[j - 1] = m_trackersList[j];
            m_trackersList.RemoveAt(m_trackersList.Count - 1);
            return;
        }
    }

    /// mono-behavior update - responsible to call UpdateFrame on all objects
    public void Update()
    {
        foreach (NIGestureTracker tracker in m_trackersList)
        {
            tracker.UpdateFrame();
        }
    }

    /// mono-behavior awake, initialize the trackers list
    public void Awake()
    {
        m_trackersList = new List<NIGestureTracker>();
    }

    /// this creates the correct object implementation of the tracker
    /// @return the tracker object. 
    protected abstract NIGestureTracker GetNewTrackerObject();

    /// the list of gesture objects which were created.
    protected List<NIGestureTracker> m_trackersList;
}
