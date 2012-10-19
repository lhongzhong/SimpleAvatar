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

/// @brief This class implements the gesture tracker for the exit pose gesture (a gesture in
/// which the user crosses his hands in front of himself
/// @note This is a very naive implementation aimed at showing how to do such things. It is @b NOT
/// a good implementation but only aims at showing a simple solution. It is the responsibility of
/// the developers to optimize the gestures to their game.
/// 
/// 
/// @note currently we define the exit gesture as follows:
/// 1. the angle of the right hand (compared to the elbow) is around 45 degrees toward the left
/// 2. the angle of the left hand (compared to the elbow) is around 45 degrees toward the right
/// 3. the right hand is left of the left hand (by at least 25% of the arm length)
/// 4. both hands are above the opposite elbows (by at least 25% of the arm length)
/// @ingroup OpenNITrackerSamples
public class  NIExitPoseDetector : NIGestureTracker
{
    
    /// Release the gesture
    public override void ReleaseGesture()
    {
        m_pointTracker = null;
    }

    /// base constructor
    /// @param timeToHoldPose the time the user is required to hold the pose.
    /// @param maxMoveSpeed the maximum speed (in mm/sec) allowed for each of the relevant joints.
    /// @param angleTolerance  the hands are supposed to be at about 45 degrees in each direction. 
    /// This is the allowed tolerance in degrees (i.e. a tolerance of 10 means everything from 35 
    /// degrees to 55 degrees is ok
    /// in each axis. @note for the elbow displacement in the y axis this is the MINIMUM we have to be larger
    /// @param timeToSavePoints the time we use to average points
    public NIExitPoseDetector(float timeToHoldPose, float maxMoveSpeed, float angleTolerance, float timeToSavePoints)
    {
        m_maxMoveSpeed=maxMoveSpeed;
        m_timeToHoldPose=timeToHoldPose;
        m_timeToSavePoints = timeToSavePoints;
        m_angleTolerance = angleTolerance;
        m_holdingPose=false;
        m_timeDetectedPose=0;
        m_firedEvent=false;
        m_pointsRightHand = new NITimedPointSpeedListUtility(timeToSavePoints);
        m_pointsLeftHand = new NITimedPointSpeedListUtility(timeToSavePoints);
        m_pointsRightElbow = new NITimedPointSpeedListUtility(timeToSavePoints);
        m_pointsLeftElbow = new NITimedPointSpeedListUtility(timeToSavePoints);
    }

    /// This is true if the gesture is in the middle of doing (i.e. it has detected but not gone out of the gesture).
    /// for our purposes this means the steady event has occurred and the unsteady has not occurred yet
    /// @return a value between 0 and 1. 0 means no pose, 1 means the pose has been detected and held 
    /// for a while. a value in the middle means the pose has been detected and has been held this
    /// portion of the time required to fire the trigger (@ref m_timeToHoldPose).
    public override float GestureInProgress()
    {
        if (m_holdingPose == false)
            return 0.0f;
        float diffTime = Time.time - m_timeDetectedPose;
        if (diffTime >= m_timeToHoldPose || m_timeToHoldPose <= 0)
            return 1.0f;
        return diffTime / m_timeToHoldPose;
    }

    /// used for updating every frame
    /// @note While we are still steady (i.e. we haven't gotten a "not steady" event) we update
    /// the time and frame every frame!
    public override void UpdateFrame()
    {
        if (FillPoints() == false)
            return;
        int numPoints;
        bool foundPos = TestExitPose(out numPoints);
        if (numPoints < 1)
            return; // we don't have enough points to make a decision

        if (foundPos == false)
        {
            m_holdingPose = false;
            return;
        }
        // now we know we have found a pose.

        // first time of the pose since last we didn't have a pose
        if (m_holdingPose==false)
        {
            m_holdingPose = true;
            m_timeDetectedPose = Time.time;
            m_firedEvent = false;
        }


        float diffTime = Time.time - m_timeDetectedPose;
        if (diffTime >= m_timeToHoldPose || m_timeToHoldPose <= 0)
        {
            InternalFireDetectEvent();
        }
    }
    
    // protected methods

    /// this method tries to fill a new point on each of the relevant joints.
    /// It returns true if it succeed and false otherwise
    /// @note it will fail if even one of the points has a low confidence!
    /// @return true on success, false on failure.
    protected bool FillPoints()
    {
        // first we find a reference to the skeleton capability
        NISkeletonTracker hand = m_pointTracker as NISkeletonTracker;
        if (hand == null)
            return false; // no hand to track
        NISelectedPlayer player = hand.GetTrackedPlayer();
        if (player == null || player.Valid == false || player.Tracking == false)
            return false; // no player to work with...

        // We need to figure out if we have a good confidence on all joints

        SkeletonJointPosition rightHand;
        SkeletonJointPosition leftHand;
        SkeletonJointPosition rightElbow;
        SkeletonJointPosition leftElbow;
        if(player.GetSkeletonJointPosition(SkeletonJoint.RightHand,out rightHand)==false || rightHand.Confidence<=0.5f)
            return false;
        if(player.GetSkeletonJointPosition(SkeletonJoint.LeftHand,out leftHand)==false || leftHand.Confidence<=0.5f)
            return false;
        if(player.GetSkeletonJointPosition(SkeletonJoint.RightElbow,out rightElbow)==false || rightElbow.Confidence<=0.5f)
            return false;
        if(player.GetSkeletonJointPosition(SkeletonJoint.LeftElbow,out leftElbow)==false || leftElbow.Confidence<=0.5f)
            return false;
        Vector3 pos = NIConvertCoordinates.ConvertPos(rightHand.Position);
        m_pointsRightHand.AddPoint(ref pos);
        pos = NIConvertCoordinates.ConvertPos(leftHand.Position);
        m_pointsLeftHand.AddPoint(ref pos);
        pos = NIConvertCoordinates.ConvertPos(rightElbow.Position);
        m_pointsRightElbow.AddPoint(ref pos);
        pos = NIConvertCoordinates.ConvertPos(leftElbow.Position);
        m_pointsLeftElbow.AddPoint(ref pos);
        return true;
    }

    /// this method tests if we are relatively steady (i.e. our speed is low).
    /// In order to be steady, all relevant joints must move very slowly.
    /// @param numPoints the number of points tested
    /// @return true if we are steady and false otherwise
    /// @note we assume the number of points is the same for all joints
    protected virtual bool IsSteady(out int numPoints)
    {
        Vector3 curSpeed = m_pointsRightHand.GetAvgSpeed(m_timeToHoldPose, out numPoints);
        if (curSpeed.magnitude > m_maxMoveSpeed)
            return false; // we are moving
        if (numPoints < 1)
            return false; // we assume the number of points is the same for all joints (although speed would have 1 more point than positions)
        curSpeed = m_pointsLeftHand.GetAvgSpeed(m_timeToHoldPose, out numPoints);
        if (curSpeed.magnitude > m_maxMoveSpeed)
            return false; // we assume the number of points is the same for all joints (although speed would have 1 more point than positions)
        curSpeed = m_pointsRightElbow.GetAvgSpeed(m_timeToHoldPose, out numPoints);
        if (curSpeed.magnitude > m_maxMoveSpeed)
            return false; // we assume the number of points is the same for all joints (although speed would have 1 more point than positions)
        curSpeed = m_pointsLeftElbow.GetAvgSpeed(m_timeToHoldPose, out numPoints);
        if (curSpeed.magnitude > m_maxMoveSpeed)
            return false; // we assume the number of points is the same for all joints (although speed would have 1 more point than positions)
        return true;
    }

    /// this method tests the current point to figure out if we are in an exit pose
    /// @param numPoints the number of points tested
    /// @return true if we found the exit pose
    /// @note we assume the number of points is the same for all joints
    protected bool TestExitPose(out int numPoints)
    {
        if (IsSteady(out numPoints))
            return false;
        
        Vector3 rightHandPos = m_pointsRightHand.GetAvgPos(m_timeToHoldPose, out numPoints);
        rightHandPos.z = 0; 
        Vector3 rightElbowPos = m_pointsRightElbow.GetAvgPos(m_timeToHoldPose, out numPoints);
        rightElbowPos.z = 0;
        Vector3 rightDir = rightHandPos - rightElbowPos;
        // we check the angle compared to the up vector. since the right hand moves left
        // we check from the direction to up which on a perfect "X" gesture would be 45 degrees
        float angle1 = Vector3.Angle(rightDir.normalized, Vector3.up);
        if (Mathf.Abs(angle1 - 45.0f) > m_angleTolerance)
            return false; 
        
        Vector3 leftHandPos = m_pointsLeftHand.GetAvgPos(m_timeToHoldPose, out numPoints);
        leftHandPos.z = 0;
        Vector3 LeftElbowPos = m_pointsLeftElbow.GetAvgPos(m_timeToHoldPose, out numPoints);
        LeftElbowPos.z = 0;
        Vector3 leftDir = rightHandPos - rightElbowPos;
        // since the left angle goes to the other direction we switch between the direction and the up
        // direction to still get a positive 45 degrees.
        float angle2 = Vector3.Angle(Vector3.up, leftDir.normalized);
        if (Mathf.Abs(angle2 - 45.0f) > m_angleTolerance)
            return false; 

        // now we know the angles of the hands are correct and we need to check if they intersect
        // properly.
        float maxOffset = Mathf.Min(rightDir.magnitude, leftDir.magnitude); // the longest of the arms
        if(rightHandPos.x>leftHandPos.x-maxOffset/4)
            return false; // the right hand should be to the left of the left hand by at least 25% of maxOffset
        if (rightHandPos.y < LeftElbowPos.y + maxOffset / 4)
            return false; // the right hand should be above the left elbow by at least 25% of maxOffset
        if (leftHandPos.y < rightElbowPos.y + maxOffset / 4)
            return false; // the left hand should be above the right elbow by at least 25% of maxOffset
        return true;

    }


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

    /// the maximum speed (in mm/sec) allowed for each of the relevant joints.
    protected float m_maxMoveSpeed;

    /// the hands are supposed to be at about 45 degrees in each direction. This is the allowed 
    /// tolerance in degrees (i.e. a tolerance of 10 means everything from 35 degrees to 55 degrees is ok
    protected float m_angleTolerance;

    /// the time we need to hold the pose
    protected float m_timeToHoldPose;

    /// the time we need to hold the points
    protected float m_timeToSavePoints;

    /// if this is true then we found the pose
    protected bool m_holdingPose;

    /// the time we first detected the pose (and not lost it since)
    protected float m_timeDetectedPose;

    /// this holds true if we already fired the event
    protected bool m_firedEvent;

    /// this holds the points we are tracking on the right hand
    protected NITimedPointSpeedListUtility m_pointsRightHand;
    /// this holds the points we are tracking on the left hand
    protected NITimedPointSpeedListUtility m_pointsLeftHand;
    /// this holds the points we are tracking on the right elbow
    protected NITimedPointSpeedListUtility m_pointsRightElbow;
    /// this holds the points we are tracking on the left elbow
    protected NITimedPointSpeedListUtility m_pointsLeftElbow;

    
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
