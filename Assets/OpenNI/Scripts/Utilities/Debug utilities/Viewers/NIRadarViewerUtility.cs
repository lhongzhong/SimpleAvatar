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

/// @brief A debug utility to show users
/// 
/// A basic utility to show users which were identified. The user's color can be set to change when
/// they are uncalibrated, in the middle of calibrating and calibrated.
/// @ingroup OpenNIViewerUtilities
public class NIRadarViewerUtility : MonoBehaviour
{
    /// a link to the object with the NI context
    public OpenNISettingsManager m_context;
    /// the image will appear inside this rectangle.
    public Rect m_placeToDraw;
	

	
	/// @brief tells us how to handle @ref m_placeToDraw. 
    /// 
    /// The nearest corner central position (x,y) of @ref m_placeToDraw is considered to be relative 
    /// to the corner of the snap so that the relevant corner will be of that distance from the 
    /// corner of the screen.
	public NIMapViewerBaseUtility.ScreenSnap m_snap;
	
    /// @brief This limits the area (in real world coordinates) where we look for the users. 
    /// 
    /// This is the a normalization for the x and z coordinates of a user's center of mass inside
    /// the radar viewer rectangle. So a value of 1000,2000 would mean users which are inside the
    /// +/- 0.5m x from the center of the sensor and up to 2m away from the sensor's z axis will
    /// be viewer linearly in the radar rectangle and those outside the area will be stuck to one
    /// of the sides.
    public Vector2 m_radarRealWorldDimensions = new Vector2(4000, 4000);

    /// The color used to show uncalibrated users
    public Color m_UncalibratedColor=Color.red;
    /// The color used to show users currently in the calibration process
    public Color m_CalibratingColor=Color.magenta;
    /// The color used to show users already calibrated
    public Color m_CalibratedColor = Color.yellow;
    /// The color used to show users currently tracking
    public Color m_TrackingColor = Color.green;


    private GUIStyle m_style; ///< @brief Internal GUI style used to draw the box (sets the texture)
    private Texture2D m_texture; ///< @brief Internal texture to draw a specific color 


    /// mono-behavior initialization
    void Start()
    {
        m_style = new GUIStyle();
        m_texture = new Texture2D(1, 1);
		if (m_context == null)
            m_context = FindObjectOfType(typeof(OpenNISettingsManager)) as OpenNISettingsManager;

    }

    /// mono-behavior GUI drawing
    void OnGUI()
    {
		Rect posToPut=m_placeToDraw;
		switch(m_snap)
		{
            case NIMapViewerBaseUtility.ScreenSnap.UpperRightCorner:
			{
				posToPut.x=Screen.width-m_placeToDraw.x-m_placeToDraw.width;
				break;
			}
            case NIMapViewerBaseUtility.ScreenSnap.LowerLeftCorner:
			{
				posToPut.y=Screen.height-m_placeToDraw.y-m_placeToDraw.height;
				break;
			}
            case NIMapViewerBaseUtility.ScreenSnap.LowerRightCorner:
			{
				posToPut.x=Screen.width-m_placeToDraw.x-m_placeToDraw.width;
				posToPut.y=Screen.height-m_placeToDraw.y-m_placeToDraw.height;
				break;
			}
			
		}
		GUI.BeginGroup(posToPut);
        GUI.Box(new Rect(0, 0, m_placeToDraw.width, m_placeToDraw.height), "Users Radar");


        if (m_context.Valid == false || m_context.UserSkeletonValid == false)
        {
            GUI.EndGroup();
            return;
        }

        int[] users = m_context.UserGenrator.GetUserIds();
        foreach (int userId in users)
        {
            // normalize the center of mass to radar dimensions
            Vector3 com = m_context.UserGenrator.GetUserCenterOfMass(userId);
            Vector2 radarPosition = new Vector2(com.x / m_radarRealWorldDimensions.x, -com.z / m_radarRealWorldDimensions.y);

            // X axis: 0 in real world is actually 0.5 in radar units (middle of field of view)
            radarPosition.x += 0.5f;

            // clamp
            radarPosition.x = Mathf.Clamp(radarPosition.x, 0.0f, 1.0f);
            radarPosition.y = Mathf.Clamp(radarPosition.y, 0.0f, 1.0f);

            // we always want the radar to mirror the view, even if the depth doesn't
            if (!m_context.Mirror)
            {
                radarPosition.x = 1.0f - radarPosition.x;
            }

            // draw the user
            Color newColor = m_UncalibratedColor;
            if(m_context.UserGenrator.IsTracking(userId))
            {
                newColor = m_TrackingColor;
            }
            else if(m_context.UserGenrator.IsCalibrated(userId))
            {
                newColor = m_CalibratedColor;
            }
            else if(m_context.UserGenrator.IsCalibrating(userId))
            {
                newColor = m_CalibratingColor;
            }
            m_texture.SetPixel(0, 0, newColor);
            m_texture.Apply();
            m_style.normal.background = m_texture;
            GUI.Box(new Rect(radarPosition.x * m_placeToDraw.width - 10, radarPosition.y * m_placeToDraw.height - 10, 20, 20), userId.ToString(),m_style);
        }
        GUI.EndGroup();
    }
}
