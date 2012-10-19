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
[CustomEditor(typeof(NIPlayerManagerCOMSelection))]
public class NIPlayerManagerCOMSelectionInspector : NIPlayerManagerInspector 
{

    /// editor OnInspectorGUI to control the NIEventLogger properties
    override public void OnInspectorGUI()
    {
        DrawPlayerManager();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    protected override void DrawPlayerManager()
    {
        base.DrawPlayerManager();
        NIPlayerManagerCOMSelection manager = target as NIPlayerManagerCOMSelection;
        manager.m_maxAllowedDistance = EditorGUILayout.FloatField("Max distance", manager.m_maxAllowedDistance);
        if (manager.m_maxAllowedDistance < 0)
            manager.m_maxAllowedDistance = 0;
        manager.m_failurePenalty = EditorGUILayout.FloatField("Failure penalty", manager.m_failurePenalty);
        if (manager.m_failurePenalty < 0)
            manager.m_failurePenalty = 0;
        manager.m_minCOMChangeForFailure = EditorGUILayout.FloatField("Min change to retry", manager.m_minCOMChangeForFailure);
        if (manager.m_minCOMChangeForFailure < 0)
            manager.m_minCOMChangeForFailure = 0;
        manager.m_hysteresis = EditorGUILayout.FloatField("Hysteresis", manager.m_hysteresis);
        if (manager.m_hysteresis < 0)
            manager.m_hysteresis = 0;

    }

}
