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
using OpenNI;
/// @brief This class is aimed at converting the coordinates from a Point3D to a Vector3 
/// @ingroup OpenNIBasicObjects
public static class NIConvertCoordinates
{
    /// @brief Converts a Point3D position in sensor coordinates to a Vector3 in Unity coordinates
    /// 
    /// This method receives a position in the sensor coordinates (e.g. from the skeleton) and
    /// converts it to a Vector3 in Unity coordinates. 
    /// @param pos the position we wish to convert
    /// @return the converted position
    public static Vector3 ConvertPos(Point3D pos)
    {
        Vector3 vec = Vector3.zero;
        Quaternion rotator = Quaternion.identity;
        // calculates the rotation from the floor normal to the up. This is how to change.
        rotator.SetFromToRotation(m_floorNormal, Vector3.up);

        vec.x = pos.X;
        vec.y = pos.Y;
        vec.z = -pos.Z; // the Unity z axis is the opposite direction from the sensor's
        vec = rotator * vec;
        return vec;
    }


    /// This method updates the floor normal.
    /// @param newFloorNormal the new normal
    /// @param overrideNormal if this is false then the normal will only 
    /// be updated if it has never been updated before
    public static void UpdateFloorNormal(Vector3 newFloorNormal, bool overrideNormal)
    {
        if (m_normalUpdated == false || overrideNormal)
        {
            m_floorNormal = newFloorNormal.normalized;
            m_normalUpdated = true;
        }
    }

    /// resets the normal (and the updated info) to its starting values. This should be used
    /// whenever the sensor changes its angle.
    public static void ResetFloorNormal()
    {
        m_normalUpdated = false;
        m_floorNormal = Vector3.up;
    }

    /// Accessor to see if the normal is updated.
    public static bool NormalUpdated
    {
        get { return m_normalUpdated; }
    }

    /// @brief the floor's normal in the sensor's coordinate system.
    /// 
    /// This member represents the normal of the floor in the sensor's coordinate system. <br>
    /// If no data is available, it is assumed that the value is simply up. <br>
    /// It is very common for the sensor to have a pitch angle (i.e. to look a little bit up or down
    /// instead of being perfectly parallel to the floor). This can causes the height data (y axis) of
    /// objects detected to change considerably when moving forward or backwards. In many cases this 
    /// is not a big problem but if it is, the best way to correct it is to find out the floor's normal
    /// in the sensor's coordinates and correct according to it (which is done in @ref ConvertPos).<br>
    /// a possible way to correct the normal is to figure it out from the calibration pose (as we 
    /// assume that the floor is leveled and the user is standing erect).
    private static Vector3 m_floorNormal = Vector3.up;
    /// holds true if the normal was changed 
    private static bool m_normalUpdated = false; 
}

