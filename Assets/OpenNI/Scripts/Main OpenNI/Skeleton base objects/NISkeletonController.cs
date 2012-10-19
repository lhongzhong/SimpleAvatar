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

///@brief Component to control skeleton movement by the user's motion.
/// 
/// This class controls the skeleton supplied to it. The user needs to drag & drop the
/// relevant joints (head, legs, shoulders etc.) and on update this will query OpenNI to get
/// the skeleton updated positions and rotate everything accordingly.
/// @ingroup SkeletonBaseObjects
public class NISkeletonController : MonoBehaviour
{
    /// @brief The array of joints to control.
    /// 
    /// These transforms are what the user should drag & drop to in order to connect the skeleton 
    /// controller component to the skeleton it needs to control.
    /// @note not all transforms are currently supported in practice and therefore not all
    /// are necessary. Furthermore, if only some of the transforms are dragged, only those transforms will be
    /// moved. This means that in order for everything to work properly, all "rigged" joints need to be included
    /// (by "rigged" we mean those that will move all the important things in the model)
    public Transform[] m_jointTransforms;

    /// @brief If this is true the joint positions will be updated
    public bool m_updateJointPositions = false;
    /// @brief If this is true the entire game object will move
    public bool m_updateRootPosition = false;
    /// @brief If this is true the joint orientation will be updated
    public bool m_updateOrientation = true;
    /// @brief The speed in which the orientation can change
    public float m_rotationDampening = 15.0f;
    /// @brief Scale for the positions
    public float m_scale = 0.001f;
    /// @brief Speed for movement of the centeral position
    public float m_speed = 1.0f;

    /// @brief The user to player mapper (to figure out the user ID for each player).
    public NIPlayerManager m_playerManager;

    /// @brief Which player this skeleton follows
    public int m_playerNumber;

    /// Holds a line debugger which shows the lines connecting the joints. If null, nothing is
    /// drawn.
    public NISkeletonControllerLineDebugger m_linesDebugger;
    
    /// Activates / deactivate the game object (skeleton and all sub objects)
    /// @param state on true sets to active, on false sets to inactive
    public void SetSkeletonActive(bool state)
    {
        gameObject.SetActiveRecursively(state);
    }

    /// mono-behavior Initialization
    public void Start()
    {
        if (m_playerManager == null)
            m_playerManager = FindObjectOfType(typeof(NIPlayerManager)) as NIPlayerManager;
        if (m_playerManager == null)
            throw new System.Exception("Must have a player manager to control the skeleton!");
        int jointCount = Enum.GetNames(typeof(SkeletonJoint)).Length + 1; // Enum starts at 1
        m_jointsInitialRotations = new Quaternion[jointCount];
        // save all initial rotations
        // NOTE: Assumes skeleton model is in "T" pose since all rotations are relative to that pose
        foreach (SkeletonJoint j in Enum.GetValues(typeof(SkeletonJoint)))
        {
            if((int)j>=m_jointTransforms.Length)
                continue; // if we increased the number of joints since the initialization.
            if (m_jointTransforms[(int)j])
            {
                // we will store the relative rotation of each joint from the game object rotation
                // we need this since we will be setting the joint's rotation (not localRotation) but we 
                // still want the rotations to be relative to our game object
                // Quaternion.Inverse(transform.rotation) gives us the rotation needed to offset the game object's rotation
                m_jointsInitialRotations[(int)j] = Quaternion.Inverse(transform.rotation) * m_jointTransforms[(int)j].rotation;
                if (m_linesDebugger != null)
                {
                    m_linesDebugger.InitJoint(j);
                }
            }
        }
        m_originalRootPosition = transform.localPosition;
        // start out in calibration pose
        RotateToCalibrationPose();
    }


    /// mono-behavior update (called once per frame)
	public void Update () 
    {
        if (m_playerManager==null || m_playerManager.Valid == false)
            return; // we can do nothing.

        NISelectedPlayer player = m_playerManager.GetPlayer(m_playerNumber);
        if (player == null || player.Valid == false || player.Tracking==false)
        {
            RotateToCalibrationPose(); // we don't have anything to work with.
            return;
        }
        Vector3 skelPos = Vector3.zero;
        SkeletonJointTransformation skelTrans;
        if (player.GetReferenceSkeletonJointTransform(SkeletonJoint.Torso, out skelTrans))
        {
            if(skelTrans.Position.Confidence<0.5f)
            {
                player.RecalcReferenceJoints(); // we NEED the torso to be good.
            }
            if (skelTrans.Position.Confidence >= 0.5f)
            {
                skelPos = NIConvertCoordinates.ConvertPos(skelTrans.Position.Position);
            }
        }
        UpdateSkeleton(player, skelPos);
	}

    /// @brief updates the root position
    /// 
    /// This method updates the root position and if m_updateRootPosition is true, also move the entire transform
    /// @note we do not update if we do not have a high enough confidence!
    /// @param skelRoot the new central position
    /// @param centerOffset the offset we should use on the center (when moving the root). 
    /// This is usually the starting position (so the skeleton will not "jump" when doing the first update
    protected void UpdateRoot(SkeletonJointPosition skelRoot, Vector3 centerOffset)
    {
        if (skelRoot.Confidence < 0.5f)
            return; // we are not confident enough!
        m_rootPosition = NIConvertCoordinates.ConvertPos(skelRoot.Position);
        m_rootPosition -= centerOffset;
        m_rootPosition *= m_scale * m_speed;
        m_rootPosition = transform.rotation * m_rootPosition;
        m_rootPosition += m_originalRootPosition; 
        if (m_updateRootPosition)
        {
            transform.position =  m_rootPosition;
        }
    }


    /// @brief updates a single joint
    /// 
    /// This method updates a single joint. The decision of what to update (orientation, position)
    /// depends on m_updateOrientation and m_updateJointPositions. Only joints with high confidence
    /// are updated. @note it is possible to update only position or only orientation even though both
    /// are expected if the confidence of one is low.
    /// @param centerOffset the new central position
    /// @param joint the joint we want to update
    /// @param skelTrans the new transformation of the joint
    protected void UpdateJoint(SkeletonJoint joint, SkeletonJointTransformation skelTrans, SkeletonJointPosition centerOffset)
    {
        // make sure something is hooked up to this joint
        if ((int)joint >= m_jointTransforms.Length || !m_jointTransforms[(int)joint])
        {
            return;
        }
        // if we have debug lines to draw we need to collect the data.
        if (m_linesDebugger!=null)
        {
            Vector3 pos = CalcJointPosition(joint, ref skelTrans, ref centerOffset) + transform.position;
            float posConf = skelTrans.Position.Confidence;
            Quaternion rot = CalcRotationForJoint(joint, ref skelTrans, ref centerOffset);
            float rotConf = skelTrans.Orientation.Confidence;
            m_linesDebugger.UpdateJointInfoForJoint(joint, pos, posConf, rot, rotConf);

        }
        
        // modify orientation (if needed and confidence is high enough)
        if (m_updateOrientation && skelTrans.Orientation.Confidence >= 0.5)
        {
            m_jointTransforms[(int)joint].rotation=CalcRotationForJoint(joint, ref skelTrans, ref centerOffset);
        }

        // modify position (if needed, and confidence is high enough)
        if (m_updateJointPositions && skelTrans.Position.Confidence>=0.5f)
        {
            m_jointTransforms[(int)joint].localPosition = CalcJointPosition(joint, ref skelTrans, ref centerOffset);
        }
    }

    /// This rotates the skeleton to the requested calibration position.
    /// @note it assumes the initial position is a "T" position and orients accordingly (returning
    /// everything to its base and then rotating the arms).
    public void RotateToCalibrationPose()
    {
        foreach (SkeletonJoint j in Enum.GetValues(typeof(SkeletonJoint)))
        {
            if ((int)j<m_jointTransforms.Length && m_jointTransforms[(int)j]!=null)
            {
                m_jointTransforms[(int)j].rotation = transform.rotation * m_jointsInitialRotations[(int)j];
            }
        }
        // calibration pose is skeleton base pose ("T") with both elbows bent in 90 degrees
        Transform elbow=m_jointTransforms[(int)SkeletonJoint.RightElbow];
        if (elbow != null)
            elbow.rotation = transform.rotation * Quaternion.Euler(0, -90, 90) * m_jointsInitialRotations[(int)SkeletonJoint.RightElbow];
        elbow = m_jointTransforms[(int)SkeletonJoint.LeftElbow];
        if (elbow != null)
            elbow.rotation = transform.rotation * Quaternion.Euler(0, 90, -90) * m_jointsInitialRotations[(int)SkeletonJoint.LeftElbow];
        if (m_updateJointPositions)
        {
            // we want to position the skeleton in a calibration pose. We will therefore position select
            // joints
            UpdateJointPosition(SkeletonJoint.Torso,0,0,0);
            UpdateJointPosition(SkeletonJoint.Head, 0, 450, 0);
            UpdateJointPosition(SkeletonJoint.LeftShoulder, -150, 250, 0);
            UpdateJointPosition(SkeletonJoint.RightShoulder, 150, 250, 0);
            UpdateJointPosition(SkeletonJoint.LeftElbow, -450, 250, 0);
            UpdateJointPosition(SkeletonJoint.RightElbow, 450, 250, 0);
            UpdateJointPosition(SkeletonJoint.LeftHand, -450, 450, 0);
            UpdateJointPosition(SkeletonJoint.RightHand, 450, 450, 0);
            UpdateJointPosition(SkeletonJoint.LeftHip, -100, -250, 0);
            UpdateJointPosition(SkeletonJoint.RightHip, 100, -250, 0);
            UpdateJointPosition(SkeletonJoint.LeftKnee, -100, -700, 0); 
            UpdateJointPosition(SkeletonJoint.RightKnee, 100, -700, 0); 
            UpdateJointPosition(SkeletonJoint.LeftFoot, -100, -1150, 0);
            UpdateJointPosition(SkeletonJoint.RightFoot, 100, -1150, 0);

        }
    }

    /// @brief updates the skeleton
    /// 
    /// This method is responsible for updating the skeleton based on new information.
    /// It is called by an external manager with the correct user which controls this specific skeleton.
    /// @param player The player object for the player controlling us.
    /// @param centerOffset the offset we should use on the center (when moving the root). 
    /// This is usually the starting position (so the skeleton will not "jump" when doing the first update
    public void UpdateSkeleton(NISelectedPlayer player, Vector3 centerOffset)
    {
        if (player.Valid == false)
            return; // irrelevant player
        if (player.Tracking == false)
            return; // not tracking

        // we use the  torso as root
        SkeletonJointTransformation skelTrans;
        if (player.GetSkeletonJoint(SkeletonJoint.Torso, out skelTrans) == false)
        {
            // we don't have joint information so we simply return...
            return;
        }
        UpdateRoot(skelTrans.Position, centerOffset);

        // update each joint with data from OpenNI
        foreach (SkeletonJoint joint in Enum.GetValues(typeof(SkeletonJoint)))
        {
            SkeletonJointTransformation skelTransJoint;
            if(player.GetSkeletonJoint(joint,out skelTransJoint) == false)
                continue; // irrelevant joint
            UpdateJoint(joint, skelTransJoint, skelTrans.Position);
        }
    }

    /// @brief a utility method to update joint position 
    /// 
    /// This utility method receives a joint and unscaled position (x,y,z) and moves the joint there.
    /// it makes sure the joint has been attached and that scale is applied.
    /// @param joint The joint to update (the method makes sure it is legal)
    /// @param xPos The unscaled position along the x axis (scale will be applied)
    /// @param yPos The unscaled position along the y axis (scale will be applied)
    /// @param zPos The unscaled position along the z axis (scale will be applied)
    protected void UpdateJointPosition(SkeletonJoint joint, float xPos, float yPos, float zPos)
    {
        if(((int)joint)>=m_jointTransforms.Length || m_jointTransforms[(int)joint] == null)
            return; // an illegal joint
        Vector3 tmpPos = Vector3.zero;
        tmpPos.x = xPos;
        tmpPos.y = yPos;
        tmpPos.z = zPos;
        tmpPos *= m_scale;
        m_jointTransforms[(int)joint].localPosition = tmpPos;
    }

    /// @brief Utility method to calculate the rotation of a joint
    /// 
    /// This method receives joint information and calculates the rotation of the joint in Unity
    /// coordinate system.
    /// @param centerOffset the new central position
    /// @param joint the joint we want to calculate the rotation for
    /// @param skelTrans the new transformation of the joint
    /// @return the rotation of the joint in Unity coordinate system
    protected Quaternion CalcRotationForJoint(SkeletonJoint joint,ref SkeletonJointTransformation skelTrans, ref SkeletonJointPosition centerOffset)
    {
        // In order to convert the skeleton's orientation to Unity orientation we will
        // use the Quaternion.LookRotation method to create the relevant rotation Quaternion. 
        // for Quaternion.LookRotation to work it needs a "forward" vector and an "upward" vector.
        // These are generally the "Z" and "Y" axes respectively in the sensor's coordinate
        // system. The orientation received from the skeleton holds these values in their 
        // appropriate members.

        // Get the forward axis from "z".
        Point3D sensorForward = Point3D.ZeroPoint;
        sensorForward.X = skelTrans.Orientation.Z1;
        sensorForward.Y = skelTrans.Orientation.Z2;
        sensorForward.Z = skelTrans.Orientation.Z3;
        // convert it to Unity
        Vector3 worldForward = NIConvertCoordinates.ConvertPos(sensorForward);
        worldForward *= -1.0f; // because the Unity "forward" axis is opposite to the world's "z" axis.
        if (worldForward.magnitude == 0)
            return Quaternion.identity; // we don't have a good point to work with.
        // Get the upward axis from "Y".
        Point3D sensorUpward = Point3D.ZeroPoint;
        sensorUpward.X = skelTrans.Orientation.Y1;
        sensorUpward.Y = skelTrans.Orientation.Y2;
        sensorUpward.Z = skelTrans.Orientation.Y3;
        // convert it to Unity
        Vector3 worldUpwards = NIConvertCoordinates.ConvertPos(sensorUpward);
        if (worldUpwards.magnitude == 0)
            return Quaternion.identity; // we don't have a good point to work with.
        Quaternion jointRotation = Quaternion.LookRotation(worldForward, worldUpwards);

        Quaternion newRotation = transform.rotation * jointRotation * m_jointsInitialRotations[(int)joint];

        // we try to limit the speed of the change.
        return Quaternion.Slerp(m_jointTransforms[(int)joint].rotation, newRotation, Time.deltaTime * m_rotationDampening);
    }

    /// @brief Utility method to calculate the @b LOCAL position of a joint
    /// 
    /// This method receives joint information and calculates the @b LOCAL position rotation of the joint
    /// (compare to its parent transform) in Unity coordinate system.
    /// @param centerOffset the new central position
    /// @param joint the joint we want to calculate the position for
    /// @param skelTrans the new transformation of the joint
    /// @return the @b LOCAL position rotation of the joint (compare to its parent transform) in 
    /// Unity coordinate system
    protected Vector3 CalcJointPosition(SkeletonJoint joint, ref SkeletonJointTransformation skelTrans, ref SkeletonJointPosition centerOffset)
    {
        Vector3 v3pos=NIConvertCoordinates.ConvertPos(skelTrans.Position.Position);
        Vector3 v3Center = NIConvertCoordinates.ConvertPos(centerOffset.Position);
        v3pos -= v3Center;
        return v3pos*m_scale;
    }

    ///  the initial rotations of the joints we move everything compared to this.
    protected Quaternion[] m_jointsInitialRotations;

    /// the current root position of the game object (movement is based on this).
    protected Vector3 m_rootPosition;

    /// the original (placed) root position of the game object.
    protected Vector3 m_originalRootPosition;


}
