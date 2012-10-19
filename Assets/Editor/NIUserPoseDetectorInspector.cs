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
using System;
using OpenNI;


/// @brief A class to define the inspector of NIEventLogger
/// 
/// This class is responsible for adding various inspector capabilities to the NIEventLogger tool
/// @ingroup OpenNIGestureTrackers
[CustomEditor(typeof(NIUserPoseGestureFactory))]
public class NIUserPoseDetectorInspector : Editor
{
    /// editor OnInspectorGUI to control the NIEventLogger properties
    override public void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.LookLikeInspector();
        NIUserPoseGestureFactory detector = target as NIUserPoseGestureFactory;

        string[] legalPoses = NIUserAndSkeleton.GetLegalPoses();

        if (legalPoses != null)
        {
            int selectedIndex;
            if (detector.m_poseName == null)
            {
                selectedIndex = 0;
            }
            else for (selectedIndex = 0; selectedIndex < legalPoses.Length; selectedIndex++)
            {
                if (detector.m_poseName.CompareTo(legalPoses[selectedIndex])==0)
                    break;
            }
            if (selectedIndex >= legalPoses.Length)
                selectedIndex = 0;
            selectedIndex = EditorGUILayout.Popup("pose to detect", selectedIndex, legalPoses);
            detector.m_poseName = legalPoses[selectedIndex];
        }
        else
        {
            EditorGUILayout.LabelField("Pose to detect", detector.m_poseName);
        }

        detector.m_timeToHoldPose = EditorGUILayout.FloatField("Time to hold pose", detector.m_timeToHoldPose);
        if (detector.m_timeToHoldPose < 0)
            detector.m_timeToHoldPose = 0;

        detector.m_Context = EditorGUILayout.ObjectField("Context", detector.m_Context, typeof(OpenNISettingsManager), true) as OpenNISettingsManager;


        if (EditorApplication.isPlaying == false)
        {
            if (GUILayout.Button("Update legal poses (might take some time)"))
            {
                OpenNISettingsManager.InspectorReloadAnInstance();
            }
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
