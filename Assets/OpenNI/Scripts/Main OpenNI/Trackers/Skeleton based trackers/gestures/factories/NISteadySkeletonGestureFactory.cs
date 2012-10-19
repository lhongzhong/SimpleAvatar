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

/// @brief Factory for @ref NISteadySkeletonHandDetector.
/// @ingroup OpenNIGestureTrackers
public class NISteadySkeletonGestureFactory : NIGestureFactory
{
    /// this is the time between the first detection of a steady gesture and the
    /// time it is considered to have "clicked". This is used to make timed steady
    /// gestures. A value of 0 (or smaller) ignores the timing element.
    public float m_timeToClick;
    /// this is the time after the "click" event when we reset the steady (i.e. as if we got
    /// a not steady event). This is to simulate a repeat fire button.
    /// @note this means that if one leaves his hand steady continuously then the event will fire
    /// every timeToClick+timeToReset seconds starting from timeToClick seconds after the initial
    /// steady.
    public float m_timeToReset;


    /// this is the time (in seconds) we check over to see a steady/not steady result.
    public float m_steadyTestTime=0.5f;

    /// This holds the maximum threshold to be considered steady. A steady result occurs if the square of the standard
    /// deviation on the points along @ref m_steadyTestTime have a magnitude square of their standard deviation smaller than this value
    /// @note this MUST be smaller than @ref m_unsteadyStdSqrThreshold or unpredictable results may occur
    public float m_steadyStdSqrThreshold=2;
    /// This holds the minimum threshold to be considered not steady. A not steady result occurs if the square of the standard
    /// deviation on the points along @ref m_steadyTestTime have a magnitude square of their standard deviation larger than this value
    /// @note this MUST be larger than @ref m_steadyStdSqrThreshold or unpredictable results may occur
    public float m_unsteadyStdSqrThreshold=8;

    /// this holds the maximum we allow to move from the first steady (in mm) before it is considered
    /// unsteady
    public float m_maxMoveFromFirstSteady=40;


    /// returns a unique name for the gesture type.
    /// @note this is what is used to identify the factory
    /// @return the unique name.
    public override string GetGestureType()
    {
        return "SteadySkeletonGesture";
    }

    /// this creates the correct object implementation of the tracker
    /// @return the tracker object. 
    protected override NIGestureTracker GetNewTrackerObject()
    {
        NISteadySkeletonHandDetector gestureTracker=new NISteadySkeletonHandDetector(m_timeToClick, m_timeToReset,m_steadyTestTime);
        gestureTracker.m_steadyStdSqrThreshold=m_steadyStdSqrThreshold;
        gestureTracker.m_unsteadyStdSqrThreshold=m_unsteadyStdSqrThreshold;
        gestureTracker.m_maxMoveFromFirstSteady = m_maxMoveFromFirstSteady;
        return gestureTracker;
    }
}
