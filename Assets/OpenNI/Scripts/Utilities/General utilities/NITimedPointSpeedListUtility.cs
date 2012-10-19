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


/// @brief utility to hold points and speeds over time.
/// 
/// This class is a utility for handling a list of points (each with a time on it) and derives 
/// speeds for it. It provides various analysis elements (e.g. average).
/// @ingroup OpenNIGeneralUtilities
/// 
public class NITimedPointSpeedListUtility : NITimedPointListUtility 
{
    /// adds a point to the queue.
    /// @param point the point to add
    public override void AddPoint(ref Vector3 point)
    {
        ClearTimes(m_speeds,m_maxTimeToRemember);
        if (m_points.Count > 0)
        {
            TimedPoint pnt = m_points.Peek();
            if (pnt.m_time + m_maxTimeToRemember > Time.time)
            {
                if (Time.time == pnt.m_time)
                {
                    pnt.m_point = Vector3.zero;
                }
                else
                {
                    pnt.m_point -= point;
                    pnt.m_point /= (Time.time - pnt.m_time);
                    pnt.m_time = Time.time;
                    m_speeds.Enqueue(pnt);
                }
                
            }
        }        
        base.AddPoint(ref point);
    }

    /// calculates the average speed over the points. 
    /// @param timeLength the length of time to check
    /// @note if timeLength is larger than the maximum time defined then the maximum time is used
    /// @param[out] numPoints the number of points we did the calculation on
    /// @return the average speed
    public virtual Vector3 GetAvgSpeed(float timeLength, out int numPoints)
    {
        ClearTimes(m_speeds, m_maxTimeToRemember);
        return GetAvgPos(timeLength, m_speeds, out numPoints);
    }

    /// calculates the standard deviation of the position over the points separatly for each axis
    /// @param timeLength the length of time to check
    /// @note if timeLength is larger than the maximum time defined then the maximum time is used
    /// @param[out] numPoints the number of points we did the calculation on
    /// @return the standard deviation squared
    public virtual Vector3 GetStdDeviationSqrSpeed(float timeLength, out int numPoints)
    {
        ClearTimes(m_speeds, m_maxTimeToRemember);
        return GetStdDeviationSqr(timeLength, m_speeds, out numPoints);
    }

    /// clears the points
    /// @param timeToClear the time to clear (a negative time would clear everything...)
    public override void Clear(float timeToClear)
    {
        base.Clear(timeToClear);
        ClearTimes(m_speeds, timeToClear);
    }

    /// used to get a debug string (i.e. a human readable information chart)
    /// @return the human readable debug string
    public override string GetDebugString()
    {
        string str = base.GetDebugString();
        str+="\nSPEED:\n used "+m_points.Count+" timeToRemember=" + m_maxTimeToRemember + " points=";
        int numPnt=0;
        foreach (TimedPoint pnt in m_points)
        {
            str += "" + numPnt + ": " + pnt.m_point + " at" + pnt.m_time + ", ";
            numPnt++;
        }
        return str;
    }


    /// constructor
    /// @param maxTimeToRemember the maximum time (in seconds) to remember elements in the queue. points
    /// older than this value will be removed.
    public NITimedPointSpeedListUtility(float maxTimeToRemember)
        : base(maxTimeToRemember)
    {
        m_speeds = new Queue<TimedPoint>();
    }

    /// holds the speeds queue.
    protected Queue<TimedPoint> m_speeds;
}
