using UnityEngine;
using System.Collections.Generic;
using OpenNI;
using System;

/// @brief Class to select players using poses
/// 
/// A @ref NIPlayerManager implementation which uses poses to select and unselect the players.
/// @ingroup UserSelectionModule
public class NIPlayerManagerPoseSelection : NIPlayerManager
{

    /// true if the mapper is valid, false otherwise.
    public override bool Valid
    {
        get { return m_valid && base.Valid; }
    }

    public override string GetSelectionString()
    {
        return "User selector based on pose";
    }

    public string m_PoseToSelect; ///< @brief This is the pose used for selection
    public string m_PoseToUnselect; ///< @brief This is the pose used for unselect. "" means no unselection!

    /// @brief Time between selection and unselection
    /// 
    /// This is a time to switch between selection and unselection, i.e. if the the select pose
    /// was detected, then the unselect pose will be ignored until m_timeToSwitch seconds have passed
    /// from being out of pose in that pose. By the same measure, the selection pose will not
    /// be tested until m_timeToSwitch seconds have passed from the out of pose of the unselect pose.
    public float m_timeToSwitch;

    
    protected override bool UnsafeUnselectPlayer(int playerNumber)
    {
        NISelectedPlayer player = m_players[playerNumber];
        NIPlayerPoseCandidateObject user = player.User as NIPlayerPoseCandidateObject;
        user.m_unselectionOutOfPoseTime = float.MaxValue; // to make sure we can select again!
        return base.UnsafeUnselectPlayer(playerNumber);
    }

    protected override void InternalInit()
    {
        base.InternalInit();
        m_valid = false;
        if (base.Valid == false)
            return;
        if(m_contextManager.UserGenrator.IsPoseSupported(m_PoseToSelect) == false)
        {
            m_contextManager.Log("Selection pose "+m_PoseToSelect+" not supported by pose detection capability!", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.UserSelector, NIEventLogger.VerboseLevel.Errors);
            return;
        }
        if (m_PoseToUnselect != null && m_PoseToUnselect.CompareTo("")!=0 && m_contextManager.UserGenrator.IsPoseSupported(m_PoseToUnselect) == false)
        {
            m_contextManager.Log("Unselection pose " + m_PoseToUnselect + " not supported by pose detection capability!", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.UserSelector, NIEventLogger.VerboseLevel.Errors);
            return;
        }
        m_contextManager.UserGenrator.UserNode.PoseDetectionCapability.PoseDetected += new EventHandler<PoseDetectedEventArgs>(PoseDetectedCallback);
        m_contextManager.UserGenrator.UserNode.PoseDetectionCapability.OutOfPose += new EventHandler<OutOfPoseEventArgs>(OutOfPoseDetectedCallback);
        m_valid = true;
    }

    protected override void AddNewUser(int userID)
    {
        base.AddNewUser(userID);
        m_contextManager.UserGenrator.RequestPoseDetection(m_PoseToSelect, userID);
    }

    protected override NIPlayerCandidateObject GetNewUserObject(int userID)
    {
        return new NIPlayerPoseCandidateObject(m_contextManager, userID);
    }

    protected bool m_valid=false; ///< @brief Internal member which holds true when valid.

    /// @brief Internal method to handle detecting the selection pose for a specific user
    ///  
    /// @param user The user object of the detected user.
    protected virtual void DetectSelectionPoseForUser(NIPlayerPoseCandidateObject user)
    {
        if(user.PlayerStatus==UserStatus.Tracking || user.PlayerStatus==UserStatus.Selected)
            return; // the user is already selected so nothing to do here...
        if (user.m_unselectionOutOfPoseTime > Time.time - m_timeToSwitch) 
            return; // we have not yet received an out of pose or not enough time has passed.
        // reaching here means we detected a selection pose and we should use it to select. Lets
        // try to find a good player to use.
        int playerIndex;
        for(playerIndex=0; playerIndex<m_players.Count; playerIndex++)
        {
            if (m_players[playerIndex].Valid == false)
            {
                break;
            }
        }
        if (playerIndex == m_players.Count)
            return; // we did not find a good player to use.
        user.SelectUser(m_numRetries);
        m_players[playerIndex].User = user;
        if (m_PoseToUnselect != null && m_PoseToUnselect.CompareTo("")!=0)
        {
            m_contextManager.UserGenrator.RequestPoseDetection(m_PoseToUnselect, user.OpenNIUserID);
        }
    }


    /// @brief Internal method to handle detecting the out of pose for the selection pose for a specific user
    ///  
    /// @param user The user object of the detected user.
    protected virtual void OutOfPoseSelectionPoseForUser(NIPlayerPoseCandidateObject user)
    {
        if (user.m_selectionOutOfPoseTime > Time.time)
        {
            user.m_selectionOutOfPoseTime = Time.time;
            if (user.PlayerStatus != UserStatus.Selected && user.PlayerStatus != UserStatus.Tracking)
                return;
            if (m_PoseToSelect.CompareTo(m_PoseToUnselect)!=0)
            {
                m_contextManager.UserGenrator.ReleasePoseDetection(m_PoseToSelect, user.OpenNIUserID);
            }
        }
    }


    
    /// @brief Internal method to handle detecting the selection pose for a specific user
    ///  
    /// @param user The user object of the detected user.
    protected virtual void DetectUnselectionPoseForUser(NIPlayerPoseCandidateObject user)
    {
        if (user.PlayerStatus == UserStatus.Unselected || user.PlayerStatus == UserStatus.Failure)
            return; // the user is already not selected so nothing to do here...
        if (user.m_selectionOutOfPoseTime > Time.time - m_timeToSwitch)
            return; // we have not yet received an out of pose or not enough time has passed.
        // reaching here means we detected an unselection pose and we should use it to select. Lets
        // try to find the relevant player.
        int playerIndex;
        for (playerIndex = 0; playerIndex < m_players.Count; playerIndex++)
        {
            if (m_players[playerIndex].User == user)
            {
                UnselectPlayer(playerIndex);
                m_contextManager.UserGenrator.RequestPoseDetection(m_PoseToSelect, user.OpenNIUserID);
                return;
            }
        }
    }

    /// @brief Internal method to handle detecting the out of pose for the unselection pose for a specific user
    ///  
    /// @param user The user object of the detected user.
    protected virtual void OutOfPoseUnselectionPoseForUser(NIPlayerPoseCandidateObject user)
    {
        if (user.m_unselectionOutOfPoseTime > Time.time)
        {
            user.m_unselectionOutOfPoseTime = Time.time;
            if (user.PlayerStatus == UserStatus.Selected || user.PlayerStatus == UserStatus.Tracking)
                return;
            if (m_PoseToSelect.CompareTo(m_PoseToUnselect)!=0)
            {
                m_contextManager.UserGenrator.ReleasePoseDetection(m_PoseToUnselect, user.OpenNIUserID);
            }
        }
    }



    /// @brief pose detection callback
    /// 
    /// @param sender who called the callback
    /// @param e the arguments of the event.
    private void PoseDetectedCallback(object sender, PoseDetectedEventArgs e)
    {
        m_contextManager.Log("found pose " + e.Pose + " for user=" + e.ID, NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Skeleton, NIEventLogger.VerboseLevel.Verbose);

        if (e.Pose.CompareTo(m_PoseToSelect)!=0 && e.Pose.CompareTo(m_PoseToUnselect)!=0)
            return; // irrelevant pose

        NIPlayerPoseCandidateObject poseUser = GetUserFromUserID(e.ID) as NIPlayerPoseCandidateObject;
        if (poseUser == null)
            return; // irrelevant user
        if(e.Pose.CompareTo(m_PoseToSelect)==0)
        {
            DetectSelectionPoseForUser(poseUser);
        }
        if (e.Pose.CompareTo(m_PoseToUnselect)==0) // we do NOT put an else because the select and unselect might be the same pose
        {
            DetectUnselectionPoseForUser(poseUser);
        }
    }


    /// @brief out of pose detection callback    
    /// @param sender who called the callback
    /// @param e the arguments of the event.
    private void OutOfPoseDetectedCallback(object sender, OutOfPoseEventArgs e)
    {
        m_contextManager.Log("found out of pose " + e.Pose + " for user=" + e.ID, NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Skeleton, NIEventLogger.VerboseLevel.Verbose);
        if (e.Pose.CompareTo(m_PoseToSelect)!=0 && e.Pose.CompareTo(m_PoseToUnselect)!=0)
            return; // irrelevant

        NIPlayerPoseCandidateObject poseUser = GetUserFromUserID(e.ID) as NIPlayerPoseCandidateObject;
        if (poseUser == null)
            return; // irrelevant user
 
        if (e.Pose.CompareTo(m_PoseToSelect)==0)
        {
            OutOfPoseSelectionPoseForUser(poseUser);
        }
        if (e.Pose.CompareTo(m_PoseToUnselect)==0) // we do NOT put an else because the select and unselect might be the same pose
        {
            OutOfPoseUnselectionPoseForUser(poseUser);
        }
    }
}
