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


/// @brief A class to define the inspector of NIPlayerManager
/// 
/// This class is responsible for adding various inspector capabilities to the player manager
/// @ingroup UserSelectionModule
[CustomEditor(typeof(NIPlayerManagerPoseSelection))]
public class NIPlayerManagerPoseSelectionInspector : NIPlayerManagerInspector
{
    protected override void DrawPlayerManager()
    {
        base.DrawPlayerManager();
        NIPlayerManagerPoseSelection manager = target as NIPlayerManagerPoseSelection;
        string[] legalPoses = NIUserAndSkeleton.GetLegalPoses();

        if (legalPoses != null)
        {
            int selectedIndex;
            if (manager.m_PoseToSelect == null)
            {
                selectedIndex = 0;
            }
            else for (selectedIndex = 0; selectedIndex < legalPoses.Length; selectedIndex++)
            {
                if (manager.m_PoseToSelect.CompareTo(legalPoses[selectedIndex])==0)
                    break;
            }
            if (selectedIndex >= legalPoses.Length)
                selectedIndex = 0;
            selectedIndex = EditorGUILayout.Popup("pose to select", selectedIndex, legalPoses);
            manager.m_PoseToSelect = legalPoses[selectedIndex];

            bool useUnselectionPose = manager.m_PoseToUnselect != null && manager.m_PoseToUnselect.CompareTo("")!=0;
            useUnselectionPose = EditorGUILayout.Toggle("Use unselection pose", useUnselectionPose);
            if (useUnselectionPose == false)
            {
                manager.m_PoseToUnselect = "";
            }
            else
            {
                if (manager.m_PoseToUnselect == null)
                {
                    selectedIndex = 0;
                }
                else for (selectedIndex = 0; selectedIndex < legalPoses.Length; selectedIndex++)
                {
                    if (manager.m_PoseToUnselect.CompareTo(legalPoses[selectedIndex])==0)
                        break;
                }
                if (selectedIndex >= legalPoses.Length)
                    selectedIndex = 0;
                selectedIndex = EditorGUILayout.Popup("Pose to unselect", selectedIndex, legalPoses);
                manager.m_PoseToUnselect = legalPoses[selectedIndex];
            }
        }
        else
        {
            EditorGUILayout.LabelField("Pose to Select", manager.m_PoseToSelect);
            EditorGUILayout.LabelField("Pose to Unselect", manager.m_PoseToUnselect);
        }

        manager.m_timeToSwitch = EditorGUILayout.FloatField("Time between switching", manager.m_timeToSwitch);
        if (manager.m_timeToSwitch < 0.0f)
            manager.m_timeToSwitch = 0;

        if (EditorApplication.isPlaying == false)
        {
            if (GUILayout.Button("Update legal poses (might take some time)"))
            {
                OpenNISettingsManager.InspectorReloadAnInstance();
            }
        }

    }
}

 