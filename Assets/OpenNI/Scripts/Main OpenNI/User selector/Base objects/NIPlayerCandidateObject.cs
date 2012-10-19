using UnityEngine;
using System;
using System.Collections.Generic;
using OpenNI;

/// @brief Object for internal user management by the user selector.
///  
/// This class represents a single user in the user selector which is a candidate to be a player.
/// The user is chosen at construction but the selection process can change their state.
/// This will later be user by @ref NISelectedPlayer to represent a single player.
/// @ingroup UserSelectionModule
public class NIPlayerCandidateObject
{
    /// @brief Accessor to the openNI user ID (as appears in the user generator).
    public int OpenNIUserID
    {
        get { return m_openNIUserID; }
    }

    /// @brief Gets a reference joint position and orientation on the joint.
    ///  
    /// This method gets the reference joint information for the joint. The reference
    /// is the joint position and transformation at the time when the user was chosen AND
    /// started tracking.
    /// @param joint The joint we want information on.
    /// @param referenceTransform [out] The reference Transform.
    /// @return True on success and false on failure (e.g. an illegal joint or the user is not tracking).
    public bool GetReferenceSkeletonJointTransform(SkeletonJoint joint, out SkeletonJointTransformation referenceTransform)
    {
        referenceTransform = m_InitializedZero;
        if (m_playerStatus != UserStatus.Tracking || m_openNIUserID <= 0)
            return false;
        referenceTransform = m_referenceSkeletonJointTransform[joint];
        return true;
    }

    /// @brief Accessor to the time (Time.Time) when the player was SELECTED.
    ///  
    /// @note It could be that the player was selected but tracking began later.
    public float TimePlayerSelected
    {
        get { return m_timePlayerSelected; }
    }

    /// @brief Accessor to the frame (Time.frameCount) when the player was SELECTED.
    ///  
    /// @note It could be that the player was selected but tracking began later.
    public int FramePlayerSelected
    {
        get { return m_framePlayerSelected; }
    }

    /// @brief The player status.
    ///  
    /// @note The player is generally selected or tracking when used externally to the player manager.
    public UserStatus PlayerStatus
    {
        get { return m_playerStatus; }
    }

    /// @brief Accessor to the relevant skeleton capability. Returns null on failure.
    public SkeletonCapability Skeleton
    {
        get
        {
            if (m_settingsManager.UserSkeletonValid == false)
                return null;
            return m_settingsManager.UserGenrator.Skeleton;
        }
    }


    // end implement interface

    /// @brief Constructor
    ///  
    /// @param settingsManager The settings manager to use
    /// @param userID The user id of the user this relates to.
    public NIPlayerCandidateObject(OpenNISettingsManager settingsManager, int userID)
    {
        m_settingsManager = settingsManager;
        settingsManager.UserGenrator.Skeleton.CalibrationComplete += new EventHandler<CalibrationProgressEventArgs>(CalibrationEndCallback);

        m_openNIUserID = userID; 
        m_referenceSkeletonJointTransform=new Dictionary<SkeletonJoint,SkeletonJointTransformation>();
        Reset();
    }

    /// @brief Select a user 
    /// 
    /// This makes the player select a user (i.e. the user selector has just selected them).
    /// This means the user state changes and initialization is performed.
    /// @note - This will also start the user on the way to be tracked.
    /// - If we fail, the player will become reseted!
    /// 
    /// @param numRetries This is the number of retries to perform if we failed to calibrate
    /// @return true on success, false on failure. @note success means selection, if the user fails
    /// to track, this will result in a change of the state!
    public virtual bool SelectUser(int numRetries)
    {
        Reset(); // forget everything so far...

        if(m_settingsManager.UserGenrator.Valid!=true)
            return false;

        // initialize the generic stuff. we will reset if necessary later on.
        m_timePlayerSelected = Time.time;
        m_framePlayerSelected = Time.frameCount;

        // we need to know if we are already tracking or not. This is a way to know if the
        // user id is legal though as we will get an exception if not
        try
        {
            if (m_settingsManager.UserGenrator.IsTracking(m_openNIUserID))
            {
                m_playerStatus = UserStatus.Tracking;
                if (CalcReferenceJoints())
                {
                    return true;
                }
                Reset();
                return false;
            }
        }
        catch 
        {
            return false;
        }
        // if we are here then we are not currently tracking and the user is legal.
        m_playerStatus = UserStatus.Selected;
        if(!Skeleton.IsCalibrating(m_openNIUserID))
        {
            m_numRetries = numRetries;
            Skeleton.RequestCalibration(m_openNIUserID, false);
        }
        
        return true;
    }

    /// @brief Unselect the current user (making it untracked).
    /// 
    /// @return True on success, false on failure.
    public virtual bool UnselectUser()
    {
        if(m_settingsManager.UserGenrator.Valid!=true)
            return false;
        m_playerStatus = UserStatus.Unselected;
        try
        {
            if(Skeleton.IsCalibrating(m_openNIUserID))
            {
                Skeleton.AbortCalibration(m_openNIUserID);
            }
            if(Skeleton.IsTracking(m_openNIUserID))
            {
                Skeleton.StopTracking(m_openNIUserID);
            }
        }
        catch
        {
            return false;	
        }
        return true;
    }

    /// @brief resets the player object
    /// 
    /// @note This does @b NOT reset the basics created at construction (which user and which
    /// settings manager).
    public virtual void Reset()
    {
        m_timePlayerSelected = -1.0f;
        m_framePlayerSelected = -1;
        m_playerStatus = UserStatus.Unselected;
        m_referenceSkeletonJointTransform.Clear();
    }


    /// @brief Method to calculate the reference joints.
    /// 
    /// @return True on success, false on failure.
    public virtual bool CalcReferenceJoints()
    {
        m_referenceSkeletonJointTransform.Clear();
        if(Skeleton==null)
            return false;
        foreach(SkeletonJoint joint in Enum.GetValues(typeof(SkeletonJoint)))
        {
            SkeletonJointTransformation jointTransformation;
            try
            {
                if((!Skeleton.IsJointAvailable(joint)) || (!Skeleton.IsJointActive(joint)))
                    continue;
                jointTransformation=Skeleton.GetSkeletonJoint(m_openNIUserID, joint);
                m_referenceSkeletonJointTransform.Add(joint, jointTransformation);
            }
            catch
            {
                return false;
            }
        }
        
        return true;
    }
    protected int m_openNIUserID; ///< @brief openNI user ID (as appears in the user generator).


    /// @brief Reference joint information
    /// 
    /// An array which holds for each joint the reference joint information for that joint. 
    /// The reference is the joint position and transformation at the time when the user was
    /// chosen AND started tracking.
    /// If no reference is available (e.g. because the joint is not tracked or because the player
    /// is not yet tracking), zero is returned (0 rotation, 0 confidence, 0 position).
    protected Dictionary<SkeletonJoint,SkeletonJointTransformation> m_referenceSkeletonJointTransform;
    protected float m_timePlayerSelected; ///< @brief The time (Time.Time) when the player was SELECTED
    protected int m_framePlayerSelected; ///< @brief The frame (Time.framecount) when the player was SELECTED
    protected OpenNISettingsManager m_settingsManager; ///< The user generator object.
    protected UserStatus m_playerStatus;            ///< The current player status
    protected int m_numRetries; ///< This is the number of retries left when trying to calibrate after calibration failure.

    /// @brief static to have an initialized value for the skeleton transformation.
    public static SkeletonJointTransformation m_InitializedZero = new SkeletonJointTransformation();

    /// @brief virtual function on calibration success
    ///  
    /// This method starts the tracking and initializes everything. It is virtual to allow overriding
    protected virtual void UserCalibrationEndSuccess()
    {
        Skeleton.StartTracking(m_openNIUserID);
        m_playerStatus = UserStatus.Tracking;
        CalcReferenceJoints();
    }

    /// @brief virtual function on calibration failure
    ///  
    /// @param e the calibration error arguments from the calibration callback.
    /// This method retries the calibration if necessary. It is virtual to allow overriding
    protected virtual void UserCalibrationEndFail(CalibrationProgressEventArgs e)
    {
        if (e.Status == CalibrationStatus.ManualAbort)
            return; // it is a legal option...
        m_playerStatus = UserStatus.Failure;
        if(m_numRetries>0)
        {
            m_numRetries--;
            Skeleton.RequestCalibration(m_openNIUserID, true);
        }
    }

    /// @brief callback for updating structures when calibration ends
    /// 
    /// This callback is called when a calibration process ends. If the calibration succeeded
    /// then the user is in a calibrated state, otherwise it starts the calibration process from
    /// scratch.
    /// @param sender who called the callback
    /// @param e the arguments of the event.
    private void CalibrationEndCallback(object sender, CalibrationProgressEventArgs e)
    {
        if (e.ID != m_openNIUserID)
            return; // not us...
        m_settingsManager.Log("finished calibration for user=" + e.ID + " status=" + e.Status, NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Skeleton, NIEventLogger.VerboseLevel.Verbose);
        if (e.Status == CalibrationStatus.OK)
        {
            UserCalibrationEndSuccess();
        }
        else
        {
            UserCalibrationEndFail(e);
        }
        
    }

}
