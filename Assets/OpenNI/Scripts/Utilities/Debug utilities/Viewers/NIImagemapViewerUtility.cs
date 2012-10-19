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

/// @brief A utility to show the RGB image. 
/// 
/// This is aimed as a basic utility and implementation sample to show the RBG input for debugging.
/// If one wants to show the RGB image in the game, a better, more complete solution
/// should be used.
/// @note this is aimed at debugging and NOT for showing the user. It is a quick implementation 
/// and not an efficient one
/// @ingroup OpenNIViewerUtilities
public class NIImagemapViewerUtility : NIMapViewerBaseUtility 
{
    /// used to access the the data 
    protected ImageMetaData m_metaData;

    /// holds the last frame we processed. We should only change the texture if the frame changed...
    protected int m_lastProcessedImageFrameId=-1;

	protected override bool InitTexture(out Texture2D refText, out int xSize, out int ySize)
    {
        if(base.InitTexture(out refText, out xSize, out ySize)==false)
            return false;
        // make sure we have an image to work with
        if(m_context.ImageValid==false)
        {
            m_context.m_Logger.Log("Invalid image", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.Image,NIEventLogger.VerboseLevel.Errors);
            return false;
        }
        if(m_factor<=0)
        {
            m_context.m_Logger.Log("Illegal factor", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.Image, NIEventLogger.VerboseLevel.Errors);
            return false;
        }
        // get the resolution from the image
        MapOutputMode mom = m_context.Image.Image.MapOutputMode;
        // update the resolution by the factor
        ySize = mom.YRes / m_factor;
        xSize = mom.XRes / m_factor;
        // create the texture
        refText = new Texture2D(xSize, ySize, TextureFormat.RGB24, false);
        // create a new meta data object.
        NIOpenNICheckVersion.Instance.ValidatePrerequisite();
		m_metaData = new ImageMetaData();
        return true;
	}
	
   
	protected override void CalcTexture()
    {
 	    base.CalcTexture();
        // we need to make sure everything is valid, otherwise we can't do anything...
        if(m_context.Valid==false || m_context.ImageValid==false)
            return; // we can't do anything...
        // update the meta data
        m_context.Image.Image.GetMetaData(m_metaData);
        // check we have a new frame.
        if (m_metaData.FrameID >= m_lastProcessedImageFrameId)
        {
            m_lastProcessedImageFrameId = m_metaData.FrameID;
            WriteImageTexture();
        }
    }

    /// a method to write the image data to the texture.
    protected void WriteImageTexture()
    {

        // the size of the target data
		int i = XRes*YRes-1;
        // the array which holds the image
        MapData<RGB24Pixel> imageData=m_metaData.GetRGB24ImageMap();
        Color colorElement=Color.black; // just an initialization
	
        // loop over the target array (x and y).
		for (int y = 0; y < YRes; ++y)
		{
            for (int x = 0; x < XRes; ++x, i--) // the position is from end to start because of difference in coordinate systems
            {
				int ind=x*m_factor + imageData.XRes*y*m_factor; // this is the index of the current point in the original data
                // we transform the RGB24Pixel data (Received from the image) to a Color.
                RGB24Pixel pixel = imageData[ind];
                colorElement.r = ((float)pixel.Red)/255.0f;
                colorElement.g = ((float)pixel.Green)/255.0f;
                colorElement.b = ((float)pixel.Blue)/255.0f;
                m_mapPixels[i] = colorElement;
            }
		}
        m_mapTexture.SetPixels(m_mapPixels);
        m_mapTexture.Apply();
   }
}
