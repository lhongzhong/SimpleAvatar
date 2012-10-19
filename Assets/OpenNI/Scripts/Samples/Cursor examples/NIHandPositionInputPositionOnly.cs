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

/// @brief utility class to track hand position in the x/y axes using @ref NIInput.
/// 
/// This class is a utility which should be attached to an invisible game object. It 
/// will draw (using OnGUI box) the position of the hand it is attached to on the
/// screen.
/// It assumes there are a horizontal and vertical axes in the @ref NIInput inspector.
/// @ingroup OpenNISpecificLogicSamples
public class NIHandPositionInputPositionOnly : MonoBehaviour 
{
    /// the input which controls us
    protected NIInput m_input;
    /// the initialization (mono-behavior)
    public void Start()
    {
        m_input = FindObjectOfType(typeof(NIInput)) as NIInput;
    }

    /// the mono-behavior update
    public void OnGUI()
    {       
        float x = m_input.GetAxis("NI_X");
        float y = m_input.GetAxis("NI_Y");
        // since the axes are between -0.5 and 0.5 we add 0.5 to get a value between 0 and 1.
        x += 0.5f;
        y += 0.5f;
        // The value is multiplied by the screen width (minus 20 for the cursor size) to decide
        // where to place it
        x *= (Screen.width-20);
        y *= (Screen.height-20);
        // draw the cursor
        GUI.Box(new Rect(x,y,20,20),"");
    }
}
