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
/// @ingroup OpenNIBasicObjects
[CustomEditor(typeof(NIQuery))]
public class NIQueryInspector : Editor
{
    /// editor OnInspectorGUI to control the NIEventLogger properties
    override public void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.LookLikeInspector();
        NIQuery query = target as NIQuery;

        if(query.Valid==false)
        {
            query.ForceInit();
        }
        if (query.m_queryDescriptions != null)
        {
            if (m_queryFoldout == null || m_queryFoldout.Length != query.m_queryDescriptions.Length)
            {
                m_queryFoldout = new bool[query.m_queryDescriptions.Length];
                for (int i = 0; i < m_queryFoldout.Length; i++)
                {
                    m_queryFoldout[i] = false;
                }
            }

            GUILayout.Label("Queries supported for " + query.m_queryDescriptions.Length + " nodes ");
            for (int i = 0; i < query.m_queryDescriptions.Length; i++)
            {
                m_queryFoldout[i] = EditorGUILayout.Foldout(m_queryFoldout[i], "" + query.m_queryDescriptions[i].m_nodeType + " nodes query");
                if (m_queryFoldout[i])
                {
                    EditorGUI.indentLevel += 2;
                    QueryDescription desc = query.m_queryDescriptions[i];
                    if (EditorApplication.isPlaying == false)
                    {
                        desc.m_nodeName = EditorGUILayout.TextField("Node name", desc.m_nodeName);
                        desc.m_vendorName = EditorGUILayout.TextField("Vendor name", desc.m_vendorName);
                        EditorGUILayout.LabelField("Min version: ", desc.RequiresMinVersion() ? "Limited" : "no limitation");
                        desc.GetMinVersionArr(ref m_Version);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        for (int j = 0; j < m_Version.Length; j++)
                        {
                            if (j != 0)
                            {
                                GUILayout.Label(".", GUILayout.MaxWidth(5));
                            }
                            m_Version[j] = EditorGUILayout.IntField(m_Version[j], GUILayout.MaxWidth(25));
                        }
                        EditorGUILayout.EndHorizontal();
                        desc.SetMinVersion(m_Version);

                        EditorGUILayout.LabelField("Max version: ", desc.RequiresMaxVersion() ? "Limited" : "no limitation");
                        desc.GetMaxVersionArr(ref m_Version);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        for (int j = 0; j < m_Version.Length; j++)
                        {
                            if (j != 0)
                            {
                                GUILayout.Label(".", GUILayout.MaxWidth(5));
                            }
                            m_Version[j] = EditorGUILayout.IntField(m_Version[j], GUILayout.MaxWidth(25));
                        }
                        EditorGUILayout.EndHorizontal();
                        desc.SetMaxVersion(m_Version);
                    }
                    ProductionNodeDescription curNodeDesc;
                    if (OpenNISettingsManager.GetProductionNodeInformation(desc.m_nodeType, out curNodeDesc))
                    {
                        EditorGUILayout.LabelField("Last node loaded was:", "");
                        EditorGUI.indentLevel += 2;
                        EditorGUILayout.LabelField("Node name", curNodeDesc.Name);
                        EditorGUILayout.LabelField("Vendor name", curNodeDesc.Vendor);
                        EditorGUILayout.LabelField("Version:", "" + curNodeDesc.Version.Major + "." + curNodeDesc.Version.Minor + "." + curNodeDesc.Version.Maintenance + "." + curNodeDesc.Version.Build);
                        EditorGUI.indentLevel -= 2;
                    }
                    else
                    {
                        GUILayout.Label("    Load node to see its info");
                    }

                    EditorGUI.indentLevel -= 2;
                    query.m_queryDescriptions[i] = desc;
                }
            }
        }
  
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }


    /// @brief Holds the current foldout of the open inspector
    static protected bool[] m_queryFoldout;

    /// @brief utility version to be filled on the fly
    protected static int[] m_Version = new int[4];
}
