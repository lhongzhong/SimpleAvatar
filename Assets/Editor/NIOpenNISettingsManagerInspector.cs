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


/// @brief A class to define the inspector of NIConfigurableContext
/// This class is responsible for adding various inspector capabilities to the NIConfigurableContext tool
/// @ingroup OpenNIBasicObjects
[CustomEditor(typeof(OpenNISettingsManager))]
public class NIOpenNISettingsManagerInspector : Editor
{
    /// holds the foldout of the user and skeleton control
    protected bool m_userAndSkeletonControlFoldout = true;

    /// internal GUIContent, this is used to easily add tool tips.
    protected GUIContent m_myContent = new GUIContent("string label", "tooltip");


    /// This method creates the inspector when the program is running. In this mode we
    /// only show information on what is initialized, valid etc. but allow no changes.
    protected void RunInspector()
    {
        // for easy access to the object
        OpenNISettingsManager OpenNISettings = target as OpenNISettingsManager;

        // basic test. If the object is invalid, nothing else matters.
        if(OpenNISettings.Valid==false)
        {
            EditorGUILayout.LabelField("NI was not initialized and therefore everything is invalid!", "");
            return;
        }
        // show the mirror capability (global one from the context). Note, we can change the mirroring (To
        // allow showing mirrored/unmirrored viewers in realtime) but this should NOT be used except for that
        // purpose
        OpenNISettings.Mirror = EditorGUILayout.Toggle("Mirror behavior ", OpenNISettings.Mirror);

        // show image generator status
        string str = "Not used";
        if (OpenNISettings.m_useImageGenerator)
        {
            str = OpenNISettings.ImageValid ? "Valid" : "Invalid";
        }
        EditorGUILayout.LabelField("Image Generator:", str);

        EditorGUILayout.Space();

        // show user and skeleton control status
        if (OpenNISettings.m_useSkeleton)
        {
            str = OpenNISettings.UserSkeletonValid ? "user and skeleton control is Valid" : "user and skeleton control is Invalid";
            m_userAndSkeletonControlFoldout = EditorGUILayout.Foldout(m_userAndSkeletonControlFoldout, str);
            if (m_userAndSkeletonControlFoldout && OpenNISettings.UserSkeletonValid)
            {
                EditorGUI.indentLevel += 2;
                int[] users = OpenNISettings.UserGenrator.GetUserIds();
                EditorGUILayout.LabelField("Identified " + users.Length + " users", "");
                for(int i=0; i<users.Length; i++)
                {
                    int userID = users[i];
                    Vector3 center = OpenNISettings.UserGenrator.GetUserCenterOfMass(userID);
                    EditorGUILayout.LabelField("User:", ""+i);
                    EditorGUI.indentLevel += 2;
                    EditorGUILayout.LabelField("user id:", ""+userID);
                    string state="Not yet calibrated";
                    if(OpenNISettings.UserGenrator.IsTracking(userID))
                    {
                        state="Tracking";
                    }
                    else if(OpenNISettings.UserGenrator.IsCalibrated(userID))
                    {
                        state="Calibrated";
                    }
                    else if(OpenNISettings.UserGenrator.IsCalibrating(userID))
                    {
                        state="Calibrating";
                    }
                    EditorGUILayout.LabelField("state:", "" + state);
                    EditorGUILayout.LabelField("center of mass:", "" + center);
                    EditorGUI.indentLevel -= 2;
                }
                EditorGUILayout.Space();
                OpenNISettings.SmoothFactor = EditorGUILayout.FloatField("Smoothing factor:", OpenNISettings.SmoothFactor);
                EditorGUI.indentLevel -= 2;
            }
        }
        else
        {
            EditorGUILayout.LabelField("user and skeleton control:", "Not used");
        }
        EditorGUILayout.Space();

    }

    /// This method creates the inspector when the program is NOT running. In this mode we
    /// allow initialization but do not show any running information
    protected void InitInspector()
    {
        // for easy access to the object
        OpenNISettingsManager OpenNISettings = target as OpenNISettingsManager;

        // set the default mirroring.
        OpenNISettings.m_initMirroring = EditorGUILayout.Toggle("Mirror behavior ", OpenNISettings.m_initMirroring);
        // chooses the xml file. In most implementations this should be empty!

        OpenNISettings.m_XMLFileName = EditorGUILayout.TextField("XML file", OpenNISettings.m_XMLFileName);
        // set image generator use.
        OpenNISettings.m_useImageGenerator = EditorGUILayout.Toggle("Use image generator?", OpenNISettings.m_useImageGenerator);

        // set skeleton generator use.
        // show user and skeleton control status
        OpenNISettings.m_useSkeleton = EditorGUILayout.Toggle("Use user and skeleton?", OpenNISettings.m_useSkeleton);
        if (OpenNISettings.m_useSkeleton && OpenNISettings.m_useSkeleton)
        {
            EditorGUI.indentLevel += 2;
            OpenNISettings.m_smoothingFactor = EditorGUILayout.FloatField("Smoothing factor:", OpenNISettings.m_smoothingFactor);
            EditorGUI.indentLevel -= 2;
        }

        // initialize the logger and query objects
        OpenNISettings.m_Logger = EditorGUILayout.ObjectField("Logger object", OpenNISettings.m_Logger, typeof(NIEventLogger), true) as NIEventLogger;
        OpenNISettings.m_query = EditorGUILayout.ObjectField("Query object", OpenNISettings.m_query, typeof(NIQuery), true) as NIQuery;
        m_myContent.text = "Playback filename";
        m_myContent.tooltip = "If this is not empty, it will hold the filename of a recording. NOTE: if XML is defined, this is ignored. See the documentation for more info";
        OpenNISettings.m_recordingFilename = EditorGUILayout.TextField(m_myContent, OpenNISettings.m_recordingFilename);
        m_myContent.text = "Reset floor normal";
        m_myContent.tooltip = "If floor normal is updated in the game, it remains between games. If a new normal should be calculated, reset it between games. Note: this is only relevant while playing again and again in the editor as the normal will automatically be reset whenever the program is started again";
        if (GUILayout.Button(m_myContent))
        {
            NIConvertCoordinates.ResetFloorNormal();
        }
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    /// implementation of the Editor inspector GUI (this is what is called to see the inspector).
    override public void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.LookLikeInspector();
        if(EditorApplication.isPlaying)
        {
            // we are running so we can only get some feedback
            RunInspector();
        }
        else 
        {
            // we are not running so we can initialize stuff
            InitInspector();
        }
        EditorGUI.indentLevel = 0;

        // this is part of making sure we update during running. This adds the update function
        // to the editor application update which is called 100 times per second.
        if (m_initialized == false)
        {
            EditorApplication.update += Update;
            m_initialized = true;
        }
    }

    /// if true we initialized and need to do nothing
    private bool m_initialized = false;


    /// used in order to make sure we update during running even if no movement occurs as long as
    /// the object is chosen.
    void Update()
    {
        // to make sure this updates when we are not in focus too..
        if (EditorApplication.isPlaying)
        {
            Repaint();
        }
    }
}
