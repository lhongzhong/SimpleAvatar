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
using System.Collections.Generic;

/// @brief This class implements NI aware GUI elements. 
/// 
/// This class is designed to replace the regular GUI static class for all NI aware
/// elements (e.g. buttons).
/// @ingroup OpenNIGUIUtiliites
public static class NIGUI 
{
    /// Method to set the cursor
    /// @note before starting to use NIGUI one MUST set a cursor (the prefab does that by 
    /// setting it in the "awake" method.
    /// @param newCursor the new cursor to add
    public static void SetCursor(NIGUICursor newCursor)
    {
        m_cursor=newCursor;
    }

    /// sets the cursor to be active or deactivated
    /// @param state True means NIGUI is active and false means it is not...
    public static void SetActive(bool state)
    {
        m_cursor.SetActive(state);
    }


    /// Accessor to the "changed" value.
    public static bool changed
    {
        get
        {
            if (GUI.changed)
                return true;
            if (m_curFrame < Time.frameCount)
            {
                m_curFrame = Time.frameCount;
                m_changedSinceLastFrame = false;
                return false; // we are at a new frame.
            }
            return m_changedSinceLastFrame;
        }
    }


    /// same as GUI.Button but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param text the text to show on the button
    /// @return true if the button was pressed
    public static bool Button(Rect pos, string text)
    {
        if (GUI.Button(pos, text))
            return true;
        bool clicked=TestClicked(ref pos);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.Button but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param image the texture to show on the button
    /// @return true if the button was pressed
    public static bool Button(Rect pos, Texture image)
    {
        if (GUI.Button(pos, image))
            return true;
        bool clicked = TestClicked(ref pos);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.Button but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param content the GUIContent to show on the button
    /// @return true if the button was pressed
    public static bool Button(Rect pos, GUIContent content)
    {
        if (GUI.Button(pos, content))
            return true;
        bool clicked = TestClicked(ref pos);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.Button but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param text the text to show on the button
    /// @param style the GUIStyle to use
    /// @return true if the button was pressed
    public static bool Button(Rect pos, string text, GUIStyle style)
    {
        if (GUI.Button(pos, text, style))
            return true;
        bool clicked = TestClicked(ref pos);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.Button but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param image the texture to show on the button
    /// @param style the GUIStyle to use
    /// @return true if the button was pressed
    public static bool Button(Rect pos, Texture image, GUIStyle style)
    {
        if (GUI.Button(pos, image, style))
            return true;
        bool clicked = TestClicked(ref pos);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.Button but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param content the GUIContent to show on the button
    /// @param style the GUIStyle to use
    /// @return true if the button was pressed
    public static bool Button(Rect pos, GUIContent content, GUIStyle style)
    {
        if (GUI.Button(pos, content, style))
            return true;
        bool clicked = TestClicked(ref pos);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.RepeatButton but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param text the text to show on the button
    /// @return true if the button was pressed
    public static bool RepeatButton(Rect pos, string text)
    {
        if (GUI.RepeatButton(pos, text))
            return true;
        if (m_cursor == null)
            throw new System.Exception("You must set a NIGUICursor using NIGUI.SetCursor(newCursor).");
        if (m_cursor.HasClickedThisFrame() == false)
            return false;
        if (CorrectRectForGroups(ref pos) == false)
            return false;
        bool clicked = pos.Contains(m_cursor.Position);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.RepeatButton but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param image the texture to show on the button
    /// @return true if the button was pressed
    public static bool RepeatButton(Rect pos, Texture image)
    {
        if (GUI.RepeatButton(pos, image))
            return true;
        if (m_cursor == null)
            throw new System.Exception("You must set a NIGUICursor using NIGUI.SetCursor(newCursor).");
        if (m_cursor.HasClickedThisFrame() == false)
            return false;
        if (CorrectRectForGroups(ref pos) == false)
            return false;
        bool clicked = pos.Contains(m_cursor.Position);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.RepeatButton but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param content the GUIContent to show on the button
    /// @return true if the button was pressed
    public static bool RepeatButton(Rect pos, GUIContent content)
    {
        if (GUI.RepeatButton(pos, content))
            return true;
        if (m_cursor == null)
            throw new System.Exception("You must set a NIGUICursor using NIGUI.SetCursor(newCursor).");
        if (m_cursor.HasClickedThisFrame() == false)
            return false;
        if (CorrectRectForGroups(ref pos) == false)
            return false;
        bool clicked = pos.Contains(m_cursor.Position);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.RepeatButton but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param text the text to show on the button
    /// @param style the GUIStyle to use
    /// @return true if the button was pressed
    public static bool RepeatButton(Rect pos, string text, GUIStyle style)
    {
        if (GUI.RepeatButton(pos, text, style))
            return true;
        if (m_cursor == null)
            throw new System.Exception("You must set a NIGUICursor using NIGUI.SetCursor(newCursor).");
        if (m_cursor.HasClickedThisFrame() == false)
            return false;
        if (CorrectRectForGroups(ref pos) == false)
            return false;
        bool clicked = pos.Contains(m_cursor.Position);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.RepeatButton but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param image the texture to show on the button
    /// @param style the GUIStyle to use
    /// @return true if the button was pressed
    public static bool RepeatButton(Rect pos, Texture image, GUIStyle style)
    {
        if (GUI.RepeatButton(pos, image, style))
            return true;
        if (m_cursor == null)
            throw new System.Exception("You must set a NIGUICursor using NIGUI.SetCursor(newCursor).");
        if (m_cursor.HasClickedThisFrame() == false)
            return false;
        if (CorrectRectForGroups(ref pos) == false)
            return false;
        bool clicked = pos.Contains(m_cursor.Position);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.RepeatButton but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param content the GUIContent to show on the button
    /// @param style the GUIStyle to use
    /// @return true if the button was pressed
    public static bool RepeatButton(Rect pos, GUIContent content, GUIStyle style)
    {
        if (GUI.RepeatButton(pos, content, style))
            return true;
        if (m_cursor == null)
            throw new System.Exception("You must set a NIGUICursor using NIGUI.SetCursor(newCursor).");
        if (m_cursor.HasClickedThisFrame() == false)
            return false;
        if (CorrectRectForGroups(ref pos) == false)
            return false;
        bool clicked = pos.Contains(m_cursor.Position);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.Toggle but which can handle Natural Interactions
    /// @param pos the rect of where to draw the Toggle
    /// @param text the text to show on the Toggle
    /// @param value the value from before (how to draw)
    /// @return true if the Toggle was pressed
    public static bool Toggle(Rect pos, bool value, string text)
    {
        bool tmpValue;
        tmpValue=GUI.Toggle(pos,value, text);
        if (tmpValue != value)
            return tmpValue;
        if (TestClicked(ref pos))
        {
            MarkChange();
            return !tmpValue;
        }
        return tmpValue;
    }

    /// same as GUI.Toggle but which can handle Natural Interactions
    /// @param pos the rect of where to draw the Toggle
    /// @param image the texture to show on the Toggle
    /// @param value the value from before (how to draw)
    /// @return true if the Toggle was pressed
    public static bool Toggle(Rect pos, bool value, Texture image)
    {
        bool tmpValue;
        tmpValue = GUI.Toggle(pos, value, image);
        if (tmpValue != value)
            return tmpValue;
        if (TestClicked(ref pos))
        {
            MarkChange();
            return !tmpValue;
        }
        return tmpValue;
    }

    /// same as GUI.Toggle but which can handle Natural Interactions
    /// @param pos the rect of where to draw the Toggle
    /// @param value the value from before (how to draw)
    /// @param content the GUIContent to show on the Toggle
    /// @return true if the Toggle was pressed
    public static bool Toggle(Rect pos, bool value, GUIContent content)
    {
        bool tmpValue;
        tmpValue = GUI.Toggle(pos, value, content);
        if (tmpValue != value)
            return tmpValue;
        if (TestClicked(ref pos))
        {
            MarkChange();
            return !tmpValue;
        }
        return tmpValue;

    }

    /// same as GUI.Toggle but which can handle Natural Interactions
    /// @param pos the rect of where to draw the Toggle
    /// @param text the text to show on the Toggle
    /// @param value the value from before (how to draw)
    /// @param style the GUIStyle to use
    /// @return true if the Toggle was pressed
    public static bool Toggle(Rect pos, bool value, string text, GUIStyle style)
    {
        bool tmpValue;
        tmpValue = GUI.Toggle(pos, value, text, style);
        if (tmpValue != value)
            return tmpValue;
        if (TestClicked(ref pos))
        {
            MarkChange();
            return !tmpValue;
        }
        return tmpValue;
    }

    /// same as GUI.Toggle but which can handle Natural Interactions
    /// @param pos the rect of where to draw the Toggle
    /// @param value the value from before (how to draw)
    /// @param image the texture to show on the Toggle
    /// @param style the GUIStyle to use
    /// @return true if the Toggle was pressed
    public static bool Toggle(Rect pos, bool value, Texture image, GUIStyle style)
    {
        bool tmpValue;
        tmpValue = GUI.Toggle(pos, value, image, style);
        if (tmpValue != value)
            return tmpValue;
        if (TestClicked(ref pos))
        {
            MarkChange();
            return !tmpValue;
        }
        return tmpValue;
    }

    /// same as GUI.Toggle but which can handle Natural Interactions
    /// @param pos the rect of where to draw the Toggle
    /// @param value the value from before (how to draw)
    /// @param content the GUIContent to show on the Toggle
    /// @param style the GUIStyle to use
    /// @return true if the Toggle was pressed
    public static bool Toggle(Rect pos, bool value, GUIContent content, GUIStyle style)
    {
        bool tmpValue;
        tmpValue = GUI.Toggle(pos, value, content, style);
        if (tmpValue != value)
            return tmpValue;
        if (TestClicked(ref pos))
        {
            MarkChange();
            return !tmpValue;
        }
        return tmpValue;
    }

    /// same as GUI.Toolbar but which can handle Natural Interactions
    /// @note it assumes the buttons are distributed evenly, if this is changed (e.g. due
    /// to changes in the style or skin) then unpredictable results could occur!
    /// @param pos the rect of where to draw the Toolbar
    /// @param selected the selected index
    /// @param texts the texts (array) to show on the buttons
    /// @return the index of the button to show.
    public static int Toolbar(Rect pos, int selected, string[] texts)
    {
        int newValue;
        newValue = GUI.Toolbar(pos, selected, texts);
        if (newValue != selected)
        {
            return newValue;
        }
        Vector2 clickedPoint;
        if (TestClicked(ref pos, out clickedPoint) == false)
            return selected;
        // now that we know we clicked, we need to figure out where by dividing the
        // "x" of the position evenly along the box.
        clickedPoint.x -= pos.x;
        float scale = pos.width / (float)texts.Length; // this is how much is given to each element
        float selectPos = clickedPoint.x / scale;
        int res=Mathf.FloorToInt(selectPos);
        if(res!=selected)
            MarkChange();
        return res;
    }
    
    /// same as GUI.Toolbar but which can handle Natural Interactions
    /// @note it assumes the buttons are distributed evenly, if this is changed (e.g. due
    /// to changes in the style or skin) then unpredictable results could occur!
    /// @param pos the rect of where to draw the Toolbar
    /// @param selected the selected index
    /// @param images the textures (array) to show on the buttons
    /// @return the index of the button to show.
    public static int Toolbar(Rect pos, int selected, Texture[] images)
    {
        int newValue;
        newValue = GUI.Toolbar(pos, selected, images);
        if (newValue != selected)
        {
            return newValue;
        }
        Vector2 clickedPoint;
        if (TestClicked(ref pos, out clickedPoint) == false)
            return selected;
        // now that we know we clicked, we need to figure out where by dividing the
        // "x" of the position evenly along the box.
        clickedPoint.x -= pos.x;
        float scale = pos.width / (float)images.Length; // this is how much is given to each element
        float selectPos = clickedPoint.x / scale;
        int res = Mathf.FloorToInt(selectPos);
        if (res != selected)
            MarkChange();
        return res;
    }

    /// same as GUI.Toolbar but which can handle Natural Interactions
    /// @note it assumes the buttons are distributed evenly, if this is changed (e.g. due
    /// to changes in the style or skin) then unpredictable results could occur!
    /// @param pos the rect of where to draw the Toolbar
    /// @param selected the selected index
    /// @param contents the GUIContents (array) to show on the buttons
    /// @return the index of the button to show.
    public static int Toolbar(Rect pos, int selected, GUIContent[] contents)
    {
        int newValue;
        newValue = GUI.Toolbar(pos, selected, contents);
        if (newValue != selected)
        {
            return newValue;
        }
        Vector2 clickedPoint;
        if (TestClicked(ref pos, out clickedPoint) == false)
            return selected;
        // now that we know we clicked, we need to figure out where by dividing the
        // "x" of the position evenly along the box.
        clickedPoint.x -= pos.x;
        float scale = pos.width / (float)contents.Length; // this is how much is given to each element
        float selectPos = clickedPoint.x / scale;
        int res = Mathf.FloorToInt(selectPos);
        if (res != selected)
            MarkChange();
        return res;
    }

    /// same as GUI.Toolbar but which can handle Natural Interactions
    /// @note it assumes the buttons are distributed evenly, if this is changed (e.g. due
    /// to changes in the style or skin) then unpredictable results could occur!
    /// @param pos the rect of where to draw the Toolbar
    /// @param selected the selected index
    /// @param texts the texts (array) to show on the buttons
    /// @param style the GUIStyle to use
    /// @return the index of the button to show.
    public static int Toolbar(Rect pos, int selected, string[] texts, GUIStyle style)
    {
        int newValue;
        newValue = GUI.Toolbar(pos, selected, texts, style);
        if (newValue != selected)
        {
            return newValue;
        }
        Vector2 clickedPoint;
        if (TestClicked(ref pos, out clickedPoint) == false)
            return selected;
        // now that we know we clicked, we need to figure out where by dividing the
        // "x" of the position evenly along the box.
        clickedPoint.x -= pos.x;
        float scale = pos.width / (float)texts.Length; // this is how much is given to each element
        float selectPos = clickedPoint.x / scale;
        int res = Mathf.FloorToInt(selectPos);
        if (res != selected)
            MarkChange();
        return res;
    }

    /// same as GUI.Toolbar but which can handle Natural Interactions
    /// @note it assumes the buttons are distributed evenly, if this is changed (e.g. due
    /// to changes in the style or skin) then unpredictable results could occur!
    /// @param pos the rect of where to draw the Toolbar
    /// @param selected the selected index
    /// @param images the textures (array) to show on the buttons
    /// @param style the GUIStyle to use
    /// @return the index of the button to show.
    public static int Toolbar(Rect pos, int selected, Texture[] images, GUIStyle style)
    {
        int newValue;
        newValue = GUI.Toolbar(pos, selected, images, style);
        if (newValue != selected)
        {
            return newValue;
        }
        Vector2 clickedPoint;
        if (TestClicked(ref pos, out clickedPoint) == false)
            return selected;
        // now that we know we clicked, we need to figure out where by dividing the
        // "x" of the position evenly along the box.
        clickedPoint.x -= pos.x;
        float scale = pos.width / (float)images.Length; // this is how much is given to each element
        float selectPos = clickedPoint.x / scale;
        int res = Mathf.FloorToInt(selectPos);
        if (res != selected)
            MarkChange();
        return res;
    }

    /// same as GUI.Toolbar but which can handle Natural Interactions
    /// @note it assumes the buttons are distributed evenly, if this is changed (e.g. due
    /// to changes in the style or skin) then unpredictable results could occur!
    /// @param pos the rect of where to draw the Toolbar
    /// @param selected the selected index
    /// @param contents the GUIContents (array) to show on the buttons
    /// @param style the GUIStyle to use
    /// @return the index of the button to show.
    public static int Toolbar(Rect pos, int selected, GUIContent[] contents, GUIStyle style)
    {
        int newValue;
        newValue = GUI.Toolbar(pos, selected, contents, style);
        if (newValue != selected)
        {
            return newValue;
        }
        Vector2 clickedPoint;
        if (TestClicked(ref pos, out clickedPoint) == false)
            return selected;
        // now that we know we clicked, we need to figure out where by dividing the
        // "x" of the position evenly along the box.
        clickedPoint.x -= pos.x;
        float scale = pos.width / (float)contents.Length; // this is how much is given to each element
        float selectPos = clickedPoint.x / scale;
        int res = Mathf.FloorToInt(selectPos);
        if (res != selected)
            MarkChange();
        return res;
    }


    /// same as GUI.SelectionGrid but which can handle Natural Interactions
    /// @note it assumes the buttons are distributed evenly, if this is changed (e.g. due
    /// to changes in the style or skin) then unpredictable results could occur!
    /// @param pos the rect of where to draw the SelectionGrid
    /// @param selected the selected index
    /// @param texts the texts (array) to show on the buttons
    /// @param xCount the number of buttons on the x axis
    /// @return the index of the button to show.
    public static int SelectionGrid(Rect pos, int selected, string[] texts, int xCount)
    {
        int newValue;
        newValue = GUI.SelectionGrid(pos, selected, texts,xCount);
        if (newValue != selected)
        {
            return newValue;
        }
        Vector2 clickedPoint;
        if (TestClicked(ref pos, out clickedPoint) == false)
            return selected;
        // now that we know we clicked, we need to figure out where by dividing the
        // "x" of the position evenly along the box.
        clickedPoint.x -= pos.x;
        clickedPoint.y -= pos.y;
        float scaleX = pos.width / (float)xCount; // this is how much is given to each element in X
        int yCount =Mathf.CeilToInt((float)texts.Length / (float)xCount);
        float scaleY = pos.height / yCount; // this is how much is given to each element in Y
        float selectPos = clickedPoint.x / scaleX;
        int xPos = Mathf.FloorToInt(selectPos);
        selectPos = clickedPoint.y / scaleY;
        int yPos = Mathf.FloorToInt(selectPos);
        int res=xPos+(yPos*xCount);
        if (res >= texts.Length)
            return selected;
        if (res != selected)
            MarkChange();
        return res;
    }

    /// same as GUI.SelectionGrid but which can handle Natural Interactions
    /// @note it assumes the buttons are distributed evenly, if this is changed (e.g. due
    /// to changes in the style or skin) then unpredictable results could occur!
    /// @param pos the rect of where to draw the SelectionGrid
    /// @param selected the selected index
    /// @param images the textures (array) to show on the buttons
    /// @param xCount the number of buttons on the x axis
    /// @return the index of the button to show.
    public static int SelectionGrid(Rect pos, int selected, Texture[] images, int xCount)
    {
        int newValue;
        newValue = GUI.SelectionGrid(pos, selected, images, xCount);
        if (newValue != selected)
        {
            return newValue;
        }
        Vector2 clickedPoint;
        if (TestClicked(ref pos, out clickedPoint) == false)
            return selected;
        // now that we know we clicked, we need to figure out where by dividing the
        // "x" of the position evenly along the box.
        clickedPoint.x -= pos.x;
        clickedPoint.y -= pos.y;
        float scaleX = pos.width / (float)xCount; // this is how much is given to each element in X
        int yCount = Mathf.CeilToInt((float)images.Length / (float)xCount);
        float scaleY = pos.height / yCount; // this is how much is given to each element in Y
        float selectPos = clickedPoint.x / scaleX;
        int xPos = Mathf.FloorToInt(selectPos);
        selectPos = clickedPoint.y / scaleY;
        int yPos = Mathf.FloorToInt(selectPos);
        int res = xPos + (yPos * xCount);
        if (res >= images.Length)
            return selected;
        if (res != selected)
            MarkChange();
        return res;
    }


    /// same as GUI.SelectionGrid but which can handle Natural Interactions
    /// @note it assumes the buttons are distributed evenly, if this is changed (e.g. due
    /// to changes in the style or skin) then unpredictable results could occur!
    /// @param pos the rect of where to draw the SelectionGrid
    /// @param selected the selected index
    /// @param contents the textures (array) to show on the buttons
    /// @param xCount the number of buttons on the x axis
    /// @return the index of the button to show.
    public static int SelectionGrid(Rect pos, int selected, GUIContent[] contents, int xCount)
    {
        int newValue;
        newValue = GUI.SelectionGrid(pos, selected, contents, xCount);
        if (newValue != selected)
        {
            return newValue;
        }
        Vector2 clickedPoint;
        if (TestClicked(ref pos, out clickedPoint) == false)
            return selected;
        // now that we know we clicked, we need to figure out where by dividing the
        // "x" of the position evenly along the box.
        clickedPoint.x -= pos.x;
        clickedPoint.y -= pos.y;
        float scaleX = pos.width / (float)xCount; // this is how much is given to each element in X
        int yCount = Mathf.CeilToInt((float)contents.Length / (float)xCount);
        float scaleY = pos.height / yCount; // this is how much is given to each element in Y
        float selectPos = clickedPoint.x / scaleX;
        int xPos = Mathf.FloorToInt(selectPos);
        selectPos = clickedPoint.y / scaleY;
        int yPos = Mathf.FloorToInt(selectPos);
        int res = xPos + (yPos * xCount);
        if (res >= contents.Length)
            return selected;
        if (res != selected)
            MarkChange();
        return res;
    }

    /// same as GUI.SelectionGrid but which can handle Natural Interactions
    /// @note it assumes the buttons are distributed evenly, if this is changed (e.g. due
    /// to changes in the style or skin) then unpredictable results could occur!
    /// @param pos the rect of where to draw the SelectionGrid
    /// @param selected the selected index
    /// @param texts the texts (array) to show on the buttons
    /// @param xCount the number of buttons on the x axis
    /// @param style the GUIStyle to use
    /// @return the index of the button to show.
    public static int SelectionGrid(Rect pos, int selected, string[] texts, int xCount, GUIStyle style)
    {
        int newValue;
        newValue = GUI.SelectionGrid(pos, selected, texts, xCount, style);
        if (newValue != selected)
        {
            return newValue;
        }
        Vector2 clickedPoint;
        if (TestClicked(ref pos, out clickedPoint) == false)
            return selected;
        // now that we know we clicked, we need to figure out where by dividing the
        // "x" of the position evenly along the box.
        clickedPoint.x -= pos.x;
        clickedPoint.y -= pos.y;
        float scaleX = pos.width / (float)xCount; // this is how much is given to each element in X
        int yCount = Mathf.CeilToInt((float)texts.Length / (float)xCount);
        float scaleY = pos.height / yCount; // this is how much is given to each element in Y
        float selectPos = clickedPoint.x / scaleX;
        int xPos = Mathf.FloorToInt(selectPos);
        selectPos = clickedPoint.y / scaleY;
        int yPos = Mathf.FloorToInt(selectPos);
        int res = xPos + (yPos * xCount);
        if (res >= texts.Length)
            return selected;
        if (res != selected)
            MarkChange();
        return res;
    }

    /// same as GUI.SelectionGrid but which can handle Natural Interactions
    /// @note it assumes the buttons are distributed evenly, if this is changed (e.g. due
    /// to changes in the style or skin) then unpredictable results could occur!
    /// @param pos the rect of where to draw the SelectionGrid
    /// @param selected the selected index
    /// @param images the textures (array) to show on the buttons
    /// @param xCount the number of buttons on the x axis
    /// @param style the GUIStyle to use
    /// @return the index of the button to show.
    public static int SelectionGrid(Rect pos, int selected, Texture[] images, int xCount, GUIStyle style)
    {
        int newValue;
        newValue = GUI.SelectionGrid(pos, selected, images, xCount, style);
        if (newValue != selected)
        {
            return newValue;
        }
        Vector2 clickedPoint;
        if (TestClicked(ref pos, out clickedPoint) == false)
            return selected;
        // now that we know we clicked, we need to figure out where by dividing the
        // "x" of the position evenly along the box.
        clickedPoint.x -= pos.x;
        clickedPoint.y -= pos.y;
        float scaleX = pos.width / (float)xCount; // this is how much is given to each element in X
        int yCount = Mathf.CeilToInt((float)images.Length / (float)xCount);
        float scaleY = pos.height / yCount; // this is how much is given to each element in Y
        float selectPos = clickedPoint.x / scaleX;
        int xPos = Mathf.FloorToInt(selectPos);
        selectPos = clickedPoint.y / scaleY;
        int yPos = Mathf.FloorToInt(selectPos);
        int res = xPos + (yPos * xCount);
        if (res >= images.Length)
            return selected;
        if (res != selected)
            MarkChange();
        return res;
    }


    /// same as GUI.SelectionGrid but which can handle Natural Interactions
    /// @note it assumes the buttons are distributed evenly, if this is changed (e.g. due
    /// to changes in the style or skin) then unpredictable results could occur!
    /// @param pos the rect of where to draw the SelectionGrid
    /// @param selected the selected index
    /// @param contents the GUIContents (array) to show on the buttons
    /// @param style the GUIStyle to use
    /// @param xCount the number of buttons on the x axis
    /// @return the index of the button to show.
    public static int SelectionGrid(Rect pos, int selected, GUIContent[] contents, int xCount, GUIStyle style)
    {
        int newValue;
        newValue = GUI.SelectionGrid(pos, selected, contents, xCount, style);
        if (newValue != selected)
        {
            return newValue;
        }
        Vector2 clickedPoint;
        if (TestClicked(ref pos, out clickedPoint) == false)
            return selected;
        // now that we know we clicked, we need to figure out where by dividing the
        // "x" of the position evenly along the box.
        clickedPoint.x -= pos.x;
        clickedPoint.y -= pos.y;
        float scaleX = pos.width / (float)xCount; // this is how much is given to each element in X
        int yCount = Mathf.CeilToInt((float)contents.Length / (float)xCount);
        float scaleY = pos.height / yCount; // this is how much is given to each element in Y
        float selectPos = clickedPoint.x / scaleX;
        int xPos = Mathf.FloorToInt(selectPos);
        selectPos = clickedPoint.y / scaleY;
        int yPos = Mathf.FloorToInt(selectPos);
        int res = xPos + (yPos * xCount);
        if (res >= contents.Length)
            return selected;
        if (res != selected)
            MarkChange();
        return res;
    }

    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    static public void BeginGroup(Rect pos)
    {
        GUI.BeginGroup(pos);
        m_groups.Add(pos);
    }

    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param text the text to show on the BeginGroup
    static public void BeginGroup(Rect pos, string text)
    {
        GUI.BeginGroup(pos,text);
        m_groups.Add(pos);
    }
    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param image the texture to show on the BeginGroup
    static public void BeginGroup(Rect pos, Texture image)
    {
        GUI.BeginGroup(pos,image);
        m_groups.Add(pos);
    }
    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param content the GUIContent to show on the BeginGroup
    static public void BeginGroup(Rect pos, GUIContent content)
    {
        GUI.BeginGroup(pos,content);
        m_groups.Add(pos);
    }
    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param style the GUIStyle to use
    static public void BeginGroup(Rect pos,GUIStyle style)
    {
        GUI.BeginGroup(pos, style);
        m_groups.Add(pos);
    }
    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param text the text to show on the BeginGroup
    /// @param style the GUIStyle to use
    static public void BeginGroup(Rect pos, string text, GUIStyle style)
    {
        GUI.BeginGroup(pos, text, style);
        m_groups.Add(pos);
    }
    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param image the texture to show on the BeginGroup
    /// @param style the GUIStyle to use
    static public void BeginGroup(Rect pos, Texture image, GUIStyle style)
    {
        GUI.BeginGroup(pos, image, style);
        m_groups.Add(pos);
    }
    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param content the GUIContent to show on the BeginGroup
    /// @param style the GUIStyle to use
    static public void BeginGroup(Rect pos, GUIContent content, GUIStyle style)
    {
        GUI.BeginGroup(pos, content, style);
        m_groups.Add(pos);
    }

    /// same as GUI.EndGroup but which can handle Natural Interactions
    static public void EndGroup()
    {
        GUI.EndGroup();
        m_groups.RemoveAt(m_groups.Count - 1);
    }

    /// same as GUI.HorizontalScrollbar but which can handle Natural Interactions
    /// @param pos the rect of where to draw the scrollbar @note the height is used to decide where to catch so it should match the graphics 
    /// @param origValue the value where the slider button should be drawn at
    /// @param size the size of the slider button (actual values are between leftValue and rightValue-size)
    /// @param leftValue the value at the leftmost position
    /// @param rightValue the value at the rightmost position @note if size>0 it is impossible to actually reach it.
    /// @return the modified value. This can be changed by the user by dragging the scrollbar
    static public float HorizontalScrollbar(Rect pos, float origValue, float size, float leftValue, float rightValue)
    {
        float tmpValue = GUI.HorizontalScrollbar(pos, origValue, size, leftValue, rightValue);
        if (tmpValue != origValue)
            return tmpValue;
        float newValue=InternalSlider(pos, origValue, size, leftValue, rightValue, true);
        if (newValue != origValue)
            MarkChange();
        return newValue;
    }

    /// same as GUI.HorizontalScrollbar but which can handle Natural Interactions
    /// @param pos the rect of where to draw the scrollbar @note the height is used to decide where to catch so it should match the graphics 
    /// @param origValue the value where the slider button should be drawn at
    /// @param size the size of the slider button (actual values are between leftValue and rightValue-size)
    /// @param leftValue the value at the leftmost position
    /// @param rightValue the value at the rightmost position @note if size>0 it is impossible to actually reach it.
    /// @param style the GUIStyle to use
    /// @return the modified value. This can be changed by the user by dragging the scrollbar
    static public float HorizontalScrollbar(Rect pos, float origValue, float size, float leftValue, float rightValue, GUIStyle style)
    {
        float tmpValue = GUI.HorizontalScrollbar(pos, origValue, size, leftValue, rightValue, style);
        if (tmpValue != origValue)
            return tmpValue;
        float newValue= InternalSlider(pos, origValue, size, leftValue, rightValue, true);
        if (newValue != origValue)
            MarkChange();
        return newValue;
    }


    /// same as GUI.VerticalScrollbar but which can handle Natural Interactions
    /// @param pos the rect of where to draw the scrollbar @note the height is used to decide where to catch so it should match the graphics 
    /// @param origValue the value where the slider button should be drawn at
    /// @param size the size of the slider button (actual values are between leftValue and rightValue-size)
    /// @param leftValue the value at the leftmost position
    /// @param rightValue the value at the rightmost position @note if size>0 it is impossible to actually reach it.
    /// @return the modified value. This can be changed by the user by dragging the scrollbar
    static public float VerticalScrollbar(Rect pos, float origValue, float size, float leftValue, float rightValue)
    {
        float tmpValue = GUI.VerticalScrollbar(pos, origValue, size, leftValue, rightValue);
        if (tmpValue != origValue)
            return tmpValue;
        float newValue=InternalSlider(pos, origValue, size, leftValue, rightValue, false);
        if (newValue != origValue)
            MarkChange();
        return newValue;
    }

    /// same as GUI.VerticalScrollbar but which can handle Natural Interactions
    /// @param pos the rect of where to draw the scrollbar @note the height is used to decide where to catch so it should match the graphics 
    /// @param origValue the value where the slider button should be drawn at
    /// @param size the size of the slider button (actual values are between leftValue and rightValue-size)
    /// @param leftValue the value at the leftmost position
    /// @param rightValue the value at the rightmost position @note if size>0 it is impossible to actually reach it.
    /// @param style the GUIStyle to use
    /// @return the modified value. This can be changed by the user by dragging the scrollbar
    static public float VerticalScrollbar(Rect pos, float origValue, float size, float leftValue, float rightValue, GUIStyle style)
    {
        float tmpValue = GUI.VerticalScrollbar(pos, origValue, size, leftValue, rightValue, style);
        if (tmpValue != origValue)
            return tmpValue;
        float newValue=InternalSlider(pos, origValue, size, leftValue, rightValue, false);
        if (newValue != origValue)
            MarkChange();
        return newValue;
    }



    /// same as GUI.HorizontalSlider but which can handle Natural Interactions
    /// @param pos the rect of where to draw the scrollbar @note the height is used to decide where to catch so it should match the graphics 
    /// @param origValue the value where the slider button should be drawn at
    /// @param leftValue the value at the leftmost position
    /// @param rightValue the value at the rightmost position @note if size>0 it is impossible to actually reach it.
    /// @return the modified value. This can be changed by the user by dragging the scrollbar
    static public float HorizontalSlider(Rect pos, float origValue, float leftValue, float rightValue)
    {
        float tmpValue = GUI.HorizontalSlider(pos, origValue, leftValue, rightValue);
        if (tmpValue != origValue)
            return tmpValue;
        float newValue=InternalSlider(pos, origValue, 0.0f, leftValue, rightValue, true);
        if (newValue != origValue)
            MarkChange();
        return newValue;
    }

    /// same as GUI.HorizontalSlider but which can handle Natural Interactions
    /// @param pos the rect of where to draw the scrollbar @note the height is used to decide where to catch so it should match the graphics 
    /// @param origValue the value where the slider button should be drawn at
    /// @param leftValue the value at the leftmost position
    /// @param rightValue the value at the rightmost position @note if size>0 it is impossible to actually reach it.
    /// @param slider the GUIStyle to use for the dragging area
    /// @param thumb the GUIStyle to use for the slider button.
    /// @return the modified value. This can be changed by the user by dragging the scrollbar
    static public float HorizontalSlider(Rect pos, float origValue, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb)
    {
        float tmpValue = GUI.HorizontalSlider(pos, origValue, leftValue, rightValue, slider, thumb);
        if (tmpValue != origValue)
            return tmpValue;
        float newValue=InternalSlider(pos, origValue, 0.0f, leftValue, rightValue, true);
        if (newValue != origValue)
            MarkChange();
        return newValue;
    }


    /// same as GUI.VerticalSlider but which can handle Natural Interactions
    /// @param pos the rect of where to draw the scrollbar @note the height is used to decide where to catch so it should match the graphics 
    /// @param origValue the value where the slider button should be drawn at
    /// @param leftValue the value at the leftmost position
    /// @param rightValue the value at the rightmost position @note if size>0 it is impossible to actually reach it.
    /// @return the modified value. This can be changed by the user by dragging the scrollbar
    static public float VerticalSlider(Rect pos, float origValue, float leftValue, float rightValue)
    {
        float tmpValue = GUI.VerticalSlider(pos, origValue, leftValue, rightValue);
        if (tmpValue != origValue)
            return tmpValue;
        float newValue=InternalSlider(pos, origValue, 0.0f, leftValue, rightValue, false);
        if (newValue != origValue)
            MarkChange();
        return newValue;
    }

    /// same as GUI.VerticalSlider but which can handle Natural Interactions
    /// @param pos the rect of where to draw the scrollbar @note the height is used to decide where to catch so it should match the graphics 
    /// @param origValue the value where the slider button should be drawn at
    /// @param leftValue the value at the leftmost position
    /// @param rightValue the value at the rightmost position @note if size>0 it is impossible to actually reach it.
    /// @param slider the GUIStyle to use for the dragging area
    /// @param thumb the GUIStyle to use for the slider button.
    /// @return the modified value. This can be changed by the user by dragging the scrollbar
    static public float VerticalSlider(Rect pos, float origValue, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb)
    {
        float tmpValue = GUI.VerticalSlider(pos, origValue, leftValue, rightValue, slider, thumb);
        if (tmpValue != origValue)
            return tmpValue;
        float newValue=InternalSlider(pos, origValue, 0.0f, leftValue, rightValue, false);
        if (newValue != origValue)
            MarkChange();
        return newValue;
    }

    
    /// This method is responsible for handling a generic slider (it is called from the overall sliders).
    /// it will return the slider value and will manage internal states such as m_doingSlider).
    /// It is assumed that there is a slider button and a slider area. If the click is outside the slider button
    /// then the result will "jump" to the correct value. If the click is on the button then the slider
    /// will move until a second click.
    /// @param pos the rect of the slider @note it assumes the axis we do not use is the correct size
    /// @param origValue this is what will return if nothing happened (i.e. we are not in the middle of sliding and no click was done).
    /// @param size the size of the slider button (the returned value is between minValue and maxValue-size)
    /// For the purpose of pressing clicking the button, a minimum value @ref m_minSliderButtonSize is used. 
    /// @param minValue the minimum value (at pos.x or pos.y)
    /// @param maxValue the maximum value (at pos.x+width or pos.y+height)
    /// @param horizontal if this is true the slider sliders from left to right and if false then it slides from up to down.
    /// @return the slider value
    static public float InternalSlider(Rect pos, float origValue, float size, float minValue, float maxValue, bool horizontal)
    {
        if (minValue == maxValue)
            return minValue; // just so we won't crush...
        Vector2 clickPosition;
        float curPos = horizontal ? m_cursor.Position.x : m_cursor.Position.y;
        float range = horizontal ? pos.width : pos.height;
        float minPos = horizontal ? pos.x : pos.y;

        float scale = (maxValue - minValue) / range; // this is the scaling between the position and the value.
        if (m_doingSlider)
        {
            Rect tmpRef = pos;
            if (CorrectRectForGroups(ref tmpRef) == false)
                return origValue; // this is not us...
            if (tmpRef.Contains(m_sliderStartPos) == false || tmpRef.Contains(m_sliderEndPos) == false)
                return origValue;
            if(TestClicked(ref pos, out clickPosition))
                m_doingSlider = false; // this click ended the sliding.

            float relativePos = horizontal ? m_sliderStartPos.x : m_sliderStartPos.y;
            // we already clicked on the slider but have not clicked to end. 

            // the following tells us where is the current position compared to the start position.
            float tmpValue = curPos-relativePos;
            // this gives us the value for that position
            tmpValue = minValue + (tmpValue * scale);
            return Mathf.Clamp(tmpValue, minValue, maxValue);
        }
        bool clicked = TestClicked(ref pos, out clickPosition);
        float clickPos = horizontal ? clickPosition.x : clickPosition.y;        // if we are here we are not doing any sliding.
        if (clicked == false)
            return origValue; // no click.
        // now we know we just clicked somewhere, lets figure out where
        float origValuePos = ((origValue - minValue) / scale) + minPos;
        float pixelSize = size / scale;
        if (pixelSize < m_minSliderButtonSize)
            pixelSize = m_minSliderButtonSize;
        float diff = clickPos - origValuePos;
        if (diff > pixelSize || diff<0)
        {
            // this means we are outside the button so lets just "jump" there
            float tmpValue = curPos - minPos;
            tmpValue = minValue + (tmpValue * scale);
            return Mathf.Clamp(tmpValue, minValue, maxValue);
        }
        // if we are here then we have clicked on slider button.
        // first we set where the slider starts
        m_sliderStartPos.x = pos.x;
        m_sliderStartPos.y = pos.y;
        m_sliderEndPos.x = pos.x + pos.width-1; /// we lower by one pixel to make sure the point is in the rect
        m_sliderEndPos.y = pos.y + pos.height - 1; /// we lower by one pixel to make sure the point is in the rect
        // now we move it a bit to handle the size of the button
        if (horizontal)
            m_sliderStartPos.x += diff;
        else
            m_sliderStartPos.y += diff;
        m_doingSlider = true;
        return origValue; // because we haven't really moved.

    }


    /// resets the group (should be called on level load as a group might have been open when the load level is called.
    /// @note this is done by default on the GUI cursor!
    static public void ResetGroups()
    {
        m_groups.Clear();
    }

    /// this is the minimum size (number of pixels) which the slider button can be for the purpose of deciding whether
    /// a click is on it or not.
    static public int m_minSliderButtonSize=20;



    /// Method to check if we clicked since the last found click
    /// @param where the rect of the control to check
    /// @return true when we clicked.
    static private bool TestClicked(ref Rect where)
    {
        Vector2 pos;
        return TestClicked(ref where, out pos);
    }


    /// Method to check if we clicked since the last found click
    /// @param where the rect of the control to check 
    /// @note the where rect will be changed based on the groups (to be in the cursor coordinate system). If TestClick returns
    /// false it is undefined whether it was changed or not.
    /// @param[out] lastClickPosition the position in which the click occurred. @note this is relevant only when returning true.
    /// @return true when we clicked.
    static private bool TestClicked(ref Rect where, out Vector2 lastClickPosition)
    {
        if (m_cursor == null)
            throw new System.Exception("You must set a NIGUICursor using NIGUI.SetCursor(newCursor).");
        int lastClickFrame;
        float lastClickTime;
        lastClickTime=m_cursor.GetLastClickedTime(out lastClickFrame,out lastClickPosition);
        if(lastClickTime<=m_lastClickTimeUsed)
            return false; // we already used or ignored that click.
        if (lastClickFrame < Time.frameCount - 1)
            return false; // if we are two frames back then we sure didn't miss it...
        if (CorrectRectForGroups(ref where) == false)
            return false; // the entire control was clipped.
        if (m_doingSlider)
        {
            if (where.Contains(m_sliderStartPos) == false || where.Contains(m_sliderEndPos) == false)
                return false; // we are doing a slider but are looking at a different controller
        }
        else
        {
            if (where.Contains(lastClickPosition) == false)
                return false; // this is not us
        }
        m_lastClickTimeUsed=Time.time; // we clicked now
        return true;
    }


    /// method to correct a rect for groups. Basically a group would create a moving and clipping of
    /// the rect based on the groups before it.
    /// @param rectToCorrect The rect to correct. @note This will CHANGE the rect (even if the rect is clipped)
    /// @return True when the rect is still legal after correction, false if it has been totally clipped.
    static private bool CorrectRectForGroups(ref Rect rectToCorrect)
    {
        for (int i = 0; i < m_groups.Count; i++)
        {
            // move the start position.
            rectToCorrect.x += m_groups[i].x;
            rectToCorrect.y += m_groups[i].y;

            // do the clipping

            // x axis clipping         
            if (rectToCorrect.x < m_groups[i].x)
            {
                rectToCorrect.width += rectToCorrect.x;
                rectToCorrect.x = m_groups[i].x;
            }
            if(rectToCorrect.x+rectToCorrect.width>=m_groups[i].x+m_groups[i].width)
                rectToCorrect.width = m_groups[i].x + m_groups[i].width - rectToCorrect.x; // clip it to reach the end.

            if (rectToCorrect.width <= 0)
                return false; // nothing left

            // y axis clipping         
            if (rectToCorrect.y < m_groups[i].y)
            {
                rectToCorrect.height += rectToCorrect.y;
                rectToCorrect.y = m_groups[i].y;
            }
            if (rectToCorrect.y + rectToCorrect.height >= m_groups[i].y+m_groups[i].height)
                rectToCorrect.height = m_groups[i].y + m_groups[i].height - rectToCorrect.y; // clip it to reach the end.

            if (rectToCorrect.height <= 0)
                return false; // nothing left

        }
        return true;
    }

    /// this utility marks a change in the GUI (should be called whenever there is a change.
    static private void MarkChange()
    {
        m_curFrame = Time.frameCount;
        m_changedSinceLastFrame = true;
    }

    static private int m_curFrame=-1; ///< holds the current frame
    static private bool m_changedSinceLastFrame=false; ///< holds true if there was a change from last frame.
    static private NIGUICursor m_cursor=null; ///< the cursor
    static private float m_lastClickTimeUsed=-1.0f; ///< the time of the last click we actually used to click
    static private List<Rect> m_groups = new List<Rect>(); ///< a list of groups opened by BeginGroup which weren't closed by EndGroup (their rect).
    static private bool m_doingSlider=false;               ///< if this is true then we clicked on the slider and are currently sliding
    ///this is the position of the slider defined and working in @ref m_doingSlider
    ///@note it is defined as the start of the range + the size from the start of the button itself. <br>
    ///To better understand this we can look at an example: Lets say we have a horizontal slider starting from x=100
    ///to x=200. Assume the slider's button itself is size 0. In this case the vector's x position would be 100.
    ///If on the other hand the slider's button itself was 10 pixels wide, then this value would be 100 if we clicked at
    ///its beginning, 105 if we clicked at its middle and 110 if we clicked at its end.
    static private Vector2 m_sliderStartPos=Vector2.zero;

    ///this is the end position of the slider defined and working in @ref m_doingSlider
    ///since more than one slider MIGHT start at the same place we also save the other end...
    static private Vector2 m_sliderEndPos = Vector2.zero;  
}
