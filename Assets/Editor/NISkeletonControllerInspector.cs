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

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using OpenNI;
using System;


/// @brief A class to define the inspector of NISkeletonController
/// 
/// This class is responsible for adding various inspector capabilities to the NISkeletonController object
/// @ingroup SkeletonBaseObjects
[CustomEditor(typeof(NISkeletonController))]
public class NISkeletonControllerInspector : Editor
{
    /// editor OnInspectorGUI to control the NIEventLogger properties
    override public void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.LookLikeInspector();
        NISkeletonController controller = target as NISkeletonController;
        EditorGUILayout.LabelField("Controlling player", "");
        EditorGUI.indentLevel += 2;
        controller.m_playerManager = EditorGUILayout.ObjectField("Player manager", controller.m_playerManager, typeof(NIPlayerManager), true) as NIPlayerManager;
        controller.m_playerNumber = EditorGUILayout.IntField("Player Number", controller.m_playerNumber);
        EditorGUI.indentLevel -= 2;
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Joints to control","");
        EditorGUI.indentLevel += 2;
        // make sure the joint transforms are initialized.
        if(controller.m_jointTransforms==null)
        {
            controller.m_jointTransforms=new Transform[Enum.GetNames(typeof(SkeletonJoint)).Length + 1];
            for(int i=0; i<controller.m_jointTransforms.Length; i++)
                controller.m_jointTransforms[i]=null;
        }
        else if(controller.m_jointTransforms.Length!=Enum.GetNames(typeof(SkeletonJoint)).Length + 1) 
        {
            // the skeleton joints enum changed since last time...
            Transform[] newVal=new Transform[Enum.GetNames(typeof(SkeletonJoint)).Length + 1];
            for(int i=0; i<newVal.Length; i++)
            {
                if(i<controller.m_jointTransforms.Length)
                    newVal[i]=controller.m_jointTransforms[i];
                else
                    newVal[i]=null;
            }
            controller.m_jointTransforms=newVal;
        }
        foreach(SkeletonJoint joint in Enum.GetValues(typeof(SkeletonJoint)))
        {
            controller.m_jointTransforms[(int)joint] = EditorGUILayout.ObjectField(""+joint, controller.m_jointTransforms[(int)joint], typeof(Transform), true) as Transform;
        }
        EditorGUI.indentLevel -= 2;
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("What to update", "");
        EditorGUI.indentLevel += 2;
        controller.m_updateRootPosition = EditorGUILayout.Toggle("Update Root Positions?", controller.m_updateRootPosition);
        controller.m_updateJointPositions = EditorGUILayout.Toggle("Update Joint Positions?",controller.m_updateJointPositions);
        controller.m_updateOrientation = EditorGUILayout.Toggle("Update joint Orientation?",controller.m_updateOrientation);
        EditorGUI.indentLevel -= 2;
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Scaling & limitations", "");
        EditorGUI.indentLevel += 2;
        controller.m_rotationDampening = EditorGUILayout.FloatField("Rotation Dampening", controller.m_rotationDampening);
        controller.m_scale = EditorGUILayout.FloatField("Scale", controller.m_scale);
        controller.m_speed = EditorGUILayout.FloatField("Torso speed scale", controller.m_speed);
        EditorGUI.indentLevel -= 2;

        EditorGUILayout.Space();
        controller.m_linesDebugger = EditorGUILayout.ObjectField("Lines debugger", controller.m_linesDebugger, typeof(NISkeletonControllerLineDebugger), true) as NISkeletonControllerLineDebugger;

        EditorGUILayout.Space();

        EditorGUI.indentLevel -= 2;
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
