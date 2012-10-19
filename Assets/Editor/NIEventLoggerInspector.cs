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
/// @ingroup OpenNIBasicObjects
[CustomEditor(typeof(NIEventLogger))]
public class NIEventLoggerInspector : Editor
{
    /// editor OnInspectorGUI to control the NIEventLogger properties
    override public void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.LookLikeInspector();
        NIEventLogger logger = target as NIEventLogger;
        EditorGUILayout.LabelField("Categories to show","");
        EditorGUI.indentLevel += 2;

        if (Initialized == false)
        {
            // this is aimed to make sure the categories and sources list will be updated when
            // the enums change. Therefore we use the static variable to make sure the test is done
            // only once and not every frame.
            if (logger.InitCategoriesList() || logger.InitSourcesList())
            {
                EditorUtility.SetDirty(target);
            }
            Initialized = true;
        }
        for (int i = 0; i < logger.m_categoriesToShow.Length; i++)
        {
            logger.m_categoriesToShow[i] = EditorGUILayout.Toggle("" + (NIEventLogger.Categories)i,logger.m_categoriesToShow[i]);
        }
        EditorGUI.indentLevel -= 2;
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Sources to show", "");
        EditorGUI.indentLevel += 2;
        for (int i = 0; i < logger.m_sourcesToShow.Length; i++)
        {
            logger.m_sourcesToShow[i] = EditorGUILayout.Toggle("" + (NIEventLogger.Sources)i, logger.m_sourcesToShow[i]);
        }
        logger.m_minLevelToShow = (NIEventLogger.VerboseLevel)EditorGUILayout.EnumPopup("Minimum log level", (System.Enum)logger.m_minLevelToShow);
        EditorGUI.indentLevel -= 2;
        EditorGUILayout.Space();

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    private bool Initialized = false; ///< @brief Used to make sure we initialize the sources and categories list of the logger.
}
