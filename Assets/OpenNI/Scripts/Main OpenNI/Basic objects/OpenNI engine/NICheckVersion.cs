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

/// @brief Base class to check versions.
/// 
/// This class provides tools to check versions. It is aimed to be inherited and specific tests 
/// implemented.<br>
/// @note if ValidatePrerequisite fails then an exception is thrown.
/// @ingroup OpenNIBasicObjects
public abstract class NICheckVersion
{
    /// @brief Internal version structure.
    /// 
    /// This structure is used to define a version and operators on it. For simplicity it can also
    /// be initialized from OpenNI.Version.
    /// @note this is a serializable structure which can be supported by the inspector
    public struct NIVersion
    {
        public const int MaxLegalMaintenance = 1000; ///< @brief Maximum allowed maintenance version when exporting to OpenNI
        public const int MaxLegalBuild = 1000; ///< @brief Maximum allowed build version when exporting to OpenNI

        /// @brief A static empty version with everything as 0.
        public static NIVersion ZeroVersion = new NIVersion(0,0,0,0);

        /// @brief Initializes the current structure from an OpenNI version.
        /// @param ver the version to initialize from.
        public void InitFromOpenNIVersion(OpenNI.Version ver)
        {
            m_major=ver.Major;
            m_minor=ver.Minor;
            m_maintenance = ver.Maintenance;
            m_build=ver.Build;
        }

        /// @brief creates an OpenNI version from the version
        /// 
        /// @return an OpenNI version representation of the version
        public OpenNI.Version ExportVersion()
        {
            OpenNI.Version ver = new OpenNI.Version();
            ver.Major = (byte)m_major;
            ver.Minor = (byte)m_minor;
            ver.Maintenance = (m_maintenance>=0 && m_maintenance<MaxLegalMaintenance) ? m_maintenance : MaxLegalMaintenance;
            ver.Build = (m_build >= 0 && m_build < MaxLegalBuild) ? m_build : MaxLegalBuild; 
            return ver;
        }

        /// @brief Constructor
        /// @param major the major version number
        /// @param minor the minor version number
        /// @param maintenance the maintenance version number
        /// @param build the build version number
        public NIVersion(int major, int minor, int maintenance, int build)
        {
            m_major = major;
            m_minor = minor;
            m_maintenance = maintenance;
            m_build = build;
        }

        /// @brief Method to compare the current version to another version.
        /// @param other the other version
        /// @return 1 if the current version is newer, -1 if the other is newer and 0 if equal
        public int CompareVersion(ref NIVersion other)
        {
            if (other.m_major != m_major)
            {
                return other.m_major < m_major ? -1 : 1;
            }
            if (other.m_minor != m_minor)
            {
                return other.m_minor < m_minor ? -1 : 1;
            }
            if (other.m_maintenance != m_maintenance)
            {
                return other.m_maintenance < m_maintenance ? -1 : 1;
            }
            if (other.m_build != m_build)
            {
                return other.m_build < m_build ? -1 : 1;
            }
            return 0;
        }

        /// @brief Creates a human readable string of the version
        /// @return the string.
        public override string ToString()
        {
            return "Version " + m_major + "." + m_minor + "." + m_maintenance + "." + m_build;
        }

        private int m_major; ///< the major number of the version
        private int m_minor; ///< the minor number of the version
        private int m_maintenance; ///< the maintenance number of the version
        private int m_build; ///< the build number of the version

    }

    /// @brief Method to make sure all prerequisites are met before running the toolkit.
    /// 
    /// This method should be called before any class derived from dll a different package is
    /// created. Its goal is to throw a human readable exception if the prerequisites are not met
    /// to enable the developer to fix the situation (i.e. install the relevant prerequisite).
    public abstract void ValidatePrerequisite();

    /// @brief Gets the current version of the Toolkit
    /// @return the version structure
    public NIVersion GetVersion()
    {
        return m_currentVersion;
    }

    /// @brief The current version of the package.
    /// @note it is the responsibility of the inheriting class to set this! 
    protected NIVersion m_currentVersion=NIVersion.ZeroVersion;
}

