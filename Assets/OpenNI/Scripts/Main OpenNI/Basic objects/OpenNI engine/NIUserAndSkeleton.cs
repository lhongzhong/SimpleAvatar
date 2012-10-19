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
using System;
using OpenNI;
using System.Collections.Generic;
using UnityEngine;

/// @brief abstraction of the the user generator and skeleton
/// 
/// This class is responsible for abstracting the user generator and skeleton capabilities. 
/// It is responsible for all relevant initializations.
/// @ingroup OpenNIBasicObjects
public class NIUserAndSkeleton : NIWrapperContextDependant
{
    /// @brief A structure to get information on the pose status
    ///  
    /// This struct replaces PoseDetectionStateStatus struct provided by OpenNI by changing
    /// the time into the time the pose was held in Time.time units. A negative number means
    /// the pose is not held...
    public struct NIPoseDetectionStateStatus
    {
        public PoseDetectionState m_eState; ///< @brief The state of the user pose (i.e. in pose, out of pose).  
        public PoseDetectionStatus m_eStatus; ///< @brief The status of the user's pose, i.e. the progress error for getting into pose (XnPoseDetectionStatus, the same as received from the in progress callback
        public float m_timePoseHeld; ///< @brief The time (in Time.time units) the user has held the pose. A negative number means not in pose.

    }
    /// This gets the center of mass for a specific user
    /// @param niUserID the NI id of a user
    /// @return position of the center of mass of the user (undefined if none was found).
    public Vector3 GetUserCenterOfMass(int niUserID)
    {
        try
        {
            Point3D com = m_userGenerator.GetCoM(niUserID);
            if (com.Z <= 0)
                return Vector3.forward; // this means a positive 'z' which is illegal!
            return NIConvertCoordinates.ConvertPos(com);
        }
        catch 
        {
            return Vector3.forward;
        }
        

    }

    /// The skeleton (null if invalid)
    public SkeletonCapability Skeleton
    {
        get { return Valid ? m_userGenerator.SkeletonCapability : null; }
    }

    /// @brief Returns true if the user is tracked.
    ///  
    /// @param userID The user we want to check if tracked.
    /// @return True if the user is tracked, false otherwise.
    public bool IsTracking(int userID)
    {
        if (Skeleton == null)
            return false;
        return Skeleton.IsTracking(userID);
    }

    /// @brief Returns true if the user is calibrated.
    ///  
    /// @param userID The user we want to check if calibrated.
    /// @return True if the user is calibrated, false otherwise.
    public bool IsCalibrated(int userID)
    {
        if (Skeleton == null)
            return false;
        return Skeleton.IsCalibrated(userID);
    }

    /// @brief Returns true if the user is calibrating.
    ///  
    /// @param userID The user we want to check if calibrating.
    /// @return True if the user is calibrating, false otherwise.
    public bool IsCalibrating(int userID)
    {
        if (Skeleton == null)
            return false;
        return Skeleton.IsCalibrating(userID);
    }

    /// Holds true if the user and skeleton is valid
    public override bool Valid
    {
        get { return base.Valid && m_userGenerator!=null; }
    }

    /// @brief Gets a list of available user ids
    /// 
    /// @return an array of user ids.
    public int[] GetUserIds()
    {
        if (Valid==false)
            return null;
        return m_userGenerator.GetUsers();
    }

    /// Accessor to @ref m_userGenerator
    public UserGenerator UserNode
    {
        get { return m_userGenerator; }
    }

    /// Allows us to change the smoothing factor of the skeleton (to get smoother behavior).
    public float SkeletonSmoothingFactor
    {
        set { if (Valid) Skeleton.SetSmoothing(value); }
    }


    /// Accessor to the singleton (@ref m_singleton)
    public static NIUserAndSkeleton Instance
    {
        get { return m_singleton; }
    }


    /// @brief Initialize the user and skeleton information
    /// 
    /// This method initializes the user and skeleton information. It assumes the 
    /// context is already valid (otherwise we fail).
    /// @note - Since we use a singleton pattern, multiple initializations will simply delete 
    /// the old information and create new one!
    /// @note - It is the responsibility of the initializer to call @ref Dispose.
    /// @param context the context used
    /// @param logger the logger object we will enter logs into
    /// @return true on success, false on failure. 
    public bool Init(NIEventLogger logger, NIContext context)
    {
        NIOpenNICheckVersion.Instance.ValidatePrerequisite();
        Dispose(); // to make sure we are not initialized
        if (InitWithContext(logger, context) == false)
            return false;
        
        m_userGenerator = context.CreateNode(NodeType.User) as UserGenerator;
        if (m_userGenerator == null || m_userGenerator.SkeletonCapability==null)
        {
            Log("Failed to create proper user generator.", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.Skeleton, NIEventLogger.VerboseLevel.Errors);
            // we either don't have a user generator or the user generator we received does not have a skeleton
            Dispose();
            return false;
        }
        // skeleton heuristics tries to handle the skeleton when the confidence is low. It can
        // have two values: 0 (no heuristics) or 255 (use heuristics).
        m_userGenerator.SetIntProperty("SkeletonHeuristics", 255);
        // makes sure we use all joints
        m_userGenerator.SkeletonCapability.SetSkeletonProfile(SkeletonProfile.All);
        PoseDetectionCapability cap = UserNode.PoseDetectionCapability;
        if (cap != null)
            m_legalPoses = cap.GetAllAvailablePoses();
        else
            m_legalPoses = null;
        m_poseDetectionCounter = new List<poseDetectionReferenceCounter>();
        m_userGenerator.LostUser += new EventHandler<UserLostEventArgs>(LostUserCallback);

        return true;
    }

    /// @brief Release a previously initialize image node.
    /// 
    /// This method releases a previously initialized image node.
    /// @note - Since we use a singleton pattern, only one place should do a release, otherwise the
    /// result could become problematic for other objects
    /// @note - It is the responsibility of the whoever called @ref Init to do the release. 
    /// @note - The release should be called BEFORE releasing the context. If the context is invalid, the result is
    /// undefined.
    public override void Dispose()
    {
        if (m_context == null)
        {
            return;
        }
        if (m_userGenerator != null)
        {
            if (m_poseDetectionCounter != null)
            {
                m_poseDetectionCounter.Clear();
                m_poseDetectionCounter = null;
            }
            m_context.ReleaseNode(m_userGenerator); // we call this even if the context is invalid because it is a singleton which will release stuff
            m_userGenerator = null;
        }
        base.Dispose();
    }

    /// @brief static accessor to @ref m_legalPoses
    /// 
    /// @note This is used to save the legal poses between runs so as to provide inspectors with
    /// a list of legal poses in the supported nodes
    /// @return @ref m_legalPoses
    public static string[] GetLegalPoses()
    {
        return m_legalPoses; 
    }


    // methods to encompass the pose detection capability

    /// @brief Gets a string array of supported poses
    /// 
    /// @return An array of supported poses, null if invalid or no supported poses
    public string[] GetAllAvailablePoses()
    {
        if (!Valid)
            return null;
        PoseDetectionCapability cap = UserNode.PoseDetectionCapability;
        if (cap == null)
            return null;
        return cap.GetAllAvailablePoses();
    }

    /// @brief Gets the current pose status.
    /// 
    /// @param userID The openNI user id whose status we seek
    /// @param poseName The name of the pose we want status on
    /// @param status a reference to the status we want to fill.
    /// @return true on success and false on failure.
    public bool GetPoseStatus(int userID, string poseName, ref NIPoseDetectionStateStatus status)
    {        
        if (!Valid)
            return false;
        PoseDetectionCapability cap = UserNode.PoseDetectionCapability;
        if (cap == null)
            return false;
        PoseDetectionStateStatus openNIStatus;
        try
        {
             openNIStatus = cap.GetPoseStatus(userID, poseName);
        }
        catch (System.Exception ex)
        {
            Log("Failed to get pose status for userID " + userID + " pose " + poseName + " with message "+ex.Message, NIEventLogger.Categories.Misc, NIEventLogger.Sources.BaseObjects, NIEventLogger.VerboseLevel.Errors);
            return false;
        }
        
        status.m_eState = openNIStatus.m_eState;
        status.m_eStatus = openNIStatus.m_eStatus;
        if (openNIStatus.m_poseTime <= 0 || openNIStatus.m_eState == PoseDetectionState.OutOfPose)
        {
            status.m_timePoseHeld = -1.0f;
        }
        else
        {
            long userGeneratorTime = m_userGenerator.Timestamp;
            if (userGeneratorTime <= openNIStatus.m_poseTime)
            {
                status.m_timePoseHeld = -1.0f;
            }
            else
            {
                userGeneratorTime -= openNIStatus.m_poseTime;
                status.m_timePoseHeld = (float)userGeneratorTime;
                status.m_timePoseHeld /= 1000000.0f; // to transform from microseconds to seconds
                status.m_timePoseHeld *= Time.timeScale; // to convert the coodrinates based on Time scale
            }
        }
        return true;
    }

    /// @brief Tells us if a pose is supported
    /// 
    /// @param pose The name of the pose we want to know if supported
    /// @return true if the pose is supported and false if not (or there is an error).
    public bool IsPoseSupported(string pose)
    {
        if (!Valid)
            return false;
        PoseDetectionCapability cap = UserNode.PoseDetectionCapability;
        if (cap == null)
            return false;
        return cap.IsPoseSupported(pose);
    }

    /// @brief Request a pose detection to start
    /// 
    /// This method requests a pose detection to start. It supports reference counting so as to
    /// make sure pose detection will continue as long as at least one 
    /// @param pose The pose we want to detect
    /// @param user The openNI user id of the user we want to detect on.
    /// @return true on success, false on failure.
    public bool RequestPoseDetection(string pose, int user)
    {
        if (UserNode.PoseDetectionCapability == null || pose==null)
            return false;
        foreach (poseDetectionReferenceCounter refCounter in m_poseDetectionCounter)
        {
            if (refCounter.m_usedID == user && pose.CompareTo(refCounter.m_pose) == 0)
            {
                // we already have this pose for this user so we are already detecting, just increase
                // the reference count
                refCounter.m_referenceCount++; 
                return true;
            }
        }
        // if we are here then we are not yet following this user so we need to create a new reference
        // counter
        try
        {
            UserNode.PoseDetectionCapability.StartPoseDetection(pose, user);
        }
        catch (System.Exception ex)
        {
            Log("Failed to start pose detection with message " + ex.Message, NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects, NIEventLogger.VerboseLevel.Errors);
            return false;
        }
        Log("Started following pose " + pose + " for user " + user, NIEventLogger.Categories.Misc, NIEventLogger.Sources.BaseObjects, NIEventLogger.VerboseLevel.Verbose);
        poseDetectionReferenceCounter newCounter = new poseDetectionReferenceCounter();
        newCounter.m_pose = pose;
        newCounter.m_referenceCount = 1;
        newCounter.m_usedID = user;
        m_poseDetectionCounter.Add(newCounter);
        return true;
    }

     /// @brief Releases a request to a pose detection 
    /// 
    /// This method releases a request to detect a pose for a user which lowers the reference count.
    /// if no one uses the pose, the pose detection will be stopped.
    /// @param pose The pose we want to detect
    /// @param user The openNI user id of the user we want to detect on.
    /// @return true on success, false on failure.
    public bool ReleasePoseDetection(string pose, int user)
    {
        if (UserNode.PoseDetectionCapability == null || pose == null)
            return false;

        for (int i = 0; i < m_poseDetectionCounter.Count; i++)
        {
            poseDetectionReferenceCounter refCounter = m_poseDetectionCounter[i];
            if (refCounter.m_usedID == user && pose.CompareTo(refCounter.m_pose)==0)
            {
                // we found the reference, lets dereference
                refCounter.m_referenceCount--;
                if (refCounter.m_referenceCount > 0)
                    return true; // nothing to do, others still need it.
                try
                {
                    UserNode.PoseDetectionCapability.StopSinglePoseDetection(user, pose);
                }
                catch (System.Exception ex)
                {
                    Log("Failed to stop pose detection with message " + ex.Message, NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects, NIEventLogger.VerboseLevel.Errors);
                    refCounter.m_referenceCount++; // we failed to stop so we return it...
                    return false;
                }
                Log("Stopped following pose " + pose + " for user " + user, NIEventLogger.Categories.Misc, NIEventLogger.Sources.BaseObjects, NIEventLogger.VerboseLevel.Verbose);
                m_poseDetectionCounter[i] = m_poseDetectionCounter[m_poseDetectionCounter.Count - 1];
                m_poseDetectionCounter.RemoveAt(m_poseDetectionCounter.Count - 1);
                return true;
            }
        }
        return false; // no such user
    }


    // protected members

    /// @brief Internal class to hold for a specific user and pose the reference count of the number
    /// of requests to start without releasing
    protected class poseDetectionReferenceCounter
    {
        public string m_pose; ///< @brief The pose name for which we count
        public int m_usedID;  ///< @brief The openNI user id for which we count
        public int m_referenceCount; ///< @brief The reference count itself.
    }

    /// @brief Internal list which contains the reference counting for each user and pose
    protected List<poseDetectionReferenceCounter> m_poseDetectionCounter;

    /// An internal object using the the OpenNI basic user generator node.
    protected UserGenerator m_userGenerator;
    /// @brief The singleton itself
    /// 
    /// NIImage uses a singleton pattern. There is just ONE NIUserSkeleton object which is used by all.
    protected static NIUserAndSkeleton m_singleton = new NIUserAndSkeleton();

    /// @brief Holds the legal poses as received from the user generator node.
    /// 
    /// @note This static member is refilled whenever the user node is initialized and saves the list of
    /// poses supported by that node. 
    static protected string[] m_legalPoses=null; 



    // private methods

    /// @brief private constructor
    /// 
    /// This is part of the singleton pattern, a protected constructor cannot be called externally (although
    /// it can be inherited for extensions. In which case the extender should also replace the singleton!
    private NIUserAndSkeleton()
    {
        m_userGenerator = null;
    }


    /// @brief callback for updating structures when a user is lost
    /// 
    /// This callback is called when a user is lost. It removes the user from all structures...
    /// @param sender who called the callback
    /// @param e the arguments of the event.
    private void LostUserCallback(object sender, UserLostEventArgs e)
    {
        for (int i = 0; i < m_poseDetectionCounter.Count; i++)
        {
            if(m_poseDetectionCounter[i].m_usedID != e.ID)
                continue; // irrelevant;
            m_poseDetectionCounter[i] = m_poseDetectionCounter[m_poseDetectionCounter.Count - 1];
            m_poseDetectionCounter.RemoveAt(m_poseDetectionCounter.Count - 1);
            i--;
        }
    }
}

