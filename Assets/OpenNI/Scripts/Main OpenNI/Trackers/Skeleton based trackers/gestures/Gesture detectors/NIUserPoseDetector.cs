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

/// @brief This class implements the gesture tracker for the user node based poses gesture 
/// 
/// This class implements a gesture tracker based on poses supported by the user generator node.
/// @ingroup OpenNIGestureTrackers
public class  NIUserPoseDetector : NIGestureTracker
{
    
    /// Release the gesture
    public override void ReleaseGesture()
    {
        if (m_context != null && m_context.UserSkeletonValid)
        {
            NISkeletonTracker tracker = m_pointTracker as NISkeletonTracker;
            if(tracker.Valid)
            {
                NISelectedPlayer player = tracker.GetTrackedPlayer();
                if (player != null && player.Valid && validRequestedPoseDetection)
                {
                    m_context.UserGenrator.ReleasePoseDetection(m_poseName, player.OpenNIUserID);
                }                
            }
        }
        m_context = null;
        m_poseName = "";
        m_pointTracker = null;
    }

    /// base constructor
    /// @param timeToHoldPose The time the user is required to hold the pose.
    /// @param poseName The name of the pose to detect
    /// @param context The OpenNISettingsManager to use to get the basic OpenNI nodes
    public NIUserPoseDetector(float timeToHoldPose, string poseName, OpenNISettingsManager context)
    {
        m_timeToHoldPose = timeToHoldPose;
        if (m_timeToHoldPose < 0)
            m_timeToHoldPose = 0;
        m_firedEvent=false;
        m_context = context;
        m_poseName = poseName;
    }

    /// This is true if the gesture is in the middle of doing (i.e. it has detected but not gone out of the gesture).
    /// for our purposes this means the steady event has occurred and the unsteady has not occurred yet
    /// @return a value between 0 and 1. 0 means no pose, 1 means the pose has been detected and held 
    /// for a while. a value in the middle means the pose has been detected and has been held this
    /// portion of the time required to fire the trigger (@ref m_timeToHoldPose).
    public override float GestureInProgress()
    {
        if(m_context==null || m_context.UserSkeletonValid==false)
            return 0.0f; // no data;
        NISkeletonTracker tracker = m_pointTracker as NISkeletonTracker;
        if(tracker.Valid == false)
            return 0.0f; // no one to track.
        NISelectedPlayer player = tracker.GetTrackedPlayer();
        if(player==null || player.Valid==false)
            return 0.0f;
        NIUserAndSkeleton.NIPoseDetectionStateStatus curStatus = new NIUserAndSkeleton.NIPoseDetectionStateStatus();
        if (m_context.UserGenrator.GetPoseStatus(player.OpenNIUserID, m_poseName, ref curStatus) == false)
            return 0.0f; // we do not have good pose information
        if (curStatus.m_eState != PoseDetectionState.InPose)
            return 0.0f;
        if (curStatus.m_timePoseHeld < 0)
            return 0.0f;
        if (curStatus.m_timePoseHeld >= m_timeToHoldPose)
            return 1.0f;
        return curStatus.m_timePoseHeld / m_timeToHoldPose;
    }

    /// used for updating every frame
    /// @note While we are still steady (i.e. we haven't gotten a "not steady" event) we update
    /// the time and frame every frame!
    public override void UpdateFrame()
    {
        if (m_context == null || m_context.UserSkeletonValid == false)
            return; // no data;
        NISkeletonTracker tracker = m_pointTracker as NISkeletonTracker;
        if (tracker.Valid == false)
            return; // no one to track.
        NISelectedPlayer player = tracker.GetTrackedPlayer();
        if (player == null || player.Valid == false)
            return;
        NIUserAndSkeleton.NIPoseDetectionStateStatus curStatus = new NIUserAndSkeleton.NIPoseDetectionStateStatus();
        if (m_context.UserGenrator.GetPoseStatus(player.OpenNIUserID, m_poseName, ref curStatus) == false)
            return; // we do not have good pose information
        if (curStatus.m_eState != PoseDetectionState.InPose)
            return;
        if (curStatus.m_timePoseHeld >= m_timeToHoldPose)
            InternalFireDetectEvent();
    }
    
    // protected methods
    protected override bool InternalInit(NIPointTracker hand)
    {
        NISkeletonTracker curHand = hand as NISkeletonTracker;
        if (curHand == null)
            return false;
        if(m_context==null || m_context.UserSkeletonValid==false)
            return false;
        NISelectedPlayer player = curHand.GetTrackedPlayer();
        if (player == null)
            return false; // no player
        player.m_userChangeEventHandler += PlayerUserChangeHandler;
        if (player.Valid)
        {
            validRequestedPoseDetection=m_context.UserGenrator.RequestPoseDetection(m_poseName,player.OpenNIUserID);
            return validRequestedPoseDetection;
        }
        validRequestedPoseDetection = false;
        return true;
    }
 
    // protected members

    /// the time we need to hold the pose
    protected float m_timeToHoldPose;

    protected string m_poseName; ///< @brief The name of the pose we use 

    protected OpenNISettingsManager m_context; ///< @brief the context of OpenNI

    /// this holds true if we already fired the event
    protected bool m_firedEvent;

    /// @brief This variable holds true when we have successfully requested pose detection on the
    /// user.
    protected bool validRequestedPoseDetection=false;

    /// @brief A callback method to handle changing of a user in the tracked player
    /// 
    /// This method is called as a callback whenever the tracked player changes the user it tracks.
    /// @param origUser The user object for the user @b before the change
    /// @param newUser The user object for the user @b after the change
    protected void PlayerUserChangeHandler(NIPlayerCandidateObject origUser, NIPlayerCandidateObject newUser)
    {
        if (m_context == null || m_context.UserSkeletonValid == false)
            return; // no context to work with
        if (origUser != null && validRequestedPoseDetection)
        {
            m_context.UserGenrator.ReleasePoseDetection(m_poseName, origUser.OpenNIUserID);
            validRequestedPoseDetection = false;
        }
        if (newUser != null)
        {
            validRequestedPoseDetection=m_context.UserGenrator.RequestPoseDetection(m_poseName, newUser.OpenNIUserID);
        }
    }

    
    /// this marks the result as clicked by updating the time and frame and the first
    /// time after the last change it also fires the gesture event.
    protected virtual void InternalFireDetectEvent()
    {

        m_timeDetected = Time.time;
        m_frameDetected = Time.frameCount;
        if (m_firedEvent == false)
        {
            DetectGesture();
            m_firedEvent = true;
        }
    }
}
