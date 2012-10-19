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

/// @brief This class implements a factory which allows us to receive a user pose based gesture and 
/// assign it to a hand.
/// @ingroup OpenNIGestureTrackers
public class NIUserPoseGestureFactory : NIGestureFactory
{
    /// this is the time between the first detection of the pose gesture and the
    /// time it is considered to have "clicked". This is used to make timed poses
    /// gestures. A value of 0 (or smaller) ignores the timing element.
    public float m_timeToHoldPose;

    public string m_poseName; ///< @brief The name of the pose to track

    /// @brief The OpenNI settings manager context which holds the relevant OpenNI nodes.
    public OpenNISettingsManager m_Context;

    /// returns a unique name for the gesture type.
    /// @note this is what is used to identify the factory
    /// @return the unique name.
    public override string GetGestureType()
    {
        return m_poseName+" pose gesture";
    }

    /// this creates the correct object implementation of the tracker
    /// @return the tracker object. 
    protected override NIGestureTracker GetNewTrackerObject()
    {
        if (m_Context == null)
        {
            m_Context = FindObjectOfType(typeof(OpenNISettingsManager)) as OpenNISettingsManager;
        }
        NIUserPoseDetector gestureTracker = new NIUserPoseDetector(m_timeToHoldPose,m_poseName,m_Context);
        return gestureTracker;
    }
}
