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

/// @brief class to check OpenNI Toolkit versions.
/// 
/// This class implements the @ref NICheckVersion class for the OpenNI Utility Toolkit.
/// @note If test version fails then an exception is thrown.
/// 
/// @note This is implemented as a singleton!
/// @ingroup OpenNIBasicObjects
public class NIOpenNICheckVersion : NICheckVersion
{
    /// @brief accessor to the singleton
    public static NIOpenNICheckVersion Instance
    {
        get { return m_singleton; }
    }

    /// @copydoc NICheckVersion::ValidatePrerequisite
    /// <br>
    /// The implementation here will check the OpenNI dll version.
    public override void ValidatePrerequisite()
    {
        try
        {
            NIVersion ver = GetOpenNIDllVersion();
            if (m_minOpenNIVersion.CompareVersion(ref ver) < 0)
            {
                // we have an illegal version!
                throw new System.Exception("OpenNI version is too old (" + ver + ", need minimum " + m_minOpenNIVersion + ". Please install a new version. See documentation for more info");
            }
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("Failed to get version with message " + ex.Message + ". This probably means that OpenNI is not installed or an old version is installed, please install a new version. See documentation for more info");
        }
    }

    /// @brief Gets the current version of the OpenNI dll
    /// @return the version structure
    public NIVersion GetOpenNIDllVersion()
    {
        NIVersion ver = NIVersion.ZeroVersion;
        ver.InitFromOpenNIVersion(OpenNI.Version.Current);
        return ver;
    }
    /// @brief The minimal OpenNI version required for this package.
    protected NIVersion m_minOpenNIVersion;

    /// @brief Private constructor for singleton pattern.
    private NIOpenNICheckVersion()
    {
        m_minOpenNIVersion = new NIVersion(1, 5, 2, 23);
        m_currentVersion = new NIVersion(0, 9, 7, 4);
    }

    /// @brief Private singleton.
    private static NIOpenNICheckVersion m_singleton=new NIOpenNICheckVersion();
}

