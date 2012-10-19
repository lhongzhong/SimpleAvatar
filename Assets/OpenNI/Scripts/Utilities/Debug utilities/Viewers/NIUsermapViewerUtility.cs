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

/// @brief A utility to show the users pixels
/// 
/// This is aimed as a basic utility and implementation sample to show the users pixels for debugging.
/// If one wants to show the users in the game, a better, more complete solution should be used.
/// @note this is aimed at debugging and NOT for showing the user. It is a quick implementation 
/// and not an efficient one.
/// @ingroup OpenNIViewerUtilities
public class NIUsermapViewerUtility : NIMapViewerBaseUtility 
{
    /// holds the colors to show users in. If more users exist then color then they will be cycled through.
    public List<Color> UserColors;
    /// holds the background color (the color used for the background of everything @b NOT a user).
    public Color m_backgroundColor;
    /// used to access the the data 
    protected SceneMetaData m_metaData;

    /// holds the last frame we processed. We should only change the texture if the frame changed...
    protected int m_lastProcessedImageFrameId=-1;

	protected override bool InitTexture(out Texture2D refText, out int xSize, out int ySize)
    {
        NIOpenNICheckVersion.Instance.ValidatePrerequisite();
        if (base.InitTexture(out refText, out xSize, out ySize) == false)
            return false;
        // make sure we have an image to work with
        if(m_context.UserSkeletonValid==false)
        {
            m_context.m_Logger.Log("Invalid users node", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.Skeleton, NIEventLogger.VerboseLevel.Errors);
            return false;
        }
        if(m_factor<=0)
        {
            m_context.m_Logger.Log("Illegal factor", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.Skeleton, NIEventLogger.VerboseLevel.Errors);
            return false;
        }
        // initialize the meta data object...
        m_metaData=m_context.UserGenrator.UserNode.GetUserPixels(0);
        // update the resolution by the factor
        ySize = m_metaData.YRes / m_factor;
        xSize = m_metaData.XRes / m_factor;
        // create the texture
        refText = new Texture2D(xSize, ySize, TextureFormat.RGB24, false);
        // create a new meta data object.
        return true;
	}
	
   
	protected override void CalcTexture()
    {
        base.CalcTexture();
        // we need to make sure everything is valid, otherwise we can't do anything...
        if (m_context.Valid == false || m_context.UserSkeletonValid == false)
            return; // we can't do anything...
        // check we have a new frame.
        if (m_metaData.FrameID >= m_lastProcessedImageFrameId)
        {
            m_lastProcessedImageFrameId = m_metaData.FrameID;
            WriteUserTexture();
        }
    }

    /// a method to write the image data to the texture.
    protected void WriteUserTexture()
    {

        // the size of the target data
		int i = XRes*YRes-1;
        // the array which holds the image
        UInt16MapData imageData = m_metaData.GetLabelMap();

        // loop over the target array (x and y).
		for (int y = 0; y < YRes; ++y)
		{
            for (int x = 0; x < XRes; ++x, i--) // the position is from end to start because of difference in coordinate systems
            {
                // we transform the RGB24Pixel data (Received from the image) to a Color.
                int pixel = imageData[x*m_factor,y*m_factor];
                if (pixel == 0)
                {
                    m_mapPixels[i] = m_backgroundColor;
                }
                else
                {
                    int ind = pixel % UserColors.Count;
                    m_mapPixels[i] = UserColors[ind];
                }
            }
		}

        m_mapTexture.SetPixels(m_mapPixels);
        m_mapTexture.Apply();
   }


}
