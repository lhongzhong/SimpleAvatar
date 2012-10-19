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

/// @brief abstraction of queries
/// 
/// This class is responsible for abstracting a query limiting the nodes created
/// @note it is mono-behavior so we can drag & drop it.
/// @ingroup OpenNIBasicObjects
public class NIQuery : MonoBehaviour
{
    /// @brief Gets an appropriate query for use with the node type
    /// 
    /// @param nt The node type we need a query for
    /// @return The query for the node type (null is returned if no appropriate query is available).
    public Query GetQueryForType(NodeType nt)
    {
        if (m_queries.ContainsKey(nt))
            return m_queries[nt];
        if (m_queryDescriptions == null)
            return null;
        Query ret=null;
        for (int i = 0; i < m_queryDescriptions.Length; i++)
        {
            QueryDescription desc=m_queryDescriptions[i];
            if(desc.m_nodeType!=nt)
                continue;
            ret=CreateQueryFromDesc(ref desc);
        }
        m_queries.Add(nt, ret);
        return ret;        
    }

    /// Mono behavior awake.
    public void Awake()
    {
        NIOpenNICheckVersion.Instance.ValidatePrerequisite();
        m_queries = new Dictionary<NodeType, Query>();
        m_Initialized = true;

    }

    /// @brief This method forces an initialization of the query descriptions to base (non limiting) queries
    public void ForceInit()
    {
        m_queryDescriptions = new QueryDescription[5];
        QueryDescription desc = new QueryDescription(NodeType.Depth);
        m_queryDescriptions[0] = desc;
        desc = new QueryDescription(NodeType.Image);
        m_queryDescriptions[1] = desc;
        desc = new QueryDescription(NodeType.User);
        m_queryDescriptions[2] = desc;
        desc = new QueryDescription(NodeType.Hands);
        m_queryDescriptions[3] = desc;
        desc = new QueryDescription(NodeType.Gesture);
        m_queryDescriptions[4] = desc;

    }



    /// @brief Accessor, if this is true then the query is valid, otherwise it requires initialization.
    public virtual bool Valid
    {
        get { return m_queryDescriptions != null && m_queryDescriptions.Length > 0; }
    }

    /// @brief A constant in order to know what the maximum allowed values are for the version.
    public static readonly int[] maxVersionValues = new int[] { byte.MaxValue, byte.MaxValue, NICheckVersion.NIVersion.MaxLegalMaintenance, NICheckVersion.NIVersion.MaxLegalBuild };

    /// @brief An array which holds query descriptions to create any queries supported by this object
    public QueryDescription[] m_queryDescriptions;

    /// @brief Accessor to m_Initialized
    public bool Initialized
    {
        get { return m_Initialized; }
    }

    /// @brief makes sure we release everything on destroy
    /// 
    /// This will make sure the context is released when the object is destroyed
    /// (important for playing and stopping in the editor).
    /// @note - This is based on the MonoBehavior.OnDestroy.
    public void OnDestroy()
    {
        m_Initialized = false;
    }

    /// @brief Utility function to create a query from the description
    /// 
    /// @param desc the query description
    /// @return The result query
    protected virtual Query CreateQueryFromDesc(ref QueryDescription desc)
    {
        Query ret = new Query();
        if (desc.m_vendorName != null && desc.m_vendorName.CompareTo("") != 0)
        {
            ret.SetVendor(desc.m_vendorName);
        }
        if (desc.m_nodeName != null && desc.m_nodeName.CompareTo("") != 0)
        {
            ret.SetName(desc.m_nodeName);
        }
        if (desc.RequiresMinVersion())
        {
            ret.SetMinVersion(desc.GetMinVersion());
        }
        if (desc.RequiresMinVersion())
        {
            ret.SetMaxVersion(desc.GetMaxVersion());
        }
        return ret;
    }

    /// The query to use. For the default (null) query this is null.
    protected Query m_query;

    /// @brief A dictionary of the actual queries (to avoid the need to create them multiple times)
    protected Dictionary<NodeType, Query> m_queries=null;

    protected bool m_Initialized = false; ///< @brief internal member to check if initialized


}
