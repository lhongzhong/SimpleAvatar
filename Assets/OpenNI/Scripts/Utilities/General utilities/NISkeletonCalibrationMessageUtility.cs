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

/// @brief A small utility which prompts users to calibrate.
/// 
/// Used mainly for debugging.
/// @ingroup OpenNIGeneralUtilities
public class NISkeletonCalibrationMessageUtility : MonoBehaviour 
{
    /// @brief this holds the player manager. 
    public NIPlayerManager m_playerManager;

    /// @brief A string describing what to do to be selected
    public string m_actionToSelect;

    /// @brief If this is true then the message will be seen as long as there is at least one unselected 
    /// player and if false the message will only be seen if no players are selected.
    public bool m_AllPlayersMessage;

    /// @brief The image of the calibration pose.
    public Texture2D m_Image;

    /// mono-behavior initialization
	void Start () 
    {
        if (m_playerManager == null)
        {
            m_playerManager = FindObjectOfType(typeof(NIPlayerManager)) as NIPlayerManager;
        }

        m_basePos=new Rect(0, Screen.height/2,150,30);
        if (m_actionToSelect == null)
        {
            m_actionToSelect = "";
        }
        m_actionToSelectRectWidth = m_actionToSelect.Length * 7;
	}

    /// mono-behavior GUI drawing
    void OnGUI()
    {
        Rect curPos = m_basePos;

        if (m_playerManager == null || m_playerManager.Valid==false)
            return; // no player manager so nothing to do...

        int numTracking = m_playerManager.GetNumberOfTrackingPlayers();

        if(m_AllPlayersMessage && numTracking>=m_playerManager.m_MaxNumberOfPlayers)
            return; // all players are tracking, nothing to do here.
        if (!m_AllPlayersMessage && numTracking > 0)
            return; // at least one player is tracking and we don't want to show the message to the rest

        // reaching here means we have a valid player mapper with no calibrated users, we need to 
        // show a message...
        int numUnselected = 0;
        for (int i = 0; i < m_playerManager.m_MaxNumberOfPlayers; i++)
        {
            NISelectedPlayer player = m_playerManager.GetPlayer(i);
            if (player == null || player.Valid == false)
            {
                GUI.Box(curPos, "Player " + i + " is unselected.");
                numUnselected++;
            }
            else if (player.Tracking == false)
            {
                GUI.Box(curPos, "Player " + i + " is calibrating.");
            }
            else
                continue;
            curPos.y += 35;
            
        }

        if (numUnselected == 0)
            return;
        if(m_actionToSelect.CompareTo("")!=0)
        {
            curPos.width = m_actionToSelectRectWidth;
            GUI.Box(curPos, m_actionToSelect);
            curPos.y += 35;
        }
        
        if (m_Image != null)
        {
            curPos.width = 128;
            curPos.height = 128;
            GUI.Box(curPos, m_Image);
        }
    }

    /// @brief The base position for the first GUI drawing.
    protected Rect m_basePos;

    /// @brief The width needed for the action message;
    protected int m_actionToSelectRectWidth;
    
}
