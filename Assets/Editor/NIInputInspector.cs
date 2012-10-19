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


/// @brief A class to define the inspector of NIInput
/// 
/// This class is responsible for adding various inspector capabilities to @ref NIInput.
/// It provides the following information to configure:
/// - Hands tracker manager: a link to the hands tracker manager which tells us which points (or
///   hands) to track using the axis (this is similar to having a list of available joysticks).
/// - Gestures manager: a link to the gestures manager which tells us which gestures could be used
///   for the axes (this is similar to having a list of buttons on a joystick).
/// - An axis array: This array serves the exact same purpose as the (<A HREF="http://unity3d.com/support/documentation/Components/class-InputManager.html"> 
///   InputManager </A> provided by Unity.
/// - The axes themselves are based on the @ref NIAxis class and are designed to be as similar to 
///   InputManager axes as possible. The axes definitions include the following:
///     - @b Name: The name used for the axis (same as InputManager Name field, used as the identifier 
///       for GetAxis or GetAxisRaw). @note if multiple axes (between InputManager and NIInput) have 
///       the same name, the definition which returns the greatest absolute value will be used!.
///     - @b NIInputAxis?: This checkbox tells us if we need to check the regular input for the axis. 
///       If this is checked (true) then %NIInput will not try to get axis information for that axis 
///       from the regular Input class.
///     - <b>Descriptive name</b>: A short description of the axis(same as InputManager Descriptive 
///       Name field)
///     - @b Gesture: The gesture used to push the axis in the positive direction. The list of available
///       gestures is derived from the GestureManager. This comes instead of the key choice in 
///       InputManager. @note the gesture chosen must be a legal gesture for the hand tracker (
///       otherwise a runtime error would occure)!
///     - @b Dead: Size of the analog dead zone. All analog device values within this range result 
///       map to neutral (same as InputManager)
///     - @b Sensitivity: for gestures: Speed in units per second that the the axis will move toward the target value. 
///       for movement this is a scaling factor
///     - @b Invert: If true, all values are inverted (negative)
///     - @b Type: The type of input used. For a list of types (and their meaning) 
///       see @ref NIAxis::NIInputTypes
///     - @b Normalization: Defines a transformation of the tracked point's in the axis to [-1,1] 
///       range (clipped). -1 is anything smaller than the center minus m_maxMovement, +1 is anything
///       larger than the center plus m_maxMovement and the values in between are a linear change.
///       The center is defined based on the type (0 for HandMovement, focus point for 
///       HandMovementFromFocusPoint etc.). If the normalization value is 0 or negative then there 
///       will be no transformation (i.e. the axis value will be the position). 
///       @note the actual value is still multiplied by the sensitivity, changed to 0 in the dead
///       zone and can be inverted.    
///     - @b Axis: Which axes the type relates to (x,y,z). 
///     - <b>Tracker to use</b>: This tells us which tracker to use (very similar to the joystick 
///       number defined in InputManager. Unlike InputManager there is no "all joystick". The 
///       available values are extracted from the Hands tracker manager.
///       @note It is possible to have a value of None (which is the default and will occur if the
///       relevant tracker is removed from the hand tracker). This will result in a runtime error! 
///       the developer must make sure a valid value exits!
/// <br><br>
/// @see @ref NIInputConcept for more information.
/// @ingroup OpenNIInput
[CustomEditor(typeof(NIInput))]
[System.Serializable]
public class NIInputInspector : Editor
{
    /// used to save the array size before we press "enter" to actually do the change.
    protected int arraySize=-1;
    /// internal GUIContent, this is used to easily add tool tips.
    protected GUIContent m_myContent = new GUIContent("label", "tooltip");

    /// the list of gestures we can choose
    protected string[] m_gestureStrings = null;

    /// the list of hand trackers we can choose
    protected string[] m_pointTrackerStrings = null;

    /// initializes the list of gestures to use.
    protected void InitGestureStrings()
    {
        NIInput inp = target as NIInput;
        if(inp.m_gestureManager==null || inp.m_gestureManager.m_gestures==null)
        {
            if(m_gestureStrings==null || m_gestureStrings.Length!=1)
            {
                m_gestureStrings=new string[1];
                m_gestureStrings[0]="No gesture";
                return;
            }
            else
            {
                // we already have the gesture strings
                return;
            }
        }
        if (m_gestureStrings != null && m_gestureStrings.Length == inp.m_gestureManager.m_gestures.Length)
            return; // it is already initialized properly.
        m_gestureStrings = new string[inp.m_gestureManager.m_gestures.Length + 1];
        for (int i = 0; i < inp.m_gestureManager.m_gestures.Length; i++)
            m_gestureStrings[i] = inp.m_gestureManager.m_gestures[i].GetGestureType();
        m_gestureStrings[inp.m_gestureManager.m_gestures.Length] = "None";
    }

    /// initializes the list of point trackers to use.
    protected void InitPointTrackerStrings()
    {
        NIInput inp = target as NIInput;
        if (inp.m_pointsTrackingManager == null || inp.m_pointsTrackingManager.m_trackers == null)
        {
            if (m_pointTrackerStrings == null || m_pointTrackerStrings.Length != 1)
            {
                m_pointTrackerStrings = new string[1];
                m_pointTrackerStrings[0] = "No point trackers";
                return;
            }
            else
            {
                // we already have the gesture strings
                return;
            }
        }
        if (m_pointTrackerStrings != null && m_pointTrackerStrings.Length == inp.m_pointsTrackingManager.m_trackers.Length)
            return; // it is already initialized properly.
        m_pointTrackerStrings = new string[inp.m_pointsTrackingManager.m_trackers.Length + 1];
        for (int i = 0; i < inp.m_pointsTrackingManager.m_trackers.Length; i++)
            m_pointTrackerStrings[i] = inp.m_pointsTrackingManager.m_trackers[i].GetTrackerType();
        m_pointTrackerStrings[inp.m_pointsTrackingManager.m_trackers.Length] = "None";
    }

    /// utility method to fix the index of gestures after changes (assuming the string is always
    /// fine while the index might change because of changes done to the GestureManager).
    /// @param axis the axis to fix.
    protected void fixGestureIndex(NIAxis axis)
    {
        if(axis.m_gestureIndex<m_gestureStrings.Length && m_gestureStrings[axis.m_gestureIndex].CompareTo(axis.m_gestureString)==0)
            return; // we match
        // if we are not a "gesture" type, lets just make it the "none" gesture
        if(axis.m_Type != NIAxis.NIInputTypes.Gesture)
        {
            axis.m_gestureIndex=m_gestureStrings.Length-1;
            axis.m_gestureString=m_gestureStrings[axis.m_gestureIndex];
            return;
        }
        // we now know we are a gesture type so lets look for a match
        for (int i = 0; i < m_gestureStrings.Length; i++)
        {
            if (m_gestureStrings[i].CompareTo(axis.m_gestureString) == 0)
            {
                axis.m_gestureIndex = i;
                return; // we found a match
            }
        }
        // if we are here, the gesture no longer exist. We therefore will set ourselves  to not be a gesture
        // anymore but instead a hand movement type.
        axis.m_gestureIndex = m_gestureStrings.Length - 1;
        axis.m_Type = NIAxis.NIInputTypes.HandMovementFromStartingPoint;
        axis.m_gestureString = m_gestureStrings[axis.m_gestureIndex];
    }


    /// utility method to fix the index of point trackers after changes (assuming the string is always
    /// fine while the index might change because of changes done to the pointsTrackingManager).
    /// @param axis the axis to fix.
    protected void fixTrackerIndex(NIAxis axis)
    {
        if (axis.m_sourceTrackerIndex < m_pointTrackerStrings.Length && m_pointTrackerStrings[axis.m_sourceTrackerIndex].CompareTo(axis.m_sourceTrackerString) == 0)
            return; // we match
        for (int i = 0; i < m_pointTrackerStrings.Length; i++)
        {
            if (m_pointTrackerStrings[i].CompareTo(axis.m_sourceTrackerString) == 0)
            {
                axis.m_sourceTrackerIndex = i;
                return; // we found a match
            }
        }
        // if we are here, the tracker no longer exist. We therefore will set ourselves  
        // the "none" tracker 
        axis.m_sourceTrackerIndex = m_pointTrackerStrings.Length - 1;
        axis.m_sourceTrackerString = m_pointTrackerStrings[axis.m_sourceTrackerIndex];
    }


    /// implementation of the Editor inspector GUI (this is what is called to see the inspector).
    override public void OnInspectorGUI()
    {
		if(EditorApplication.isPlaying)
			return;
        InitGestureStrings();
        InitPointTrackerStrings();
        EditorGUIUtility.LookLikeInspector();
        NIInput inp = target as NIInput;
        EditorGUI.indentLevel = 0;
        m_myContent.text = "Hand trackers manager";
        m_myContent.tooltip = "The hands tracker manager which holds which hand tracking source we have";
        inp.m_pointsTrackingManager = EditorGUILayout.ObjectField(m_myContent, inp.m_pointsTrackingManager, typeof(NIPointTrackerManager), true) as NIPointTrackerManager;
        m_myContent.text = "Gestures manager";
        m_myContent.tooltip = "The gestures manager which holds which gestures we can use";
        inp.m_gestureManager = EditorGUILayout.ObjectField(m_myContent, inp.m_gestureManager, typeof(NIGestureManager), true) as NIGestureManager;

        m_myContent.text = "Axes";
        m_myContent.tooltip = "All the axes to use for Natural Interactions";
        inp.m_foldAllAxes = EditorGUILayout.Foldout(inp.m_foldAllAxes, m_myContent);
        if (inp.m_foldAllAxes == false)
            return;
        EditorGUI.indentLevel += 2;
        if (arraySize < 0)
            arraySize = inp.m_axisList.Count;
        m_myContent.text = "Size";
        m_myContent.tooltip = "The number of axes used";
        int numElements = EditorGUILayout.IntField(m_myContent, arraySize);
        arraySize = numElements;
        Event e = Event.current;
        if (e.keyCode == KeyCode.Return)
        {
            if (numElements == 0)
            {
                numElements = inp.m_axisList.Count; // don't allow to make it into 0 to avoid "deleting" to change something.
                arraySize = numElements;
            }
            if (numElements != inp.m_axisList.Count)
            {
                if (numElements < inp.m_axisList.Count)
                {
                    while (numElements < inp.m_axisList.Count)
                    {
                        inp.m_axisList.RemoveAt(inp.m_axisList.Count - 1);
                    }
                }
                else
                {
                    for (int i = inp.m_axisList.Count; i < numElements; i++)
                    {
                        NIAxis newAxis = new NIAxis();
                        newAxis.m_gestureIndex = m_gestureStrings.Length - 1; // initialize to none
                        inp.m_axisList.Add(newAxis);
                    }
                }
                List<bool> newFoldout = new List<bool>();
                int size = inp.m_foldAxisElement.Count;
                if (size > inp.m_axisList.Count)
                    size = inp.m_axisList.Count;
                for (int i = 0; i < size; i++)
                {
                    newFoldout.Add(inp.m_foldAxisElement[i]);
                }
                for (int i = size; i < inp.m_axisList.Count; i++)
                    newFoldout.Add(false);
                inp.m_foldAxisElement = newFoldout;
            }
        }
        
        for (int i = 0; i < inp.m_foldAxisElement.Count; i++)
        {
            inp.m_foldAxisElement[i] = EditorGUILayout.Foldout(inp.m_foldAxisElement[i], inp.m_axisList[i].m_axisName);
            if(inp.m_foldAxisElement[i]==false)
                continue;
            EditorGUI.indentLevel += 2;
            m_myContent.text = "Name";
            m_myContent.tooltip = "Change the name of the current axis";
            inp.m_axisList[i].m_axisName = EditorGUILayout.TextField(m_myContent, inp.m_axisList[i].m_axisName);
            m_myContent.text = "NIInput axis?";
            m_myContent.tooltip = "if checked, the axis is used only by NIInput, if not, it is also used by the input manager";
            inp.m_axisList[i].m_NIInputAxisOnly = EditorGUILayout.Toggle(m_myContent, inp.m_axisList[i].m_NIInputAxisOnly);
            m_myContent.text = "Descriptive Name";
            m_myContent.tooltip = "Name presented to the user if relevant";
            inp.m_axisList[i].m_descriptiveName = EditorGUILayout.TextField(m_myContent, inp.m_axisList[i].m_descriptiveName);
            fixGestureIndex(inp.m_axisList[i]);
            inp.m_axisList[i].m_gestureIndex = EditorGUILayout.Popup("Gesture", inp.m_axisList[i].m_gestureIndex,m_gestureStrings);
            inp.m_axisList[i].m_gestureString = m_gestureStrings[inp.m_axisList[i].m_gestureIndex];
            if (inp.m_axisList[i].m_gestureIndex != m_gestureStrings.Length - 1)
                inp.m_axisList[i].m_Type = NIAxis.NIInputTypes.Gesture; // to make sure we use either
            m_myContent.text = "Dead";
            m_myContent.tooltip = "Movement of less than this value (when changes are used) will be ignored and considered 0";
            float deadZone=EditorGUILayout.FloatField(m_myContent,inp.m_axisList[i].m_deadZone);
            inp.m_axisList[i].m_deadZone = deadZone < NIAxis.m_minDeadZone ? NIAxis.m_minDeadZone : deadZone;
            m_myContent.text = "Sensitivity";
            m_myContent.tooltip = "A scaling factor";
            float sensitivity = EditorGUILayout.FloatField(m_myContent, inp.m_axisList[i].m_sensitivity);
            inp.m_axisList[i].m_sensitivity = sensitivity < NIAxis.m_minSensitivity ? NIAxis.m_minSensitivity : sensitivity;
            m_myContent.text = "Invert";
            m_myContent.tooltip = "Flip the values between positive and negative";
            inp.m_axisList[i].m_invert = EditorGUILayout.Toggle(m_myContent, inp.m_axisList[i].m_invert);
            m_myContent.text = "Type";
            m_myContent.tooltip = "The type of movement to do";
            inp.m_axisList[i].m_Type = (NIAxis.NIInputTypes)EditorGUILayout.EnumPopup(m_myContent, (System.Enum)inp.m_axisList[i].m_Type);
            if (inp.m_axisList[i].m_Type != NIAxis.NIInputTypes.Gesture)
            {
                inp.m_axisList[i].m_gestureIndex = m_gestureStrings.Length - 1; // make it a "none" gesture
            }
            m_myContent.text = "Normalization";
            m_myContent.tooltip = "Defines a transformation of the hand's axis to [-1,1] range (clipped).";
            m_myContent.tooltip += " -1 is anything smaller than the center minus m_maxMovement,";
            m_myContent.tooltip += " +1 is anything larger than the center plus m_maxMovement";
            m_myContent.tooltip += " and the values in between are a linear change.";
            m_myContent.tooltip += " The center is defined based on the type (0 for HandMovement, focus point";
            m_myContent.tooltip += " for HandMovementFromFocusPoint etc.). If m_maxMovement is 0 or negative";
            m_myContent.tooltip += " then there will be no  transformation. \n";
            m_myContent.tooltip += "Note the actual value is still multiplied by the sensitivity, ";
            m_myContent.tooltip += " changed to 0 in the dead zone and can base inverted. ";
            inp.m_axisList[i].m_maxMovement = EditorGUILayout.FloatField(m_myContent, inp.m_axisList[i].m_maxMovement);

            m_myContent.text = "Axis";
            m_myContent.tooltip = "Axis to use";
            inp.m_axisList[i].m_axisUsed = (NIAxis.AxesList)EditorGUILayout.EnumPopup(m_myContent, (System.Enum)inp.m_axisList[i].m_axisUsed);
            fixTrackerIndex(inp.m_axisList[i]);
            inp.m_axisList[i].m_sourceTrackerIndex = EditorGUILayout.Popup("Tracker to use", inp.m_axisList[i].m_sourceTrackerIndex, m_pointTrackerStrings);
            inp.m_axisList[i].m_sourceTrackerString = m_pointTrackerStrings[inp.m_axisList[i].m_sourceTrackerIndex];
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 2;
        }
        EditorGUI.indentLevel -= 2;
        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space();
        if(GUI.changed)
		    EditorUtility.SetDirty(target);
    }
}
