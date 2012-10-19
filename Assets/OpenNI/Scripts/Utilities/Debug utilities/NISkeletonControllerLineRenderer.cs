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
using System;
using System.Collections.Generic;
using OpenNI;

///@brief Component to add debug lines on skeleton controllers
/// 
/// This class is aimed at being hanged (and operated) by a skeleton controller to draw debug lines
/// on it.
/// @ingroup OpenNIDebugUtilities
public class NISkeletonControllerLineRenderer : NISkeletonControllerLineDebugger
{

    protected override void InternalAwake()
    {
        base.InternalAwake();
        // initialize the line renderer array.
        if (m_debugLines.Count <= 0)
            return; // no lines.
        m_LineRenderers = new LineRenderer[m_debugLines.Count];
        for (int i = 0; i < m_debugLines.Count; i++)
        {
            GameObject obj = new GameObject("internal line renderer for line "+i);
            obj.transform.parent = transform;
            m_LineRenderers[i] = obj.AddComponent<LineRenderer>();
            m_LineRenderers[i].material = new Material(Shader.Find("Particles/Additive"));
            m_LineRenderers[i].SetWidth(0.2F, 0.2F);
            m_LineRenderers[i].SetColors(Color.red, Color.green); // a temporary color
            m_LineRenderers[i].SetVertexCount(2);
        }
    }

 
    protected override void DrawSingleLine(JointInfo sourceData, JointInfo targetData,int lineNumber)
    {
        m_LineRenderers[lineNumber].SetColors(GetColor(sourceData, sourceData), GetColor(targetData, targetData));
        m_LineRenderers[lineNumber].SetPosition(0, sourceData.m_jointPos);
        m_LineRenderers[lineNumber].SetPosition(1, targetData.m_jointPos);
    }

    /// Contains a line renderer for each joint
    /// @note it is the responsibility of the caller NOT to call on illegal joints.
    protected LineRenderer[] m_LineRenderers;
}
