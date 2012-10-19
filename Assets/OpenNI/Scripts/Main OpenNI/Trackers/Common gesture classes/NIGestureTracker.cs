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
using System;

/// @brief base class for all gestures.
/// 
/// This class defines the behavior and interface of gesture tracking. 
/// @ingroup OpenNIGestureTrackers
public abstract class NIGestureTracker 
{
    /// Method to return the time when we last detected the gesture.
    /// @return the time (in based on Time.time) when we last detected the gesture.
    public virtual float GetLastGestureTime()
    {
        return m_timeDetected;
    }

    /// Method to return the frame when we last detected the gesture
    /// @return the frame count (based on Time.framecount) when we last detected the gesture
    public virtual int GetLastGestureFrame()
    {
        return m_frameDetected;
    }

    /// This is returns a value between 0 and 1 representing how we are in the gesture progress.
    /// a value of 0 means no gesture, a value of 1 means a gesture has been detected (and continuing)
    /// a value in between means the gesture is in the progress of being recognized (mainly relevant
    /// for timed gestures)
    /// @return a value between 0 and 1. 0 means no gesture, 1 means the gesture has been detected and 
    /// held for a while. A value in the middle means the gesture has been detected and has been held
    /// this portion of the time required to fire the trigger (for timed triggers).

    public abstract float GestureInProgress();


    /// Gesture initialization
    /// 
    /// This method is responsible for initializing the gesture to work with a specific hand tracker
    /// @param hand the hand tracker to work with
    /// @return true on success, false on failure (e.g. if the hand tracker does not work with the gesture).
    public virtual bool Init(NIPointTracker hand)
    {
        if (hand.Valid == false)
            return false;
        if (InternalInit(hand) == false)
            return false;
        m_pointTracker = hand;
        return true;
    }

     /// Release the gesture
    public abstract void ReleaseGesture();

    /// used for updating every frame
    public virtual void UpdateFrame()
    {
        // do nothing, only if something should be done when updating should this be overridden.
    }

    // callback handling

    /// delegate defined in order to register for events
    /// @param hand the hand tracker from which the event was created.
    public delegate void GestureEventHandler(NIPointTracker hand);

    /// anyone who wants to register to the "detect new gesture" event should do so here...
    public event GestureEventHandler m_gestureEventHandler;

    // protected methods.

    /// used to perform all internal initialization on the gesture.
    /// @note it can assume the validity of the hand is checked before calling and that after a successful
    /// return (true) m_handTracker is initialized
    /// @param hand the hand tracker to work with
    /// @return true on success, false on failure (e.g. if the hand tracker does not work with the gesture).
    protected abstract bool InternalInit(NIPointTracker hand);

    /// this basically activates the events for all those registered
    protected void DetectGesture()
    {
        if(m_gestureEventHandler!=null)
            m_gestureEventHandler(m_pointTracker);
    }

    /// constructor. @note it is protected to make sure only child classes can be created.
    protected NIGestureTracker()
    {
        m_timeDetected = -100.0f;
        m_frameDetected = -100;
        m_pointTracker = null;
    }

    // protected members
    protected float m_timeDetected; ///< holds the last time (in time.time) the gesture was detected. -1 if it was never detected
    protected int m_frameDetected; ///< holds the last frame (in time.framecount) the gesture was detected. -1 if it was never detected
    protected NIPointTracker m_pointTracker; ///< holds the relevant hand tracker to release with.    
}


