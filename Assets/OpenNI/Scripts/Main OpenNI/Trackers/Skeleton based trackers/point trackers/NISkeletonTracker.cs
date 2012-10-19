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

/// @brief A point tracker which follows a single joint from a skeleton for a specific player.
/// 
/// @ingroup OpenNIPointTrackers
public class NISkeletonTracker : NIPointTracker 
{
    /// An enum used to define the type of reference to use for @ref StartingPos.
    public enum StartingPosReferenceType
    {
        TrackedJointReference, ///< @brief The reference transform for the tracked joint is used
        SetPointReference,     ///< @brief A specific position is used (@ref m_StartPosModifier)

        /// @brief A modifier to another joint. 
        ///  
        /// The position would be the current position of @ref m_referenceJoint moved by @ref m_StartPosModifier)
        StaticModifierToOtherJoint,
        /// @brief A scaled modifier to another joint. 
        ///  
        /// The position would be the current position of @ref m_referenceJoint moved by @ref m_StartPosModifier) 
        /// multiplied by the magnitude of the vector between @ref m_referenceJointScale1 and @ref m_referenceJointScale2
        ScaledModifierToOtherJoint 
    }

    /// @brief How to calculate the starting pose.
    public StartingPosReferenceType m_startingPoseReferenceType;

    /// @brief The player number for the player whose hands we will follow
    public int m_playerNum;

    /// @brief The joint we are tracking.
    public SkeletonJoint m_jointToUse;

    /// the mapper we are working with
    public NIPlayerManager m_playerManager;

    /// @brief A joint used as a reference for the starting pos.
    /// 
    /// The meaning of this variable depends on @ref m_startingPoseReferenceType.
    /// For values of StaticModifierToOtherJoint and ScaledModifierToOtherJoint of
    /// @ref m_startingPoseReferenceType, the position of m_referenceJoint is the base of the calculation
    public SkeletonJoint m_referenceJoint;

    /// @brief Modifier for the starting position.
    /// 
    /// The meaning of this variable depends on @ref m_startingPoseReferenceType.
    /// - If @ref m_startingPoseReferenceType is SetPointReference then the starting position is
    ///   m_StartPosModifier
    /// - If @ref m_startingPoseReferenceType is StaticModifierToOtherJoint then the starting 
    ///   position is the position of @ref m_referenceJoint plus m_StartPosModifier
    /// - If @ref m_startingPoseReferenceType is ScaledModifierToOtherJoint then the starting
    ///   position is the position of @ref m_referenceJoint plus m_StartPosModifier multiplied by
    ///   the magnitude of the vector between the positions of @ref m_referenceJointScale1 and
    ///   @ref m_referenceJointScale2
    public Vector3 m_StartPosModifier;

    /// @brief reference joint for scaling
    /// 
    /// If @ref m_startingPoseReferenceType is ScaledModifierToOtherJoint then the starting
    /// position is the position of @ref m_referenceJoint plus m_StartPosModifier multiplied by
    /// the magnitude of the vector between the positions of @ref m_referenceJointScale1 and
    /// @ref m_referenceJointScale2
    public SkeletonJoint m_referenceJointScale1;

    /// @brief reference joint for scaling
    /// 
    /// If @ref m_startingPoseReferenceType is ScaledModifierToOtherJoint then the starting
    /// position is the position of @ref m_referenceJoint plus m_StartPosModifier multiplied by
    /// the magnitude of the vector between the positions of @ref m_referenceJointScale1 and
    /// @ref m_referenceJointScale2
    public SkeletonJoint m_referenceJointScale2;

    /// returns a unique name for the tracker type.
    /// @note this is what is used to identify the tracker
    /// @return the unique name.
    public override string GetTrackerType()
    {
        return "skeleton tracker for player " + m_playerNum + " tracking joint " + m_jointToUse;
    }

    /// @brief gets the player the tracker is tracking.
    ///  
    /// @return The tracked player object.
    public NISelectedPlayer GetTrackedPlayer()
    {
        return m_playerManager.GetPlayer(m_playerNum);
    }

    /// returns the initial position of the point.
    /// @note this is the position of the relevant hand when the player was identified!
    public override Vector3 StartingPos
    {
        get
        {
            switch (m_startingPoseReferenceType)
            {
                case StartingPosReferenceType.TrackedJointReference:
                    return GetStartingPosFromTrackedJoint();
                case StartingPosReferenceType.SetPointReference:
                    return GetStartingPosFromPos();
                case StartingPosReferenceType.StaticModifierToOtherJoint:
                    return GetStartingPosFromStaticModifer();
                case StartingPosReferenceType.ScaledModifierToOtherJoint:
                    return GetStartingPosFromScaledModifer();
            }
            return Vector3.zero; // should never get here!!
        }
    }

    /// return the delta of the current position (preferably from the current frame...) from 
    /// the last frame's position.
    /// @note Do not use CurDeltaPos during LateUpdate, this is because during LateUpdate 
    /// the previous point might have already been updated making CurDeltaPos return 0! 
    public override Vector3 CurDeltaPos
    {
        get { return CurPos-m_lastFrameCurPoint.LastGoodPointLastFrame; }
    }

    /// return the delta of the current position (preferably from the current frame...) from 
    /// the last frame's position.
    /// @note Do not use CurDeltaPosRaw during LateUpdate, this is because during LateUpdate 
    /// the previous point might have already been updated making CurDeltaPosRaw return 0! 
    public override Vector3 CurDeltaPosRaw
    {
        get { return CurDeltaPos; }
    }

    /// returns the current position with confidence
    /// 
    /// @param confidence the confidence of the point
    /// @return the position
    public Vector3 GetPosWithConfidence(out float confidence)
    {
        confidence=0.0f;
        NISelectedPlayer player = m_playerManager.GetPlayer(m_playerNum);
        if (player == null || player.Valid==false || player.Tracking==false)
            return Vector3.zero; // no position.
        SkeletonJointPosition skelJointPos;
        if (player.GetSkeletonJointPosition(m_jointToUse, out skelJointPos) == false)
            return Vector3.zero;
        confidence=skelJointPos.Confidence;
        return NIConvertCoordinates.ConvertPos(skelJointPos.Position);
    }


    public override Vector3 CurPos
    {
        get
        {
            return m_lastFrameCurPoint.LastGoodPoint;
        }
    }

    public override Vector3 CurPosRaw
    {
        get { return CurPos; }
    }

    public override Vector3 CurPosFromStart
    {
        get { return CurPos-StartingPos; }
    }
    public override Vector3 CurPosFromStartRaw
    {
        get { return CurPosFromStart; }
    }

    /// mono-behavior fixedUpdate
    /// This is used to update the position for the previous frame. 
    /// @note we use FixedUpdate because this can be updated multiple times during a frame.
    public void FixedUpdate()
    {
        NISelectedPlayer player = m_playerManager.GetPlayer(m_playerNum);
        if (player == null || player.Valid==false || player.Tracking==false)
            return; // no player to work with
        SkeletonJointPosition skelJointPos;
        if (player.GetSkeletonJointPosition(m_jointToUse, out skelJointPos))
        {
            m_lastFrameCurPoint.UpdatePoint(skelJointPos.Position, skelJointPos.Confidence);
        }
        if(m_startingPoseReferenceType==StartingPosReferenceType.StaticModifierToOtherJoint ||
           m_startingPoseReferenceType == StartingPosReferenceType.ScaledModifierToOtherJoint)
        {
            if (player.GetSkeletonJointPosition(m_referenceJoint, out skelJointPos))
            {
                m_lastFrameReferenceJoint.UpdatePoint(skelJointPos.Position, skelJointPos.Confidence);
            }
        }
        if (m_startingPoseReferenceType == StartingPosReferenceType.ScaledModifierToOtherJoint)
        {
            SkeletonJointPosition skelJointPos2;
            if (player.GetSkeletonJointPosition(m_referenceJointScale1, out skelJointPos) &&
                player.GetSkeletonJointPosition(m_referenceJointScale2, out skelJointPos2))
            {
                m_lastFrameScaleJoint1.UpdatePoint(skelJointPos.Position, skelJointPos.Confidence);
                m_lastFrameScaleJoint2.UpdatePoint(skelJointPos2.Position, skelJointPos2.Confidence);
            }
        }
    }

    public override void StopTracking()
    {
        base.StopTracking();
        m_lastFrameCurPoint = null;
        m_lastFrameReferenceJoint = null;
        m_lastFrameScaleJoint1 = null;
        m_lastFrameScaleJoint2 = null;
    }


     // protected methods


    /// an internal method to initialize the internal structures
    /// @note in most cases, inheriting methods should NOT override the base @ref InitTracking but
    /// rather override this method.
    /// @return true on success and false otherwise.
    protected override void InternalAwake()
    {
        base.InternalAwake();
        if (m_playerManager == null)
        {
            m_playerManager = FindObjectOfType(typeof(NIPlayerManager)) as NIPlayerManager;
            if (m_playerManager == null)
                throw new System.Exception("No player manager was found.");
        }
    }

    /// an internal method to initialize the internal structures
    /// @note in most cases, inheriting methods should NOT override the base @ref InitTracking but
    /// rather override this method.
    /// @return true on success and false otherwise.
    protected override bool InitInternalStructures()
    {
        m_lastFrameCurPoint = new NIPositionTrackerFrameManager();
        InitReferenceState();
        return true;
    }

    /// @brief Initializes the appropriate additional members depending on @ref m_startingPoseReferenceType
    protected void InitReferenceState()
    {
        if(m_startingPoseReferenceType==StartingPosReferenceType.ScaledModifierToOtherJoint)
        {
            m_lastFrameScaleJoint1 = new NIPositionTrackerFrameManager();
            m_lastFrameScaleJoint2 = new NIPositionTrackerFrameManager();
        }
        if(m_startingPoseReferenceType==StartingPosReferenceType.ScaledModifierToOtherJoint ||
           m_startingPoseReferenceType == StartingPosReferenceType.StaticModifierToOtherJoint)
        {
            m_lastFrameReferenceJoint = new NIPositionTrackerFrameManager();
        }
    }

    /// @brief Gets the starting position if @ref m_startingPoseReferenceType is TrackedJointReference
    /// 
    /// @return The reference position of the tracked joint (zero if there is a problem).
    protected Vector3 GetStartingPosFromTrackedJoint()
    {
        // The position we count from is the initial calibration pose.
        NISelectedPlayer player = m_playerManager.GetPlayer(m_playerNum);
        if (player == null || player.Valid == false)
            return Vector3.zero; // no position.
        SkeletonJointTransformation skelTrans;
        if (player.GetReferenceSkeletonJointTransform(m_jointToUse, out skelTrans) == false)
            return Vector3.zero; // we don't have the transform
        Point3D pos = skelTrans.Position.Position;
        Vector3 res = NIConvertCoordinates.ConvertPos(pos);
        if (skelTrans.Position.Confidence < 0.5f || float.IsNaN(res.magnitude) || res.z >= 0)
        {
            // this means the value is irrelevant.
            player.RecalcReferenceJoints();
            SkeletonJointPosition skelJointPos;
            if (player.GetSkeletonJointPosition(m_jointToUse, out skelJointPos) == false)
                return Vector3.zero;
            res = NIConvertCoordinates.ConvertPos(skelJointPos.Position);
        }
        return res;
    }

    /// @brief Gets the starting position if @ref m_startingPoseReferenceType is SetPointReference
    /// 
    /// @return m_StartPosModifier
    protected Vector3 GetStartingPosFromPos()
    {
        return m_StartPosModifier;
    }

    /// @brief Gets the starting position if @ref m_startingPoseReferenceType is StaticModifierToOtherJoint
    /// 
    /// @return The current position of m_referenceJoint modified by m_StartPosModifier
    protected Vector3 GetStartingPosFromStaticModifer()
    {
        return m_lastFrameReferenceJoint.LastGoodPoint + m_StartPosModifier;
    }

    /// @brief Gets the starting position if @ref m_startingPoseReferenceType is ScaledModifierToOtherJoint
    /// 
    /// @return The current position of m_referenceJoint modified by m_StartPosModifier scaled by
    /// the magnitude of the vector between m_referenceJointScale1 and m_referenceJointScale2
    protected Vector3 GetStartingPosFromScaledModifer()
    {
        Vector3 scaled = m_lastFrameScaleJoint1.LastGoodPoint - m_lastFrameScaleJoint2.LastGoodPoint;
        float scale = scaled.magnitude;
        return m_lastFrameReferenceJoint.LastGoodPoint + (m_StartPosModifier*scale);
    }

    /// used to get the last good value of the smooth point
    protected NIPositionTrackerFrameManager m_lastFrameCurPoint;

    /// used to get the last good value of the smooth reference joint
    protected NIPositionTrackerFrameManager m_lastFrameReferenceJoint;
    /// used to get the last good value of the first smooth reference scale joint
    protected NIPositionTrackerFrameManager m_lastFrameScaleJoint1;
    /// used to get the last good value of the second smooth reference scale joint
    protected NIPositionTrackerFrameManager m_lastFrameScaleJoint2;
}
