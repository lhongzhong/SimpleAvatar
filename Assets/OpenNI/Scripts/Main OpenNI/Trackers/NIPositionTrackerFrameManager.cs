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
using OpenNI;

/// @brief Class to manage positions between frames.
/// 
/// This class is aimed to collect new positions along the frames.
/// At any given moment it holds a good (i.e. point we have confidence with) of the last frame
/// and the latest good (Confident) position.
/// @ingroup OpenNIPointTrackers
public class NIPositionTrackerFrameManager
{
    /// accessor to @ref m_oldFrameLastGoodPosition.
    public Vector3 LastGoodPointLastFrame
    {
        get { return m_oldFrameLastGoodPosition; }
    }

    /// accessor to @ref m_latestGoodPositionInFrame.
    public Vector3 LastGoodPoint
    {
        get { return m_latestGoodPositionInFrame; }
    }
    /// this method receives a new point (HandPointContext) and updates the structures.
    /// @param newPoint the new point
    /// @param confidence the confidence of the point.
    public void UpdatePoint(Point3D newPoint, float confidence)
    {
        if (m_lastFrame < 0)
        {
            // this is an initialization
            if (confidence < 0.5f)
                return; // bad point, nothing to do.
            m_lastFrame = Time.frameCount;
            m_latestGoodPositionInFrame = NIConvertCoordinates.ConvertPos(newPoint);
            m_oldFrameLastGoodPosition = m_latestGoodPositionInFrame;
            return;
        }
        if (Time.frameCount != m_lastFrame)
        {
            // this is a new frame.
            m_oldFrameLastGoodPosition = m_latestGoodPositionInFrame;
            m_lastFrame = Time.frameCount;
        }
        if (confidence >= 0.5f)
        {
            m_latestGoodPositionInFrame = NIConvertCoordinates.ConvertPos(newPoint);
        }
    }
    /// constructor.
    public NIPositionTrackerFrameManager()
    {
        m_oldFrameLastGoodPosition = Vector3.zero;
        m_latestGoodPositionInFrame = Vector3.zero;
        m_lastFrame = -1;
    }

    /// This holds the last good (confident) point in a PREVIOUS frame.
    protected Vector3 m_oldFrameLastGoodPosition;
    /// This holds the last good (confident) point overall.
    protected Vector3 m_latestGoodPositionInFrame;
    /// This holds the frame we calculated @ref m_latestGoodPositionInFrame.
    protected int m_lastFrame;
}
