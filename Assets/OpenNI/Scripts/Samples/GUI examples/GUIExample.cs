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

/// @brief This class is an example for the use of NIGUI
/// 
/// This class creates a sample use of NIGUI with various GUI elements.
/// @ingroup OpenNISpecificLogicSamples
public class GUIExample : MonoBehaviour 
{
    NIInput m_input; ///< @brief The input we get the axes from.

    /// mono-behavior start - initializes the input
    public void Start()
    {
        m_input = FindObjectOfType(typeof(NIInput)) as NIInput;
    }

    /// mono-behavior OnGUI to show GUI elements
    public void OnGUI()
    {
        // create a regular label
        myRect.x=(Screen.width/2)-40;
        myRect.y=20;
        myRect.width=80;
        myRect.height=30;
        GUI.Label(myRect, "Example GUI");


        // place the first button
        myRect.x = 200;
        myRect.y = 50;
        myRect.width = 80;
        myRect.height = 40;
        if (NIGUI.Button(myRect, "Button1"))
        {
           buttonPressedMessage = "Button 1 was pressed at time=" + Time.time;
        }

        // place the second button
        myRect.x = 300;
        myRect.y = 50;
        if (NIGUI.Button(myRect, "Button2"))
        {
            buttonPressedMessage = "Button 2 was pressed at time=" + Time.time;
        }


        // place the repeat button
        myRect.x = 200;
        myRect.y = 100;
        if (NIGUI.RepeatButton(myRect, "Repeat"))
            buttonPressedMessage = "Repeat button was pressed at time=" + Time.time;

        // place the toggle button
        myRect.x = 300;
        myRect.y = 100;
        myRect.width = 250;
        m_toggle1 = NIGUI.Toggle(myRect, m_toggle1, "Toggle example");

        // place the GUI changed frame
        myRect.x = 50;
        myRect.y = (Screen.height / 2) - 140;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, guiFrameCahngedMessage);

        // place the vScroll value label
        myRect.x = 50;
        myRect.y = (Screen.height / 2) - 100;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, "vScroll=" + vScroll);

        // place the hScroll value label
        myRect.x = 50;
        myRect.y = (Screen.height / 2) - 60;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, "hScroll=" + hScroll);

        // place the button pressed label
        myRect.x = 50;
        myRect.y = (Screen.height/2) - 20;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, buttonPressedMessage);

        // Click axis value label
        myRect.x = 50;
        myRect.y = (Screen.height / 2) + 20;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, "value=" + m_input.GetAxis("NIGUI_CLICK"));


        // place the toolbar GUI
        myRect.x = 50;
        myRect.y = Screen.height - 200;
        myRect.width = 350;
        myRect.height = 30;
        toolbarInt = NIGUI.Toolbar(myRect, toolbarInt, toolbarStrings);

        // place the selection grid GUI element
        myRect.x = 50;
        myRect.y = Screen.height - 150;
        myRect.width = 350;
        myRect.height = 90;
        selectionGridInt = NIGUI.SelectionGrid(myRect, selectionGridInt, toolbarStrings, 2);

        // place the horizontal scrollbar of the clipped button
        myRect.x = (Screen.width) - 500;
        myRect.y = (Screen.height / 2) - 240;
        myRect.width = 400;
        myRect.height = 20;
        hScroll = NIGUI.HorizontalScrollbar(myRect, hScroll, 0.0f, 0.0f, 300.0f);

        // place the vertical scrollbar of the clipped button
        myRect.x = (Screen.width) - 510;
        myRect.y = (Screen.height / 2) - 225; 
        myRect.width = 10;
        myRect.height = 200;
        vScroll = NIGUI.VerticalScrollbar(myRect, vScroll, 40.0f, 0.0f, 140.0f);
   
        // placed the clipped area for the button        
        myRect.x = (Screen.width) - 500;
        myRect.y = (Screen.height / 2) - 225;
        myRect.width = 400;
        myRect.height = 200;
        NIGUI.BeginGroup(myRect);
        // place the internal button
        myRect.x=0;
        myRect.y=0;
        Color c = GUI.backgroundColor;
        Color almostClear = c;
        almostClear.a = 0.2f;
        GUI.backgroundColor = almostClear;
        GUI.Box(myRect, "");
        GUI.backgroundColor = c;
        if (NIGUI.Button(new Rect(150-hScroll, 50-vScroll, 300, 200), "a button to be clipped by the view"))
            buttonPressedMessage = "Internal button to group was pressed";
        NIGUI.EndGroup();

        // place the float slider 
        myRect.x = (Screen.width) - 500;
        myRect.y = (Screen.height / 2) + 110;
        myRect.width = 350;
        myRect.height = 30;
        floatSlider = NIGUI.HorizontalSlider(myRect, floatSlider, -5.0f, 5.0f);

        // place the float slider label
        myRect.x = (Screen.width) - 500;
        myRect.y = (Screen.height / 2) + 80;
        myRect.width = 350;
        myRect.height = 30;
        GUI.Label(myRect,"float slide=" + floatSlider);


        // place the int slider
        myRect.x = (Screen.width) - 510;
        myRect.y = (Screen.height / 2) + 120;
        myRect.width = 30;
        myRect.height = 150;
        intSlider = NIGUI.VerticalSlider(myRect, intSlider, -10.0f, 10.0f);


        // place the int slider label
        myRect.x = (Screen.width) - 490;
        myRect.y = (Screen.height / 2) + 180;
        myRect.width = 350;
        myRect.height = 30;
        GUI.Label(myRect, "int slide=" + Mathf.RoundToInt(intSlider));

        
        // update the GUI changed frame
        if (NIGUI.changed)
            guiFrameCahngedMessage = "GUI changed at frame=" + Time.frameCount;


    }

    /// @brief useful members to draw the GUI
    /// 
    /// @{
    private string[] toolbarStrings = new string[] { "Toolbar1", "Toolbar1", "Toolbar3", "Toolbar4", "Toolbar5" };
    private int toolbarInt = 0; 
    private int selectionGridInt = 0;

    private string buttonPressedMessage = "Nothing was pressed yet";
    private string guiFrameCahngedMessage = "GUI last changed at frame 0"; 
    private Rect myRect = new Rect(0, 0, 100, 100); 
    private bool m_toggle1=false; 
    private float hScroll, vScroll;
    private float floatSlider;
    private float intSlider;
    //@}
}
