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
using System.Collections.Generic;

/// @brief Serializable class to hold the description of the query needed for a specific node type.
/// 
/// @note This is not the query itself but rather the serializable info to create the query when needed.
/// @ingroup OpenNIBasicObjects
[System.Serializable]
public class QueryDescription
{
    /// @brief Gets the minimum version
    /// 
    /// @note A value of zero in all means no limitation
    /// @return The OpenNI version of the minimum version
    public Version GetMinVersion()
    {
        Version ret = new Version();
        ret.Major = m_minVersion[0] > 0 ? (byte)m_minVersion[0] : (byte)0;
        ret.Minor = m_minVersion[1] > 0 ? (byte)m_minVersion[1] : (byte)0;
        ret.Maintenance = m_minVersion[2] > 0 ? m_minVersion[2] : 0;
        ret.Build = m_minVersion[3] > 0 ? m_minVersion[2] : 0;
        return ret;
    }

    /// @brief fills an array with the minimum version
    /// 
    /// @param verArr An int array to fill with the version information.
    public void GetMinVersionArr(ref int[] verArr)
    {
        if (verArr == null || verArr.Length != m_minVersion.Length)
            throw new System.ArgumentException("Param ver must be an array of length " + m_minVersion.Length + " integers");
        for (int i = 0; i < m_minVersion.Length; i++)
            verArr[i] = m_minVersion[i];
    }

    /// @brief Sets the minimum version from OpenNI version
    /// 
    /// @param ver The minimum version to set.
    public void SetMinVersion(Version ver)
    {
        if (ver.Major == 0 && ver.Minor == 0 && ver.Maintenance <= 0 && ver.Build <= 0)
        {
            m_minVersion[0] = -1; // a zero version!
            for (int i = 1; i < m_minVersion.Length; i++)
                m_minVersion[i] = 0;
            return;
        }
        m_minVersion[0] = ver.Major;
        m_minVersion[1] = ver.Minor;
        m_minVersion[2] = ver.Maintenance > 0 ? ver.Maintenance : 0;
        m_minVersion[3] = ver.Build > 0 ? ver.Build : 0;
    }

    /// @brief Sets the minimum version from an int array;
    /// 
    /// @param ver the version as an int array
    public void SetMinVersion(int[] ver)
    {
        if (ver == null || ver.Length != m_minVersion.Length)
            throw new System.ArgumentException("Param ver must be an array of length " + m_minVersion.Length + " integers");
        if (ver[0] < 0)
        {
            // zero out the entire version as it is illegal...
            m_minVersion[0] = -1;
            for (int i = 1; i < m_minVersion.Length; i++)
            {
                m_minVersion[i] = 0;
            }
            return;
        }
        for (int i = 0; i < m_minVersion.Length; i++)
        {
            m_minVersion[i] = ver[i] > 0 ? ver[i] : 0;
            if (m_minVersion[i] > NIQuery.maxVersionValues[i])
                m_minVersion[i] = NIQuery.maxVersionValues[i];
        }
    }

    /// @brief The maximum version
    /// 
    /// A value of Version.zero mean no limit. 
    /// A negative value on maintenance and/or build means they are ignored in the test
    /// so for example a version of major = 1, minor = 2, maintenance = 3 and build = 4
    /// means all versions prior or equal to 1.2.3.4 will be included (i.e. 1.1.6.8 will be 
    /// included but 1.3.1.1 won't). 
    /// If the build would be -1 then all 1.2.3.X versions would be legal.
    /// If maintenance was -1 then every version of 1.2.X.Y would be legal (no matter what 
    /// build is)
    /// @return The OpenNI version of the maximum version
    public Version GetMaxVersion()
    {
        Version ret = new Version();
        ret.Major = m_minVersion[0] >= 0 ? (byte)m_minVersion[0] : byte.MaxValue;
        ret.Minor = m_minVersion[1] >= 0 ? (byte)m_minVersion[1] : byte.MaxValue;
        ret.Maintenance = m_minVersion[2] >= 0 ? m_minVersion[2] : NICheckVersion.NIVersion.MaxLegalMaintenance;
        ret.Build = m_minVersion[3] >= 0 ? m_minVersion[2] : NICheckVersion.NIVersion.MaxLegalBuild;
        return ret;
    }

    /// @brief fills an array with the maximum version
    /// 
    /// @param verArr An int array to fill with the version information.
    public void GetMaxVersionArr(ref int[] verArr)
    {
        if (verArr == null || verArr.Length != m_maxVersion.Length)
            throw new System.ArgumentException("Param ver must be an array of length " + m_minVersion.Length + " integers");
        for (int i = 0; i < m_maxVersion.Length; i++)
            verArr[i] = m_maxVersion[i];
    }


    /// @brief Sets the maximum version from OpenNI version
    /// 
    /// @param ver The maximum version to set.
    public void SetMaxVersion(Version ver)
    {
        if (ver.Major == 0 && ver.Minor == 0 && ver.Maintenance <= 0 && ver.Build <= 0)
        {
            m_maxVersion[0] = -1; // a zero version!
            for (int i = 1; i < m_maxVersion.Length; i++)
            {
                m_maxVersion[i] = 0;
            }
            return;
        }
        m_maxVersion[0] = ver.Major;
        m_maxVersion[1] = ver.Minor;
        m_maxVersion[2] = ver.Maintenance;
        m_maxVersion[3] = ver.Build;
    }

    /// @brief Sets the maximum version from an int array;
    /// 
    /// @param ver the version as an int array
    public void SetMaxVersion(int[] ver)
    {
        if (ver == null || ver.Length != m_maxVersion.Length)
            throw new System.ArgumentException("Param ver must be an array of length " + m_minVersion.Length + " integers");
        if (ver[0] < 0)
        {
            // zero out the entire version as it is illegal...
            m_maxVersion[0] = -1;
            for (int i = 1; i < m_maxVersion.Length; i++)
            {
                m_maxVersion[i] = 0;
            }
            return;
        }
        for (int i = 0; i < m_maxVersion.Length; i++)
        {
            m_maxVersion[i] = (ver[i] >= 0 || ver[i] > NIQuery.maxVersionValues[i]) ? ver[i] : NIQuery.maxVersionValues[i];
        }
    }

    /// @brief Tells us if the query should have a minimum version limitation
    /// 
    /// @return true if the version requires a minimum version limitation
    public bool RequiresMinVersion()
    {
        return m_minVersion[0] >= 0;
    }


    /// @brief Tells us if the query should have a maximum version limitation
    /// 
    /// @return true if the version requires a maximum version limitation
    public bool RequiresMaxVersion()
    {
        return m_maxVersion[0] >= 0;
    }


    /// @brief Constructor
    /// 
    /// @param nodeType The node type this description relates to.
    public QueryDescription(NodeType nodeType)
    {
        m_nodeType = nodeType;
        m_vendorName = "";
        m_nodeName = "";
        m_minVersion = new int[4];
        m_minVersion[0] = -1;
        for (int i = 1; i < m_minVersion.Length; i++)
        {
            m_minVersion[i] = 0;
        }
        m_maxVersion = new int[4];
        m_maxVersion[0] = -1;
        for (int i = 1; i < m_maxVersion.Length; i++)
        {
            m_maxVersion[i] = 0;
        }
    }

    /// @brief A limit on the vendor name. null (or "") mean no limit
    [SerializeField]
    public string m_vendorName;
    /// @brief a limit on the node name, null (or "") mean no limit
    [SerializeField]
    public string m_nodeName;

    /// @brief The node type to use.
    [SerializeField]
    public NodeType m_nodeType;

    /// @brief Holds the minimum version as an array of ints
    [SerializeField]
    protected int[] m_minVersion;
    /// @brief Holds the maximum version as an array of ints
    [SerializeField]
    protected int[] m_maxVersion;
}

