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

/// @brief This class implements a factory which allows us to receive an exit pose gesture 
/// and assign it to a hand.
/// @ingroup OpenNITrackerSamples
public class NIExitPoseGestureFactory : NIGestureFactory
{
    /// this is the time between the first detection of an exit pose gesture and the
    /// time it is considered to have "clicked". This is used to make timed exit poses
    /// gestures. A value of 0 (or smaller) ignores the timing element.
    public float m_timeToHoldPose;
    /// @param maxMoveSpeed the maximum speed (in mm/sec) allowed for each of the relevant joints.
    public float m_maxMoveSpeed;
    /// @param timeToSavePoints the time we use to average points
    public float m_timeToSavePoints;

    /// the hands are supposed to be at about 45 degrees in each direction. 
    /// This is the allowed tolerance in degrees (i.e. a tolerance of 10 means everything from 35 
    /// degrees to 55 degrees is ok
    public float m_angleTolerance;
    /// returns a unique name for the gesture type.
    /// @note this is what is used to identify the factory
    /// @return the unique name.
    public override string GetGestureType()
    {
        return "ExitPoseGesture";
    }

    /// this creates the correct object implementation of the tracker
    /// @return the tracker object. 
    protected override NIGestureTracker GetNewTrackerObject()
    {
        NIExitPoseDetector gestureTracker = new NIExitPoseDetector(m_timeToHoldPose,m_maxMoveSpeed,m_angleTolerance,m_timeToSavePoints);
        return gestureTracker;
    }
}
