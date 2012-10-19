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
using System.Collections;
using System.Runtime.InteropServices;
using OpenNI;

/// @brief A utility to show the depth. 
/// 
/// This is aimed as a basic utility and implementation sample to show the depth for debugging.
/// If one wants to show the depth in the game, a better, more complete solution should be used
/// @note this is aimed at debugging and NOT for showing the user. It is a quick implementation 
/// and not an efficient one
/// @ingroup OpenNIViewerUtilities
public class NIDepthmapViewerUtility : NIMapViewerBaseUtility 
{
    /// the base color we wish to use
	public Color DepthMapColor = Color.yellow;

    /// the depth map before manipulation
	protected short[] rawDepthMap;

    /// the histogram
	protected float[] depthHistogramMap;

    /// used to access the the data 
    protected DepthMetaData m_metaData;

    /// holds the last frame we processed. We should only change the texture if the frame changed...
    protected int m_lastProcessedFrameId=-1;


	protected override bool InitTexture(out Texture2D refText, out int xSize, out int ySize)
    {
        if(base.InitTexture(out refText, out xSize, out ySize)==false)
            return false;
        // make sure we have an image to work with
        if (m_context.CurrentContext.Depth == null)
        {
            m_context.m_Logger.Log("No depth", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects, NIEventLogger.VerboseLevel.Errors);
            return false;
        }

        // make sure we have an image to work with
        if(m_factor<=0)
        {
            m_context.m_Logger.Log("Illegal factor", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.Image, NIEventLogger.VerboseLevel.Errors);
            return false;
        }
        // get the resolution from the image
        MapOutputMode mom = m_context.CurrentContext.Depth.MapOutputMode;
        // update the resolution by the factor
        ySize = mom.YRes / m_factor;
        xSize = mom.XRes / m_factor;
        // create the texture
        refText = new Texture2D(xSize, ySize);
        
        // depthmap data
		rawDepthMap = new short[(int)(mom.XRes * mom.YRes)];
		// histogram stuff
		int maxDepth = m_context.CurrentContext.Depth.DeviceMaxDepth;
		depthHistogramMap = new float[maxDepth];
        NIOpenNICheckVersion.Instance.ValidatePrerequisite();
        m_metaData=new DepthMetaData();
        return true;
	}
	
	protected override void CalcTexture()
    {
 	    base.CalcTexture();
        // we need to make sure everything is valid, otherwise we can't do anything...
        if(m_context.Valid==false)
            return; // we can't do anything...
        // update the meta data
        m_context.CurrentContext.Depth.GetMetaData(m_metaData);
        // check we have a new frame.
        if (m_metaData.FrameID >= m_lastProcessedFrameId)
        {
            m_lastProcessedFrameId = m_metaData.FrameID;
            Marshal.Copy(m_context.CurrentContext.Depth.DepthMapPtr, rawDepthMap, 0, rawDepthMap.Length);
            UpdateHistogram();
            UpdateDepthmapTexture();

        }
    }

    /// @brief Internal method to update the cumulative histogram.
	protected void UpdateHistogram()
	{
		int i, numOfPoints = 0;
		
		Array.Clear(depthHistogramMap, 0, depthHistogramMap.Length);
		
		int depthIndex = 0;
		for (int y = 0; y < YRes; ++y)
		{
			for (int x = 0; x < XRes; ++x, depthIndex += m_factor)
			{
				if (rawDepthMap[depthIndex] != 0)
				{
					depthHistogramMap[rawDepthMap[depthIndex]]++;
					numOfPoints++;
				}
			}
			depthIndex += (m_factor-1)*XRes; // Skip lines
		}
        if (numOfPoints > 0)
        {
            for (i = 1; i < depthHistogramMap.Length; i++)
	        {   
		        depthHistogramMap[i] += depthHistogramMap[i-1];
	        }
            for (i = 0; i < depthHistogramMap.Length; i++)
	        {
                depthHistogramMap[i] = 1.0f - (depthHistogramMap[i] / numOfPoints);
	        }
        }
	}
	
    /// @brief Internal method to update the depth map texture before sending it to the screen.
	protected void UpdateDepthmapTexture()
    {
		// flip the depthmap as we create the texture
		int i = XRes*YRes-1;
		int depthIndex = 0;
		for (int y = 0; y < YRes; ++y)
		{
			for (int x = 0; x < XRes; ++x, --i, depthIndex += m_factor)
			{
				short pixel = rawDepthMap[depthIndex];
				if (pixel == 0)
				{
                    m_mapPixels[i] = Color.black;
				}
				else
				{
					Color c = new Color(depthHistogramMap[pixel], depthHistogramMap[pixel], depthHistogramMap[pixel], 0.9f);
                    m_mapPixels[i] = DepthMapColor * c;
				}
			}
            depthIndex += (m_factor - 1) * XRes * m_factor; // Skip lines
		}
        m_mapTexture.SetPixels(m_mapPixels);
        m_mapTexture.Apply();
   }

}
