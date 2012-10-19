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

/// @brief implementation of a timed steady gesture over a skeleton joint.
/// 
/// @ingroup OpenNIGestureTrackers
public class NISteadySkeletonHandDetector : NIGestureTracker
{
    /// This holds the maximum threshold to be considered steady. A steady result occurs if the square of the standard
    /// deviation on the points along @ref m_steadyTestTime have a magnitude square of their standard deviation smaller than this value
    /// @note this MUST be smaller than @ref m_unsteadyStdSqrThreshold or unpredictable results may occur
    public float m_steadyStdSqrThreshold;
    /// This holds the minimum threshold to be considered not steady. A not steady result occurs if the square of the standard
    /// deviation on the points along @ref m_steadyTestTime have a magnitude square of their standard deviation larger than this value
    /// @note this MUST be larger than @ref m_steadyStdSqrThreshold or unpredictable results may occur
    public float m_unsteadyStdSqrThreshold;

    /// this holds the maximum we allow to move from the first steady (in mm) before it is considered
    /// unsteady
    public float m_maxMoveFromFirstSteady;

    /// Release the gesture
    public override void ReleaseGesture()
    {
        m_pointTracker = null;
    }

    /// base constructor
    /// @param timeToClick this is the time one needs to remain steady to for the event to fire 
    /// (and the gesture recognized).
    /// @param timeToReset the time AFTER firing the event where the steady resets (i.e. as if
    /// the hand moved). 
    /// @param steadyTestTime the time along which we find points for steady (sets @ref m_steadyTestTime).
    /// @note this means that if one leaves his hand steady continuously then the event will fire
    /// every timeToClick+timeToReset seconds starting from timeToClick seconds after the initial
    /// steady.
    public NISteadySkeletonHandDetector(float timeToClick, float timeToReset, float steadyTestTime)
    {
        m_currentSteady = false;
        m_firstSteady = float.MaxValue;
        m_firedSteadyEvent = false;
        m_timeToClick = timeToClick;
        m_timeToReset = timeToClick + timeToReset;
        m_steadyTestTime = steadyTestTime;
        m_points = new NITimedPointListUtility(steadyTestTime);
    }

    /// This is true if the gesture is in the middle of doing (i.e. it has detected but not gone out of the gesture).
    /// for our purposes this means the steady event has occurred and the unsteady has not occurred yet
    /// @return a value between 0 and 1. 0 means no gesture, 1 means the gesture has been detected and 
    /// held for a while. A value in the middle means the gesture has been detected and has been held
    /// this portion of the time required to fire the trigger (@ref m_timeToClick).

    public override float GestureInProgress()
    {
        if (m_currentSteady==false)
            return 0.0f;
        float diffTime = Time.time - m_firstSteady;
        if (diffTime >= m_timeToClick || m_timeToClick<=0)
            return 1.0f;
        return diffTime / m_timeToClick;
    }

    /// used for updating every frame
    /// @note While we are still steady (i.e. we haven't gotten a "not steady" event) we update
    /// the time and frame every frame!
    public override void UpdateFrame()
    {
        float confidence;
        NISkeletonTracker hand = m_pointTracker as NISkeletonTracker;
        Vector3 newPoint=hand.GetPosWithConfidence(out confidence);
        if (confidence <= 0.5)
        {
            m_currentSteady = false;
            return; // we simply ignore the point...
        }
        m_points.AddPoint(ref newPoint);
        int numPoints;
        Vector3 stdSqr = m_points.GetStdDeviationSqr(m_steadyTestTime, out numPoints);
        if (numPoints < 1)
            return; // we don't have the points to decide what to do, so we'll just wait...
        if (stdSqr.magnitude < m_steadyStdSqrThreshold && m_currentSteady==false)
        {
            // we found a NEW steady result
            m_currentSteady = true;
            m_firstSteady = Time.time;
            m_firedSteadyEvent = false;
            m_firstSteadyPoint = newPoint;
        }
        if (m_currentSteady && stdSqr.magnitude > m_unsteadyStdSqrThreshold)
        {
            m_currentSteady = false;
            return;
        }
        if (m_currentSteady)
        {
            Vector3 pointChange = newPoint - m_firstSteadyPoint;
            if (pointChange.magnitude > m_maxMoveFromFirstSteady)
            {
                m_currentSteady = false;
                return;
            }
            float diffTime = Time.time - m_firstSteady;
            if (diffTime >= m_timeToClick || m_timeToClick <= 0)
            {
                InternalFireDetectEvent();
            }
            if (diffTime > m_timeToReset)
            {
                m_currentSteady = false;
            }
        }
    }
    
    // protected methods

    /// Gesture initialization
    /// 
    /// This method is responsible for initializing the gesture to work with a specific hand tracker
    /// @param hand the hand tracker to work with
    /// @return true on success, false on failure (e.g. if the hand tracker does not work with the gesture).
    protected override bool InternalInit(NIPointTracker hand)
    {
        NISkeletonTracker curHand = hand as NISkeletonTracker;
        if (curHand == null)
            return false;
        return true;
    }

    // protected members
    protected bool m_currentSteady;            ///< holds true if we are currently steady and false otherwise (we detected not steady)
    protected float m_firstSteady;             ///< holds the time we discovered the steady for the first time (becomes irrelevant every time a not steady is detected).
    protected bool m_firedSteadyEvent;          ///< holds true if we fired the steady event for the latest steady and false otherwise.
    /// this is the time between the first detection of a steady gesture and the
    /// time it is considered to have "clicked". This is used to make timed steady
    /// gestures. A value of 0 (or smaller) ignores the timing element.
    protected float m_timeToClick;
    /// this is the time after the initial steady when we reset the steady (i.e. as if we got
    /// a not steady event). This is to simulate a repeat fire button.
    protected float m_timeToReset;

    /// this is the time (in seconds) we check over to see a steady/not steady result.
    protected float m_steadyTestTime;

    /// this holds the points we are tracking and as a result is used to detect events.
    protected NITimedPointListUtility m_points;

    /// this will hold the position where we first identified the steady. If we go too far
    /// from this, it is as if we got an unsteady event.
    protected Vector3 m_firstSteadyPoint;
   
    
    /// this marks the result as clicked by updating the time and frame and the first
    /// time after the last change it also fires the gesture event.
    protected virtual void InternalFireDetectEvent()
    {

        m_timeDetected = Time.time;
        m_frameDetected = Time.frameCount;
        if (m_firedSteadyEvent == false)
        {
            DetectGesture();
            m_firedSteadyEvent = true;
        }
    }
}
