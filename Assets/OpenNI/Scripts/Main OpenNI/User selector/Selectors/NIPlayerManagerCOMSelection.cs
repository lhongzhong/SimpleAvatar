using UnityEngine;
using System.Collections.Generic;
using OpenNI;
using System;

/// @brief Class to select players using center of mass
/// 
/// A @ref NIPlayerManager implementation which uses the position (center of mass) to select and 
/// unselect the players.
/// @ingroup UserSelectionModule
public class NIPlayerManagerCOMSelection : NIPlayerManager
{
    public float m_maxAllowedDistance = 100000.0f; ///< @brief defines a maximum distance, we can't have depth farther than this.
    public float m_failurePenalty = 10000.0f; ///< @brief Defines the penalty (in mm) for users who failed to track and have not moved.
    public float m_minCOMChangeForFailure=200; ///< @brief The minimum change in COM needed to retry a failure
    public float m_hysteresis=100; ///< @brief Holds the hysteresis used.


    protected override NIPlayerCandidateObject GetNewUserObject(int userID)
    {
        return new NIPlayerCOMCandidateObject(m_contextManager, userID);
    }

    /// @brief Mono behavior update
    public void Update()
    {
        foreach (NIPlayerCandidateObject user in m_users)
        {
            UpdateUserPriority(user);
        }

        // now we sort the users by priority
        for(int i=0; i<m_users.Count; i++)
        {
            NIPlayerCOMCandidateObject bestUser = m_users[i] as NIPlayerCOMCandidateObject;
            int bestIndex=i;
            for (int j = i + 1; j < m_users.Count; j++)
            {
                NIPlayerCOMCandidateObject curUser = m_users[j] as NIPlayerCOMCandidateObject;
                if (curUser.m_priority > bestUser.m_priority)
                {
                    bestUser = curUser;
                    bestIndex = j;
                }
            }
            if (bestIndex != i)
            {
                m_users[bestIndex] = m_users[i];
                m_users[i] = bestUser;
            }
        }
        // now that we are ordered, we need to fill the players. The first N users should be players
        // and the rest should not when N is the number of allowed players.

        int numUsersToPlayers=Math.Min(m_players.Count,m_users.Count); // the actual number of users which are players

        // first we will unselect all irrelevant players
        for(int i=numUsersToPlayers; i<m_users.Count; i++)
        {
            int playerNum=GetPlayerIdFromUser(m_users[i]);
            UnselectPlayer(playerNum);
        }

        // now we just have selected players so we need to select the good ones and put them
        // in empty places
        for(int i=0; i<numUsersToPlayers; i++)
        {
            int playerNum=GetPlayerIdFromUser(m_users[i]);
            if(playerNum>=0)
                continue; // it is already selected.
            for (int j = 0; j < m_players.Count; j++)
            {
                if (m_players[j].Valid == false)
                {
                    m_players[j].User = m_users[i];
                    m_players[j].User.SelectUser(m_numRetries);
                    break;
                }
            }
        }

    }



    public override string GetSelectionString()
    {
        return "User selector based on closest user";
    }


    /// @brief calculates an updated priority for the user
    ///  
    /// Base implementation is: priority is based on the z axis of the center of mass and the state
    /// @param user The user object whose priority we need to calculate
    protected virtual void UpdateUserPriority(NIPlayerCandidateObject user)
    {
        NIPlayerCOMCandidateObject userCom = user as NIPlayerCOMCandidateObject;
        if (userCom == null)
            return; // irrelevant
        Vector3 com=m_contextManager.UserGenrator.GetUserCenterOfMass(userCom.OpenNIUserID);
        userCom.m_priority = com.z;
        if (userCom.m_priority > 0)
        {
            userCom.m_priority = -m_maxAllowedDistance; // as far away as possible for an illegal value
        }
        if(user.PlayerStatus==UserStatus.Selected || user.PlayerStatus==UserStatus.Tracking)
        {
            userCom.m_priority += m_hysteresis;
        }
        if(userCom.PlayerStatus==UserStatus.Failure)
        {
            com -= userCom.m_COMWhenFail;
            if (com.magnitude < m_minCOMChangeForFailure)
            {
                userCom.m_priority -= m_failurePenalty;
            }
        }
    }
}
