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

/// @brief Collider to be attached to a joint. 
/// 
/// This will make the joint apply force on the object it hits
/// @ingroup OpenNISpecificLogicSamples
public class NICollider : MonoBehaviour 
{
    /// this sets a time, after the collision where the object it attached to will be destroyed.
    private float timeToDestroy;

    /// the score board used. 
    public BallCreator m_scoreBoard;

    /// A ball can be hit just once. This variable is initialized to false and when a collision
    /// occurs it sets it to true. When this happens, no more collisions are allowed (all
    /// are ignored).
    private bool SentHitMessage;

    /// mono-behavior initialization
	void Start () 
    {
        SentHitMessage = false;
        timeToDestroy = float.MaxValue;
        if (m_scoreBoard == null)
            m_scoreBoard = FindObjectOfType(typeof(BallCreator)) as BallCreator;
	}

	/// mono-behavior update, called each frame.
	void Update () 
    {
        // destroy the ball when it is either too low (below the floor) or enough
        // time has passed.
        if (Time.time>timeToDestroy || transform.position.y<-10)
            DestroyImmediate(gameObject);
	}

    /// mono-behavior trigger - this is called when we have a collision.
    /// 
    /// @param collisionInfo The object we collide with.
    public void OnTriggerEnter(Collider collisionInfo)
    {
        // make sure we are hit just once.
        if (SentHitMessage)
            return;
        SentHitMessage = true;
        JointSpeeder speeder=collisionInfo.gameObject.GetComponent(typeof(JointSpeeder)) as JointSpeeder;
        if (speeder == null)
            return;
        Vector3 speed = speeder.GetSpeed();
        // we limit the speed to the area relevant. We can't have too much speed in the x direction
        // or it will fly strongly to the left and right. We have even more limiting speed in the y
        // direction as we want it to drop down (to high speed and it will come down too far) and not
        // drop down too fast. The limit on the z is to make sure it is hit forward and never backwards.
        speed.x = Mathf.Clamp(speed.x, -0.7f, 0.7f);
        speed.y = Mathf.Clamp(speed.y, -0.3f, 0.5f);
        speed.z = Mathf.Max(speed.z, 0.3f);

        rigidbody.AddForce(speed*70,ForceMode.Impulse);
        timeToDestroy = Time.time + 3.0f;
        if (m_scoreBoard != null)
            m_scoreBoard.ScoreHit();
    }
}
