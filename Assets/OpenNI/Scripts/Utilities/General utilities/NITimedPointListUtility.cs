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


/// @brief Utility to hold and analyze points over time.
/// 
/// This class is a utility for handling a list of points (each with a time on it).
/// It provides various analysis elements (e.g. average)
/// @ingroup OpenNIGeneralUtilities
public class NITimedPointListUtility 
{
    /// A struct to hold points and times
    public struct TimedPoint
    {
        public Vector3 m_point; ///< The position
        public float m_time;    ///< The time it was recorded
    }


    /// Calculates the average position over the points. 
    /// @param timeLength the length of time to check
    /// @note if timeLength is larger than the maximum time defined then the maximum time is used
    /// @param[out] numPoints the number of points we did the calculation on
    /// @return the average position
    public virtual Vector3 GetAvgPos(float timeLength, out int numPoints)
    {
        return GetAvgPos(timeLength, m_points, out numPoints);
    }

    /// Calculates the standard deviation of the position over the points separatly for each axis
    /// @param timeLength the length of time to check
    /// @note if timeLength is larger than the maximum time defined then the maximum time is used
    /// @param[out] numPoints the number of points we did the calculation on
    /// @return the standard deviation squared
    public virtual Vector3 GetStdDeviationSqr(float timeLength, out int numPoints)
    {
        return GetStdDeviationSqr(timeLength, m_points, out numPoints);
    }

    /// Adds a point to the queue.
    /// @param point the point to add
    public virtual void AddPoint(ref Vector3 point)
    {
        ClearTimes(m_points,m_maxTimeToRemember);
        TimedPoint newPoint;
        newPoint.m_point=point;
        newPoint.m_time=Time.time;
        m_points.Enqueue(newPoint);
    }

    /// Clears the points
    /// @param timeToClear the time to clear (a negative time would clear everything...)
    public virtual void Clear(float timeToClear)
    {
        ClearTimes(m_points, timeToClear);
    }

    /// Constructor
    /// @param maxTimeToRemember the maximum time (in seconds) to remember elements in the queue. points
    /// older than this value will be removed.
    public NITimedPointListUtility(float maxTimeToRemember)
    {
        m_maxTimeToRemember = maxTimeToRemember;
        m_points = new Queue<TimedPoint>();
    }

    /// Used to get a debug string (i.e. a human readable information chart)
    /// @return the human readable debug string
    public virtual string GetDebugString()
    {
        string str = "used "+m_points.Count+" timeToRemember=" + m_maxTimeToRemember + " points=";
        int numPnt=0;
        foreach (TimedPoint pnt in m_points)
        {
            str += "" + numPnt + ": " + pnt.m_point + " at" + pnt.m_time + ", ";
            numPnt++;
        }
        return str;
    }

    /// This method dequeues old times
    /// @param points the points to do the clearing on
    /// @param timeToClear the time to clear (a negative time would clear everything...)
    protected void ClearTimes(Queue<TimedPoint> points, float timeToClear)
    {
        while (points.Count > 0 && points.Peek().m_time + m_maxTimeToRemember < Time.time)
        {
            points.Dequeue();
        }
    }

    /// This holds the maximum time (in seconds) to remember elements in the queue
    /// points older than this value will be removed.
    protected float m_maxTimeToRemember;


    /// Calculates the average position over the points. 
    /// This is an internal version of the method which receives the points as input
    /// @param timeLength the length of time to check
    /// @param points the points to average
    /// @note if timeLength is larger than the maximum time defined then the maximum time is used
    /// @param[out] numPoints the number of points we did the calculation on
    /// @return the average position
    protected virtual Vector3 GetAvgPos(float timeLength, Queue<TimedPoint> points, out int numPoints)
    {
        ClearTimes(points,m_maxTimeToRemember);
        numPoints = points.Count;
        Vector3 res = Vector3.zero;
        int num = 0;
        foreach (TimedPoint pnt in points)
        {
            if (pnt.m_time + timeLength >= Time.time)
            {
                res += pnt.m_point;
                num++;
            }
        }
        if (num == 0)
            return res;
        return res / (float)num;
    }

    /// Calculates the standard deviation of the position over the points separatly for each axis
    /// This is an internal version of the method which receives the points as input
    /// @param timeLength the length of time to check
    /// @param points the points to average
    /// @note if timeLength is larger than the maximum time defined then the maximum time is used
    /// @param[out] numPoints the number of points we did the calculation on
    /// @return the standard deviation squared
    protected virtual Vector3 GetStdDeviationSqr(float timeLength,Queue<TimedPoint> points, out int numPoints)
    {
        Vector3 res = Vector3.zero;
        Vector3 avg = GetAvgPos(timeLength, out numPoints); // note this also clears the times
        int num = 0;
        foreach (TimedPoint pnt in points)
        {
            if (pnt.m_time + timeLength >= Time.time)
            {
                res += Vector3.Scale(pnt.m_point - avg, pnt.m_point - avg);
                num++;
            }
        }
        if (num == 0)
            return res;
        return res / (float)num;
    }

    /// Holds the point queue.
    protected Queue<TimedPoint> m_points;
}
