using UnityEngine;
using System;
using System.Collections.Generic;
using OpenNI;

/// @brief Object for internal user management by the pose user selector.
///  
/// This class extends NIPlayerCandidateObject for use with NIPlayerManagerPoseSelection.
/// @ingroup UserSelectionModule
public class NIPlayerCOMCandidateObject : NIPlayerCandidateObject
{
       /// @brief Constructor
    ///  
    /// @param settingsManager The settings manager to use
    /// @param userID The user id of the user this relates to.
    public NIPlayerCOMCandidateObject(OpenNISettingsManager settingsManager, int userID) :
                       base(settingsManager, userID)
    {
    }

    protected override void UserCalibrationEndFail(CalibrationProgressEventArgs e)
    {
        base.UserCalibrationEndFail(e);
        if (m_playerStatus != UserStatus.Failure)
            return; // not really a failure
        m_COMWhenFail = m_settingsManager.UserGenrator.GetUserCenterOfMass(m_openNIUserID);
    }

    /// @brief the center of mass when failing to track
    /// 
    /// This variable is used to hold the center of masses when failing to track. This can
    /// later be used to ignore the user until they move.
    public Vector3 m_COMWhenFail;

    /// @brief The priority of the user (higher is better)
    /// 
    /// This is updated and used by @ref NIPlayerManagerCOMSelection.
    public float m_priority;
}
