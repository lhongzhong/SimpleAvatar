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
using System;
using System.Collections.Generic;
using OpenNI;

///@brief Component to add debug lines on skeleton controllers
/// 
/// This class is aimed at being hanged (and operated) by a skeleton controller to draw debug lines
/// on it.
/// @ingroup OpenNIDebugUtilities
public class NISkeletonControllerLineDebugger : MonoBehaviour
{
    /// A utility class to enable showing debug lines.
    [System.Serializable]
    public class DebugLineDef
    {
        public SkeletonJoint m_source; ///< the source joint of the line
        public SkeletonJoint m_target; ///< the target joint of the line

        /// constructor
        /// @param source the source joint of the line
        /// @param target the target joint of the line
        public DebugLineDef(SkeletonJoint source, SkeletonJoint target)
        {
            m_source = source;
            m_target = target;
        }

        /// constructor. Sets the source and target to invalid.
        public DebugLineDef()
        {
            m_source = SkeletonJoint.Invalid;
            m_target = SkeletonJoint.Invalid;
        }
    }

    /// This holds the lines we wish to draw for debugging. This is basically a list of joint pairs.
    /// A line will be drawn for each pair.
    /// @note if this has length of 0, no debug info is drawn (or collected).
    public List<DebugLineDef> m_debugLines;

    /// mono-behavior Initialization
    public void Awake()
    {
        InternalAwake();
    }

    /// @brief Initializes the joint
    /// 
    /// @note a line containing a joint not initialized will not be drawn. This means that it is 
    /// possible to use a debugger with lots of lines and connect it to a skeleton controller with few
    /// joints. The skeleton controller is responsible for setting the joints and therefore irrelevant
    /// joints will simply not be used.
    /// @param joint the joint to initialize.
    public virtual void InitJoint(SkeletonJoint joint)
    {
        m_jointsInfo[(int)joint] = new JointInfo();
    }


    /// @brief Utility method to update JointInfo for the joint
    /// 
    /// @param joint the joint we want to update the info for
    /// @param jointPos the position of the joint
    /// @param posConf the confidence of the position of the joint 
    /// @param jointRot the quaternion rotation of the joint
    /// @param rotConf the confidence of the rotation of the joint
    public virtual void UpdateJointInfoForJoint(SkeletonJoint joint, Vector3 jointPos, float posConf, Quaternion jointRot, float rotConf)
    {
        if (m_jointsInfo[(int)joint] == null)
            return; // irrelevant joint
        m_jointsInfo[(int)joint].m_jointPos = jointPos;
        m_jointsInfo[(int)joint].m_posConfidence = posConf;
        m_jointsInfo[(int)joint].m_rotation = jointRot;
        m_jointsInfo[(int)joint].m_rotConfidence = rotConf;
    }

    /// mono-behavior update.
    public void Update()
    {
        DrawDebugLines();
    }

    /// internal initialization (which can be overriden)
    protected virtual void InternalAwake()
    {
        int jointCount = Enum.GetNames(typeof(SkeletonJoint)).Length + 1; // Enum starts at 1
        m_jointsInfo = new JointInfo[jointCount];
        foreach (SkeletonJoint j in Enum.GetValues(typeof(SkeletonJoint)))
        {
            m_jointsInfo[(int)j] = null;
        }
    }

    /// This utility method draws debug lines between the joints as defined in @ref m_debugLines
    protected virtual void DrawDebugLines()
    {
        if (m_debugLines == null)
            return;
        // we simply go over every line and try to draw it.
        for (int i = 0; i < m_debugLines.Count; i++)
        {
            DebugLineDef line=m_debugLines[i];
            if (m_jointsInfo[(int)line.m_source] == null ||
               m_jointsInfo[(int)line.m_target] == null)
                continue; // an illegal joint in one of the sides
            // get the source and target information
            JointInfo sourceData = m_jointsInfo[(int)line.m_source];
            JointInfo targetData = m_jointsInfo[(int)line.m_target];
            if (sourceData.m_jointPos == targetData.m_jointPos)
                continue; // the line is really not there as both ends are the same...
            // we choose the color based on the confidence of the 
            DrawSingleLine(sourceData, targetData,i);
        }
    }

    /// Calculates a color for the line
    /// @param sourceData the joint info on the source joint
    /// @param targetData the joint info on the target joint
    /// @return a color for the line
    protected virtual Color GetColor(JointInfo sourceData, JointInfo targetData)
    {
        if (sourceData.m_posConfidence < 0.5 || targetData.m_posConfidence < 0.5)
            return Color.red;
        else if (sourceData.m_posConfidence < 1 || targetData.m_posConfidence < 1)
            return Color.yellow;
        return Color.green;
    }

    /// Does the actual drawing of the line
    /// @param sourceData the joint info on the source joint
    /// @param targetData the joint info on the target joint
    /// @param lineNumber the number of the line in m_debugLines
    protected virtual void DrawSingleLine(JointInfo sourceData, JointInfo targetData, int lineNumber)
    {
        Debug.DrawLine(sourceData.m_jointPos, targetData.m_jointPos, GetColor(sourceData,targetData));
    }
    /// @brief Internal struct to save joint info.
    /// 
    /// This is an internal class to save the last info for each joint
    protected class JointInfo
    {
        public Vector3 m_jointPos; ///< the last position reported by the skeleton for this joint (converted to Unity coordinates)
        public float m_posConfidence; ///< the confidence of the last position
        public Quaternion m_rotation; ///< the last rotation reported by the skeleton for this joint (converted to Unity coordinates)
        public float m_rotConfidence; ///< the confidence of the last rotation
    }

    /// Contains the joint info for each joint.
    /// @note it is the responsibility of the caller NOT to call on illegal joints.
    protected JointInfo[] m_jointsInfo;
}
