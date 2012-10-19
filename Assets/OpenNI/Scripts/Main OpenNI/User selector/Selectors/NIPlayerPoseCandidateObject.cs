using UnityEngine;
using System;
using System.Collections.Generic;
using OpenNI;

/// @brief Object for internal user management by the pose user selector.
///  
/// This class extends NIPlayerCandidateObject for use with NIPlayerManagerPoseSelection.
/// @ingroup UserSelectionModule
public class NIPlayerPoseCandidateObject : NIPlayerCandidateObject
{
       /// @brief Constructor
    ///  
    /// @param settingsManager The settings manager to use
    /// @param userID The user id of the user this relates to.
    public NIPlayerPoseCandidateObject(OpenNISettingsManager settingsManager, int userID) :
                       base(settingsManager, userID)
    {
        m_selectionOutOfPoseTime = float.MaxValue; // we put max value to make sure that this time will never arrive
        m_unselectionOutOfPoseTime = float.MinValue; // we put min value to make sure that even subtracting a couple of seconds we will always be in the past
    }

    public override bool SelectUser(int numRetries)
    {
        bool res=base.SelectUser(numRetries);
        if (res)
        {
            m_selectionOutOfPoseTime = float.MaxValue;
        }
        return res;
    }

    public override bool UnselectUser()
    {
        bool res = base.UnselectUser();
        if (res)
        {
            m_unselectionOutOfPoseTime = float.MaxValue;
        }
        return res;
    }

    /// @brief Time of selection out of pose.
    /// 
    /// This variable is used to hold the first time out of pose was detected on the selection
    /// pose AFTER the user was selected. It holds a large positive number (float.MaxValue) from the 
    /// time of selection until out of pose is detected and the number is irrelevant if the user is not
    /// selected.
    public float m_selectionOutOfPoseTime;

    /// @brief Time of unselection out of pose.
    /// 
    /// This variable is used to hold the first time out of pose was detected on the unselection
    /// pose AFTER the user was unselected. It holds a large positive number (float.MaxValue) from the time of
    /// unselection until out of pose is detected and the number is irrelevant if the user is not
    /// "unselected".
    public float m_unselectionOutOfPoseTime;
}
