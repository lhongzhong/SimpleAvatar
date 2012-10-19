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

/// @brief base class for GUI cursors.
/// 
/// This is a class to define a cursor for NIGUI. The cursor is a game object
/// which moves tracking the hand.
/// @ingroup OpenNIGUIUtiliites
public abstract class NIGUICursor : MonoBehaviour 
{
    /// mono-behavior awake.
    /// @note inheriting classes should NOT create their own Awake function but rather override @ref InternalAwake
    public void Awake()
    {
        InternalAwake();
    }

    /// changes the active state of the cursor.
    /// @note an inactive cursor should be invisible and should not or move!
    /// @param state the active state (true means active)
    public abstract void SetActive(bool state);

    /// Tells us if a click occurred in the current frame.
    /// @note This is a problematic method! The problem is that one cannot be sure
    /// the click won't occur AFTER the update which caused the gesture to be detected.
    /// @return true if a click occurred this frame and false otherwise
    public abstract bool HasClickedThisFrame();

    /// This method tells us the last time, frame and position a click has occurred.
    /// This is useful because the click can occur any time inside the frame.
    /// @note in a gesture that is continuous (such as steady) this will be when the
    /// event was fired!
    /// @param[out] frameClicked the frame in which the click last occurred.
    /// @param[out] posClicked the position (in screen coordinates) the cursor was when the click occurred.
    /// @return the time the click last occurred (in Time.time)
    public abstract float GetLastClickedTime(out int frameClicked, out Vector2 posClicked);

    /// This method allows us to register for a callback for the clicking
    /// @note it is the responsibility of the caller to unregister using @ref UnRegisterCallbackForGesture
    /// @param eventDelegate the delegate to be called
    public abstract void RegisterCallbackForGesture(NIGestureTracker.GestureEventHandler eventDelegate);

    /// This method allows us to unregister a callback previously registered using @ref RegisterCallbackForGesture
    /// @param eventDelegate the delegate to be called
    public abstract void UnRegisterCallbackForGesture(NIGestureTracker.GestureEventHandler eventDelegate);

    /// returns the current position
    /// @note this is returned in screen coordinates!
    public abstract Vector2 Position
    {
        get;
    }


    /// returns the current position normalized
    /// @note this is returned between 0 and 1 in each dimension
    public abstract Vector2 PositionNormalized
    {
        get;
    }

    /// this is an internal awakening.
    /// @note it resets the groups of the GUI and therefore a new object should not be created in the middle
    /// of OnGUI.
    protected virtual void InternalAwake()
    {
        NIGUI.ResetGroups();
    }

    /// responsible for showing the cursor (override this instead of defining a new OnGUI)
    protected abstract void InternalOnGUI();

    /// mono-behavior OnGUI - used to show the cursor.
    public void OnGUI()
    {
        InternalOnGUI();
    }
}
