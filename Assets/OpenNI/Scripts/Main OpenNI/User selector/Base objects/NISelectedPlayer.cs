using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenNI;

/// @brief A selected player
/// 
/// This class is used to get information about a single player. By player we mean a human user
/// which have been selected by the player manager (@ref NIPlayerManager) to play the game.
/// @ingroup UserSelectionModule
public class NISelectedPlayer 
{
    /// @brief constructor
    public NISelectedPlayer()
    {
        m_user = null;
    }


    /// @brief Accessor to the openNI user ID (as appears in the user generator).
    public int OpenNIUserID
    {
        get { return Valid ? m_user.OpenNIUserID : -1; }
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
        referenceTransform=NIPlayerCandidateObject.m_InitializedZero;
        if(!Valid)
            return false;
        return m_user.GetReferenceSkeletonJointTransform(joint, out referenceTransform);
    }

    /// @brief Accessor to the time (Time.Time) when the player was SELECTED.
    ///  
    /// @note It could be that the player was selected but tracking began later.
    public float TimePlayerSelected
    {
        get { return Valid ? m_user.TimePlayerSelected : 0.0f; }
    }

    /// @brief Accessor to the frame (Time.frameCount) when the player was SELECTED.
    ///  
    /// @note It could be that the player was selected but tracking began later.
    public int FramePlayerSelected
    {
        get { return Valid ? m_user.FramePlayerSelected : -1; }
    }

    /// @brief Gets the current joint transformation for a specific joint
    /// 
    /// @param joint The joint we want the transformation to.
    /// @param curTransform [out] The current joint transformation
    /// @return True on success and false on failure (e.g. the user is not tracking).
    /// @note An exception might occur if there is an error (e.g. an illegal joint is used).
    public bool GetSkeletonJoint(SkeletonJoint joint, out SkeletonJointTransformation curTransform)
    {        
        curTransform = NIPlayerCandidateObject.m_InitializedZero;
        if (!Tracking)
            return false;
        if (m_user.Skeleton == null)
            return false;
        try
        {
            curTransform = m_user.Skeleton.GetSkeletonJoint(m_user.OpenNIUserID, joint);
        }
        catch
        {
            return false;
        }
        return true;

    }
    /// @brief Gets the current joint orientation for a specific joint
    /// 
    /// @param joint The joint we want the orientation to.
    /// @param curRotation [out] The current joint rotation
    /// @return True on success and false on failure (e.g. the user is not tracking).
    /// @note An exception might occur if there is an error (e.g. an illegal joint is used).
    public bool GetSkeletonJointOrientation(SkeletonJoint joint, out SkeletonJointOrientation curRotation)
    {
        curRotation = NIPlayerCandidateObject.m_InitializedZero.Orientation;
        if (!Tracking)
            return false;
        if (m_user.Skeleton == null)
            return false;
        try
        {
            curRotation = m_user.Skeleton.GetSkeletonJointOrientation(m_user.OpenNIUserID, joint);
        }
        catch
        {
            return false;
        }
        return true;
    }
    /// @brief Gets the current joint position for a specific joint
    /// 
    /// @param joint The joint we want the position to.
    /// @param curPos [out] The current joint rotation
    /// @return True on success and false on failure (e.g. the user is not tracking).
    /// @note An exception might occur if there is an error (e.g. an illegal joint is used).
    public bool GetSkeletonJointPosition(SkeletonJoint joint, out SkeletonJointPosition curPos)
    {
        curPos = NIPlayerCandidateObject.m_InitializedZero.Position;
        if (!Tracking)
            return false;
        if (m_user.Skeleton == null)
            return false;
        try
        {
            curPos = m_user.Skeleton.GetSkeletonJointPosition(m_user.OpenNIUserID, joint);
        }
        catch
        {
            return false;
        }
        return true;

    }

    /// @brief Recalculates the reference joints to be the current value if relevant.
    public void RecalcReferenceJoints()
    {
        if (Valid && Tracking)
            m_user.CalcReferenceJoints();
    }

    /// @brief True if the player is valid (can be used at all) and false otherwise
    public bool Valid
    {
        get { return m_user != null && ((m_user.PlayerStatus == UserStatus.Selected) || (m_user.PlayerStatus == UserStatus.Tracking)); }
    }

    /// @brief True if the player is valid and tracking (can be used for control).
    public bool Tracking
    {
        get { return Valid && m_user.PlayerStatus == UserStatus.Tracking; }
    }

    /// @brief Accessor to set and get the user the player relates to.
    public NIPlayerCandidateObject User
    {
        get { return m_user; }
        set 
        {
            if (m_user != value)
            {
                if (m_userChangeEventHandler != null)
                {
                    m_userChangeEventHandler(m_user, value); // raise the event of the change
                }                
                m_user = value; 
            }
        }
    }


    /// @brief delegate defined in order to register for changed user events
    /// 
    /// This is a delegate for method to be called when the user (@ref User) the player relates to
    /// changes.
    /// @param origUser The user before the change
    /// @param newUser the user after the change
    public delegate void PlayerUserChangeEventHandler(NIPlayerCandidateObject origUser, NIPlayerCandidateObject newUser);

    /// @brief event for detecting changes in users.
    /// 
    /// Anyone who wants to register to the "detect user change" event should do so here...
    public event PlayerUserChangeEventHandler m_userChangeEventHandler;



    /// @brief The relevant user object this player uses.
    protected NIPlayerCandidateObject m_user;
}

/// @brief Player status
public enum UserStatus
{
    Unselected, ///< @brief The user is unselected and not tested. This is a potential player
    Selected,   ///< @brief The user has been selected to be a player but is not yet ready (not tracking)
    Tracking,   ///< @brief the user has been selected and tracking. They should now be playing
    Failure     ///< @brief The user was selected in the past and failed to initialize. This means there is a chance this was a false identification.
}
