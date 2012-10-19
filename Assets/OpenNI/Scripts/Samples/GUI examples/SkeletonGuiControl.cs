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

/// @brief A utility class to move between skeleton mode and GUI modes
/// 
/// This class is used in the sample to move between skeleton mode (shows the skeleton)
/// and GUI mode (shows a GUI provided in here).
/// @ingroup OpenNISpecificLogicSamples
public class SkeletonGuiControl : MonoBehaviour
{
    /// @brief An internal enum to define the GUI mode
    public enum SkeletonGUIModes
    { 
        SkeletonMode, ///< In this mode we track the skeleton
        GUIMode       ///< In this mode we use the GUI (and turn off the relevant skeleton!)
    };

    /// @brief The mode (Skeleton mode where we track the skeleton or GUI mode where we have GUI)
    static public SkeletonGUIModes m_mode;
    /// @brief The input object to use to control the GUI and track the gestures
    /// @note Other than the definitions required by the cursor used in NIGUI, the SkeletonSwitch
    /// axis must be defined as a gesture with a sensitivity of 1. Once the value reaches 1 we will
    /// switch from skeleton mode to GUI mode.
    public NIInput m_input; 

    /// @brief The OpenNI settings
    public OpenNISettingsManager m_settings;

    /// @brief The image of the exit pose
    public Texture2D m_image;

    /// @brief Internal array which holds all skeleton controllers found in the scene.
    private NISkeletonController[] m_controllers = null; 

    private Rect tempRect; ///< @brief Temporary rectangle which sets the position of items (defined here to avoid creating on the fly).
	/// @brief mono-behavior start for initialization
	void Start () 
    {
        m_mode = SkeletonGUIModes.SkeletonMode;	
        // initialize all the external links
        if(m_input==null)
        {
            m_input = FindObjectOfType(typeof(NIInput)) as NIInput;
            if (m_input == null)
                throw new System.Exception("Please add an NIInput object to the scene");
        }

        if (m_settings == null)
        {
            m_settings = FindObjectOfType(typeof(OpenNISettingsManager)) as OpenNISettingsManager;
            if (m_settings == null)
                throw new System.Exception("Please add an OpenNISettingsManager object to the scene");
        }
        // a rect used later, this is mainly an initialization
        tempRect = new Rect();
        tempRect.x = Screen.width / 2 - 60;
        tempRect.y = Screen.height / 2 - 20;
        tempRect.width = 120;
        tempRect.height = 40;
        m_controllers = FindObjectsOfType(typeof(NISkeletonController)) as NISkeletonController[];
        NIGUI.SetActive(false); // we don't want to see the cursor yet so we deactivate NIGUI
	}
	
	/// @brief mono-behavior OnGUI for GUI logic. Is also responsible for mode change
    void OnGUI()
    {
        // option 1: we are in skeleton mode. All we need to do is try to detect the exit pose
        if (m_mode == SkeletonGUIModes.SkeletonMode)
        {
            // this tells us if we are detecting the exit pose.
            float val=m_input.GetAxis("SkeletonSwitch");
            if (val >= 1.0f)
            {
                // we have detected the exit pose so we are moving to GUI mode.
                m_mode=SkeletonGUIModes.GUIMode;
                // deactivate the skeleton (controller, not the capability)
                
                foreach (NISkeletonController controller in m_controllers)
                {
                    controller.SetSkeletonActive(false);
                }
                
                // when tracking a hand we need a high smoothing factor.
                m_settings.SmoothFactor = 0.95f;
                // reactivate NIGUI.
                NIGUI.SetActive(true);
                return;
            }
            if (val > 0)
            {
                // The value is between 0 and 1 so we have detected the exit pose and are waiting
                // for it to continue long enough. 
                // Just pop up a message telling the user to continue holding the pose...
                tempRect.y = 40;
                if (m_image != null)
                {
                    Rect tmpRect2=tempRect;
                    tmpRect2.height=128;
                    tmpRect2.width=128;
                    GUI.Box(tmpRect2, m_image);
                    tempRect.y = 168;
                }
                GUI.Box(tempRect, "Hold the pose until " + val + " reaches 1");
                return;
            }
            return; // nothing to do
        }
        // here we are on GUI mode. Lets draw the GUI
        tempRect.y=Screen.height/2 - 20;
        if (NIGUI.Button(tempRect, "Resume"))
        {
            // move back to skeleton mode.
            m_mode = SkeletonGUIModes.SkeletonMode;
            // reactivate the skeleton controllers
            foreach (NISkeletonController controller in m_controllers)
            {
                controller.SetSkeletonActive(true);
            }
 
            // set a regular smoothing
            m_settings.SmoothFactor = 0.5f;
            // deactivate NIGUI.
            NIGUI.SetActive(false);
            return;
        }
        tempRect.y = Screen.height / 2 + 30;
        if (NIGUI.Button(tempRect, "Exit"))
        {
            // note: in editor mode this doesn't really do anything...
            Application.Quit();
            return;
        }
    }
}
