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

/// @brief A default implementation for the @ref NIGUICursor.
/// 
/// This class is an implementation of the cursor which uses the input to control the cursor.
/// It uses NIGUI_X to control the x access, NIGUI_Y to control the y access and NIGUI_CLICK
/// to choose stuff. It requires the following:
/// - NIGUI_X and NIGUI_Y should return a normalized value between -0.5 and 0.5.
/// - we will respond to the NIGestureTracker.GestureEventHandler event and therefore the click
/// will only occur at that time.
/// @ingroup OpenNIGUIUtiliites
public class DefaultNIGUICursor : NIGUICursor 
{
    /// the input object so we can get the data from it.
    /// @note this class uses NIGUI_X to control the x access, NIGUI_Y to control the y access
    /// and NIGUI_CLICK to choose stuff. It requires the following:
    /// - NIGUI_X and NIGUI_Y should return a normalized value between -0.5 and 0.5.
    /// - we will respond to the NIGestureTracker.GestureEventHandler event and therefore the click
    /// will only occur at that time.
    /// - NIGUI_CLICK must be normalized to 1
    /// - HasClickedThisFrame will return true if NIGUI_CLICK axis returns 1
    public NIInput m_input;

    protected override void  InternalAwake()
    {
        base.InternalAwake();
        if (m_input == null)
            m_input = FindObjectOfType(typeof(NIInput)) as NIInput;
        m_lastClickFrame = -1;
        m_lastClickPos = Vector2.zero;
        m_lastClickTime = -1.0f;
        int rectSize=15;
        m_positionRect = new Rect(0, 0, rectSize, rectSize);
        m_texture = new Texture2D(1, 1);
        Color[] color = new Color[1];
        color[0] = Color.green;
        m_texture.SetPixels(color);
        m_texture.Apply();
        m_active = true;
    }


    /// Tells us if a click occurred in the current frame.
    /// @note This is a problematic method! The problem is that one cannot be sure
    /// the click won't occur AFTER the update which caused the gesture to be detected.
    /// @return true if a click occurred this frame and false otherwise
    public override bool HasClickedThisFrame()
    {
        if (m_active == false)
            return false;
        return m_input.GetAxis("NIGUI_CLICK")>=1.0f;
    }



    /// This method tells us the last time, frame and position a click has occurred.
    /// This is useful because the click can occur any time inside the frame.
    /// @param[out] frameClicked the frame in which the click last occurred.
    /// @param[out] posClicked the position (in screen coordinates) the cursor was when the click occurred.
    /// @return the time the click last occurred (in Time.time)
    public override float GetLastClickedTime(out int frameClicked, out Vector2 posClicked)
    {
        if (m_active == false)
        {
            frameClicked = -100;
            posClicked = Vector2.zero;
            return -100.0f;
        }

        frameClicked = m_lastClickFrame;
        posClicked = m_lastClickPos;
        return m_lastClickTime;
    }


    /// This method allows us to register for a callback for the clicking
    /// @note it is the responsibility of the caller to unregister using @ref UnRegisterCallbackForGesture
    /// @param eventDelegate the delegate to be called
    public override void RegisterCallbackForGesture(NIGestureTracker.GestureEventHandler eventDelegate)
    {
        m_input.RegisterCallbackForGesture(eventDelegate, "NIGUI_CLICK");
    }

    /// This method allows us to unregister a callback previously registered using @ref RegisterCallbackForGesture
    /// @param eventDelegate the delegate to be called
    public override void UnRegisterCallbackForGesture(NIGestureTracker.GestureEventHandler eventDelegate)
    {
        m_input.UnRegisterCallbackForGesture(eventDelegate, "NIGUI_CLICK");
    }

    /// returns the current position
    /// @note this is returned in screen coordinates!
    public override Vector2 Position
    {
        get 
        {
            Vector2 res = PositionNormalized;
            // transform to screen coordinates.
            res.x *= Screen.width;
            res.y *= Screen.height;
            return res;
        }
    }

    /// returns the current position normalized
    /// @note this is returned between 0 and 1 in each dimension
    public override Vector2 PositionNormalized
    {
        get 
        {
            Vector2 res = Vector2.zero;
            // we need to add 0.5 to change the range from -0.5 to 0.5 to a range from 0 to 1.
            res.x = m_input.GetAxis("NIGUI_X")+0.5f; 
            res.y = m_input.GetAxis("NIGUI_Y")+0.5f; // the screen y axis is opposite to the camera's
            return res;
        }
    }

    /// to make sure we unregister
    public void OnDestroy()
    {
        m_active = false;
        if (m_registeredClick)
        {
            m_input.UnRegisterCallbackForGesture(GestureEventHandler, "NIGUI_CLICK");
            m_registeredClick = false;
        }
    }

    /// changes the active state of the cursor.
    /// @note an inactive cursor should be invisible and should not or move!
    /// @param state the active state (true means active)
    public override void SetActive(bool state)
    {
        m_active = state;
    }

    /// responsible for showing the cursor (override this instead of defining a new OnGUI)
    protected override void InternalOnGUI()
    {
        if (m_active == false)
        {
            return; // we are invisible
        }

        if (m_registeredClick == false)
        {
            m_input.RegisterCallbackForGesture(GestureEventHandler, "NIGUI_CLICK");
            m_registeredClick = true;
        }
        m_positionRect.x = Position.x - (m_positionRect.width/2);
        m_positionRect.y = Position.y - (m_positionRect.height / 2);
        float clickVal=m_input.GetAxis("NIGUI_CLICK");
        GUI.depth = 0;
        GUI.Box(m_positionRect, "");
        if (clickVal > 0)
        {
            float width = m_positionRect.width;
            m_positionRect.width *= clickVal;
            GUI.DrawTexture(m_positionRect, m_texture,ScaleMode.StretchToFill);
            //GUI.Box(m_positionRect, m_texture);
            m_positionRect.width = width;
        }
    }

    /// holds the time of the last click (from Time.time)
    protected float m_lastClickTime;
    /// holds the frame of the last click 
    protected int m_lastClickFrame;
    /// holds the position of the last click 
    protected Vector2 m_lastClickPos;

    /// holds true if we are active, false otherwise
    protected bool m_active;

    /// a rect used for the GUI
    private Rect m_positionRect;

    /// a texture to create a progress bar
    private Texture2D m_texture;

    /// if this is true we already registered to receive the click.
    private bool m_registeredClick;

    /// a callback to get the click event.
    /// 
    /// @param hand The point tracker which is the basis of the movement.
    private void GestureEventHandler(NIPointTracker hand)
    {        
        m_lastClickTime = Time.time;
        m_lastClickFrame = Time.frameCount;
        m_lastClickPos = Position;
    }
}
