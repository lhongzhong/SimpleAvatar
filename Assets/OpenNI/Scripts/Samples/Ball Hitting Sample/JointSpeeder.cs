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

/// @brief A utility class to calculate the speed of a joint.
/// @ingroup OpenNISpecificLogicSamples
public class JointSpeeder : MonoBehaviour 
{
    /// mono-behavior initialization
    public void Start()
    {
        m_points = new NITimedPointSpeedListUtility(0.5f);
    }

    /// gets the normalized speed of the joint.
    /// @return the speed as a vector
    public Vector3 GetSpeed()
    {
        int numPoints;
        Vector3 totalSpeeds = m_points.GetAvgSpeed(0.5f, out numPoints);
        if (numPoints < 1)
            return Vector3.zero;
        return totalSpeeds.normalized;
    }

    /// @brief Monobehavior update
    public void Update() 
    {
        Vector3 vec = transform.position;
        m_points.AddPoint(ref vec);
	}

    /// an internal utility to get the points each update and calculate speed from them
    NITimedPointSpeedListUtility m_points;
}
