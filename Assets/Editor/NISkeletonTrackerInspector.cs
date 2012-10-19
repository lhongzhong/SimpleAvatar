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


/// @brief A class to define the inspector of NIEventLogger
/// 
/// This class is responsible for adding various inspector capabilities to the NIEventLogger tool
/// @ingroup OpenNIPointTrackers
[CustomEditor(typeof(NISkeletonTracker))]
public class NISkeletonTrackerInspector : Editor
{
    /// @brief Called when the inspector is initialized
    public void OnEnable()
    {
        NISkeletonTracker tracker = target as NISkeletonTracker;
        if (tracker == null)
        {
            m_lastDescription = "Error";
        }
        else
        {
            m_lastDescription = tracker.GetTrackerType();
        }
        m_style = new GUIStyle();
        m_style.wordWrap = true;
        m_style.fontStyle = FontStyle.Normal;
        m_content = new GUIContent();
        
    }

    /// editor OnInspectorGUI to control the NIEventLogger properties
    override public void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.LookLikeInspector();
        NISkeletonTracker tracker = target as NISkeletonTracker;
        
        GUILayout.Label("Tracker description:  ");
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.Label(m_lastDescription,m_style);
        GUILayout.EndHorizontal();

        if (m_lastDescription.CompareTo(tracker.GetTrackerType()) != 0)
        {
            EditorGUILayout.Space();
            Color tmpColor = m_style.normal.textColor;
            m_style.normal.textColor = Color.red;
            m_content.text="Tracker description changed! Please relink all references";
            m_content.tooltip="When objects reference the tracker (such as NIInput), they use the description to choose between the various options. When the description changes, the linking is lost. You need to relink all references unless you return the description to the previous definition by changing the player and joint";
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label(m_content,m_style);
            GUILayout.EndHorizontal();
            m_style.normal.textColor = tmpColor;
            
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Who to track","");
        EditorGUI.indentLevel += 2;
        tracker.m_playerNum = EditorGUILayout.IntField("Player Num", tracker.m_playerNum);
        if (tracker.m_playerNum < 0)
            tracker.m_playerNum = 0;
        tracker.m_jointToUse = (SkeletonJoint)EditorGUILayout.EnumPopup("Joint to track", (System.Enum)tracker.m_jointToUse);
        EditorGUI.indentLevel -= 2;
        tracker.m_context = EditorGUILayout.ObjectField("context", tracker.m_context, typeof(OpenNISettingsManager), true) as OpenNISettingsManager;
        tracker.m_playerManager = EditorGUILayout.ObjectField("Player manager", tracker.m_playerManager, typeof(NIPlayerManager), true) as NIPlayerManager;
        tracker.m_startingPoseReferenceType = (NISkeletonTracker.StartingPosReferenceType)EditorGUILayout.EnumPopup("StartPos type", (System.Enum)tracker.m_startingPoseReferenceType);
        EditorGUI.indentLevel += 2;
        switch (tracker.m_startingPoseReferenceType)
        {
            case NISkeletonTracker.StartingPosReferenceType.SetPointReference:
                tracker.m_StartPosModifier = EditorGUILayout.Vector3Field("StartPos", tracker.m_StartPosModifier);
                break;
            case NISkeletonTracker.StartingPosReferenceType.TrackedJointReference:
                EditorGUILayout.LabelField("StartPos=ref of", "" + tracker.m_jointToUse);
                break;
            case NISkeletonTracker.StartingPosReferenceType.StaticModifierToOtherJoint:
                tracker.m_referenceJoint = (SkeletonJoint)EditorGUILayout.EnumPopup("cur position of", (System.Enum)tracker.m_referenceJoint);
                tracker.m_StartPosModifier = EditorGUILayout.Vector3Field("Modified by", tracker.m_StartPosModifier);
                break;
            case NISkeletonTracker.StartingPosReferenceType.ScaledModifierToOtherJoint:
                tracker.m_referenceJoint = (SkeletonJoint)EditorGUILayout.EnumPopup("cur position of", (System.Enum)tracker.m_referenceJoint);
                tracker.m_StartPosModifier = EditorGUILayout.Vector3Field("Modified by", tracker.m_StartPosModifier);
                EditorGUILayout.LabelField("scaled by distance", "between");
                EditorGUI.indentLevel += 2;
                tracker.m_referenceJointScale1 = (SkeletonJoint)EditorGUILayout.EnumPopup("joint1", (System.Enum)tracker.m_referenceJointScale1);
                tracker.m_referenceJointScale2 = (SkeletonJoint)EditorGUILayout.EnumPopup("joint2", (System.Enum)tracker.m_referenceJointScale2);
                EditorGUI.indentLevel -= 2;
                break;

        }
        EditorGUI.indentLevel -= 2;
        EditorGUILayout.Space();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    private string m_lastDescription; ///< @brief Holds the last description since initialization (used to check when to give a relink warning)
    private GUIStyle m_style = null; ///< @brief internal GUIStyle to use
    private GUIContent m_content = null; ///< @brief internal GUIContent to use
}
