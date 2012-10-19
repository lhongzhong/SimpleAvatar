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
[CustomEditor(typeof(NIPlayerManager))]
public class NIPlayerManagerInspector : Editor
{
    /// editor OnInspectorGUI to control the NIEventLogger properties
    override public void OnInspectorGUI()
    {
        DrawPlayerManager();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    /// @brief Internal method to do the actual drawing. 
    /// 
    /// Inspectors for users selectors inheriting NIPlayerManager should inherit NIPlayerManagerInspector 
    /// and override this method. 
    /// @note for best performance, call base.DrawPlayerManager() and only then add the new behavior.
    protected virtual void DrawPlayerManager()
    {
        EditorGUIUtility.LookLikeInspector();
        NIPlayerManager manager = target as NIPlayerManager;
        GUILayout.Label(manager.GetSelectionString());
        EditorGUILayout.Space();
        manager.m_contextManager = EditorGUILayout.ObjectField("Context", manager.m_contextManager, typeof(OpenNISettingsManager), true) as OpenNISettingsManager;
        int maxPlayers = EditorGUILayout.IntField("Max allowed players", manager.m_MaxNumberOfPlayers);
        if(maxPlayers>0)
        {
            if (EditorApplication.isPlaying)
            {
                // we are running so we need to change manually
                manager.ChangeNumberOfPlayers(maxPlayers);
            }
            else
            {
                manager.m_MaxNumberOfPlayers = maxPlayers;
            }
        }
        int numRetries = EditorGUILayout.IntField("Num retries", manager.m_numRetries);
        if (numRetries > 0)
            manager.m_numRetries = numRetries;
    }
}
