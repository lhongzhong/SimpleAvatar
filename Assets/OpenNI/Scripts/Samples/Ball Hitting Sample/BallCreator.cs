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
using System.Collections.Generic;

/// @brief A utility component to create balls above the skeleton.
/// 
/// This class is added to a game object to perform the creation of balls in the 
/// \"@ref OpenNISimpleGameTutorial\" sample. It will create balls which fall from above the skeleton 
/// (moved randomly to the side) at random intervals. It is also responsible for the scoring.
/// @ingroup OpenNISpecificLogicSamples
public class BallCreator : MonoBehaviour 
{
    /// the ball prefab. @note MUST be attached
    public GameObject prefab;

    /// the object we create the balls near @note MUST be attached.
    public Transform where;

    /// holds the player mapper (used to find out if we have a target player).
    public NIPlayerManager m_playerManager;

    /// the time to create the next ball
    private float m_timeToCreateNextBall;

    /// The number of balls the user hit (used for scoring)
    private int m_numBallsHit = 0;
    /// The number of balls created (used for scoring)
    private int m_numBallsCreated;

    /// Marks that the user has scored a hit (used for scoring).
    public void ScoreHit()
    {
        m_numBallsHit++;
    }

	/// mono-behavior initialization
	public void Start () {
        m_numBallsHit = 0;
        m_numBallsCreated = 0;
        m_timeToCreateNextBall = 0;
        if(m_playerManager==null)
            m_playerManager = FindObjectOfType(typeof(NIPlayerManager)) as NIPlayerManager;
	}
	
	/// mono-behavior Update is called once per frame
	public void Update () 
    {
        if (Time.time < m_timeToCreateNextBall)
            return; // we created a ball very recently, wait.
        if (m_playerManager == null)
            return; // this means we don't even have a plyer manager.
        NISelectedPlayer player = m_playerManager.GetPlayer(0);
        if (player == null || player.Valid == false || player.Tracking == false)
            return; // this means we don't have a calibrated user
        if (SkeletonGuiControl.m_mode == SkeletonGuiControl.SkeletonGUIModes.GUIMode)
            return; // we don't throw balls while in GUI mode.

        // now we know we should throw a ball. We first figure out where (a random around the
        // x axis of the "where" transform and a constant modifier on the y and z).
        Vector3 pos = where.position;
        pos.x += Random.Range(- 2.0f, 2.0f);
        pos.y += 8.0f;
        pos.z += 2.1f;
        // create the ball
        Instantiate(prefab, pos, Quaternion.identity);

        m_numBallsCreated++;
        // we set the time for the next ball. The time itself depends on how many balls were created
        // (the more balls, the less time on average).
        float maxTime = 5.0f;
        float minTime = 1.0f;
        if (m_numBallsCreated > 5)
            maxTime = 4.0f;
        if (m_numBallsCreated > 10)
            maxTime = 3.0f;
        if (m_numBallsCreated > 15)
            minTime = 0.5f;
        if (m_numBallsCreated > 20)
            maxTime = 2.0f;
        m_timeToCreateNextBall = Time.time + Random.Range(minTime,maxTime);
	}

    /// mono-behavior OnGUI shows the scoring
    void OnGUI()
    {
        if (SkeletonGuiControl.m_mode == SkeletonGuiControl.SkeletonGUIModes.GUIMode)
            return; // we don't draw score while in GUI mode.
        GUI.Box(new Rect(Screen.width/2 -100, 10, 200, 20), "You Hit " + m_numBallsHit + " balls of " + m_numBallsCreated);
    }
    
}
