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
using UnityEngine;
using System.Collections;

/// @brief Input axis definitions
/// 
/// This class is responsible for the definition of an axis in the @ref NIInput
/// @note Since Unity does not export its InputManager and axis information, this
/// class is used to simulate it for Natural Interactions inputs. <br> In addition the axis
/// is not defined 100% the same. This is because we only implement the portions relating
/// to the Natural Interactions and thereby some of the axis information (such as negative button)
/// is irrelevant and some new elements are included.
/// 
/// @note There are some changes compared to the regular input:
///     - Alternative gestures (compared to alt-buttons) are not used because the new axis will only handle very few 
///     gestures at first.
///     - Gravity will not be used as the gestures are assumed to create "jump" or "fire" style events (i.e. gestures
///     will not be used to move a little because they take too long). For hand tracking, gravity is not really
///     required because the hand has its own gravity
///     - Snap is not used for the same reason gravity is not used.
///     - Negative description and button is not used for the same reason gravity is not used.
///     - Area clipping was added because unlike a joystick or mouse, the hand cannot continue to move
///     forever.
///     - The hand number is relevant only when skeleton tracking is done, in which case the right hand is
///     the primary hand and the left hand is the secondary hand. If focusing is used (without skeleton)
///     then the primary hand is the one which performed the focus gesture.
///     - If one wants to do some gestures on the right hand and some on the left hand then two axes need to
///     be defined (although both can be done with the same name). Similarly either gestures or tracking
///     can be used in a single axis but not both.
/// @ingroup OpenNIInput
[System.Serializable]
public class NIAxis 
{
    /// The name used for the axis (when doing GetAxis or GetAxisRaw)
    /// @note for it to work with existing axis the name must be identical!
    /// @note same as InputManager Name field
    public string m_axisName;

    /// if this is true the axis is used in NIAxis only, otherwise it is also used in
    /// the regular input.
    public bool m_NIInputAxisOnly;

    /// A short description of the axis
    /// @note same as InputManager Descriptive Name field.
    public string m_descriptiveName;

    /// The gesture used to push the axis in the positive direction. The list of available gestures is
    /// derived from the GestureManager
    public int m_gestureIndex;

    /// the string used for the gesture (to restore the index properly by the inspector after changing the GestureManager.
    public string m_gestureString;

     /// Size of the analog dead zone. All analog device values within this range result map to neutral.
    /// @note same as InputManager 
    public float m_deadZone;

    /// holds the minimum allowed dead zone (below that we don't have the resolution...)
    public const float m_minDeadZone = 0.001f;

    /// for gestures: Speed in units per second that the the axis will move toward the target value. 
    /// for movement this is a scaling factor
    public float m_sensitivity;

    /// holds the minimum allowed sensitivity (below that we don't have the resolution...)
    public const float m_minSensitivity = 0.0001f;

    /// if true, all values are inverted (negative)
    public bool m_invert;
    
    /// defines the types of inputs we support
    public enum NIInputTypes
    {
        HandMovement,                ///< a movement in the 3 axes (X,Y,Z). The result is a normalized value between -1 and 1 (corrected by dead zone, sensitivity etc.)
        DeltaHandMovement,           ///< the delta (dX and dY from last frame) in the 3D world. This is @b NOT normalized between -1 and 1 
        HandMovementFromStartingPoint,  ///< a delta from the focus point (the point in which the session started for that hand). The result is a normalized value between -1 and 1 (corrected by dead zone, sensitivity etc.)
        Gesture                      ///< this means we are using gestures, not movement
    }

    /// The type of input used
    public NIInputTypes m_Type;

    /// Defines a transformation of the hand's axis to [-1,1] range (clipped). -1 is anything smaller than
    /// the center minus m_maxMovement, +1 is anything larger than the center plus m_maxMovement and the values
    /// in between are a linear change. The center is defined based on the type (0 for HandMovement, focus point 
    /// for HandMovementFromFocusPoint etc.). If m_maxMovement is 0 or negative then there will be no 
    /// transformation. 
    /// @note the actual value is still multiplied by the sensitivity, changed to 0 in the dead zone and can
    /// be inverted.    
    public float m_maxMovement;

    /// the axes list
    public enum AxesList { xAxis, yAxis, zAxis };
    /// which axes the type relates to
    public AxesList m_axisUsed;



    /// This is the axis number (as received from the InputHandsManager).
    /// @note this replaces the joystick num option on InputManager. Unlike InputManager there is no "all joystick"
    /// option as we need to have a specific hand to track.
    public int m_sourceTrackerIndex;

    /// This is the axis string (as received from the InputHandsManager).
    /// @note this replaces the joystick num option on InputManager. Unlike InputManager there is no "all joystick"
    /// option as we need to have a specific hand to track.
    public string m_sourceTrackerString;

    /// this holds the source tracker used by the axis to get information.
    public NIPointTracker m_sourceTracker;

    /// this holds the source gesture used by the axis to get gesture information if relevant.
    public NIGestureTracker m_sourceGesture;

    public NIAxis()
    {
        m_axisName="New Axis";
        m_descriptiveName="";
        m_gestureIndex = 0;
        m_deadZone=m_minDeadZone;
        m_sensitivity=m_minSensitivity;
        m_invert=false;
        m_Type=NIInputTypes.HandMovement;
        m_maxMovement=-1.0f;
        m_axisUsed=AxesList.xAxis;
        m_sourceTrackerIndex=0;
        m_sourceTrackerString = "none";
        m_sourceTracker = null;
        m_sourceGesture = null;
        m_NIInputAxisOnly=true;
    }
}
