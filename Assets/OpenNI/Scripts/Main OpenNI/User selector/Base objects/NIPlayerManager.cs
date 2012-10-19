using UnityEngine;
using System.Collections.Generic;
using OpenNI;
using System;

/// @brief Class to set the players.
/// 
/// The goal of this class is to have a list of players and to choose how these players are selected.
/// Each player will be tracking (and have a valid skeleton).
/// This class is also responsible for unselecting a user.
/// @note This is an abstract class used as an interface for all user selection implementations but
/// tries to provide as much of the internal implementation as possible. 
/// @ingroup UserSelectionModule
public abstract class NIPlayerManager : MonoBehaviour 
{
    /// This is the context we get the data from.
    public OpenNISettingsManager m_contextManager;

    /// @brief the maximum numbers of players the player manager will allow
    /// 
    /// @note This member should @b NOT be changed in runtime!
    public int m_MaxNumberOfPlayers; 

    /// true if the mapper is valid, false otherwise.
    public virtual bool Valid
    {
        get { return m_contextManager.Valid && m_contextManager.UserSkeletonValid; }
    }

    public int m_numRetries; ///< @brief Holds the number of retries if we fail to calibrate.

    /// @brief Returns the number of players currently tracking
    /// 
    /// @return The number of players currently tracking
    public virtual int GetNumberOfTrackingPlayers()
    {
        int num=0;
        foreach (NISelectedPlayer player in m_players)
        {
            if (player != null && player.Valid && player.Tracking)
            {
                num++;
            }
        }
        return num;
    }

    /// @brief Returns the number of players currently selected or tracking
    /// 
    /// @return The number of players currently selected or tracking
    public virtual int GetNumberOfSelectedPlayers()
    {
        int num = 0;
        foreach (NISelectedPlayer player in m_players)
        {
            if (player != null && player.Valid)
            {
                num++;
            }
        }
        return num;
    }

    /// @brief Mono behavior start
    public void Awake()
    {
        InitPlayers();
    }

    /// @brief Mono behavior start
    public void Start()
    {
        InternalInit();
    }

    /// @brief Method to get a specific player
    /// 
    /// This method returns the player object for a specific player. Players are numbered from 0
    /// to m_MaxNumberOfPlayers-1.
    /// 
    /// @param nPlayerNum The player number to get
    /// @return The player object. @note the player object for a specific player remains the same
    /// during runtime. Only its contents changes.
    public NISelectedPlayer GetPlayer(int nPlayerNum)
    {
        if (nPlayerNum < 0 || nPlayerNum >= m_players.Count)
            return null;
        return m_players[nPlayerNum];
    }

    /// @brief accessor to the overall number of available players (active and non active).
    public int NumPlayers
    {
        get { return m_MaxNumberOfPlayers; }
    }

    /// @brief Method to change the number of players
    /// 
    /// @note This method changes the player list. It will unselect all players and 
    /// old player references might become invalid or irrelevant! This method also assumes that the
    /// player manager has already been initialized.
    /// @param newNum The new number of players
    /// @return True on success, false otherwise.
    public virtual bool ChangeNumberOfPlayers(int newNum)
    {
        if (newNum < 0)
            return false;
        if (newNum == m_MaxNumberOfPlayers)
            return true; // nothing to do...
        for(int i=0; i<m_players.Count; i++)
        {
            UnselectPlayer(i);
        }
        if (newNum < m_MaxNumberOfPlayers)
        {
            while (m_players.Count > newNum)
            {
                m_players.RemoveAt(m_players.Count - 1);
            }
        }
        else
        {
            while (m_players.Count < newNum)
            {
                NISelectedPlayer newPlayer = new NISelectedPlayer();
                newPlayer.User = null;
                m_players.Add(newPlayer);
            }
        }
        m_MaxNumberOfPlayers = newNum;
        return true; // success
    }

    /// @brief Method to unselect a specific player
    /// 
    /// @param playerNumber the number of the player to be unselected
    /// @return True on success, false on failure
    public virtual bool UnselectPlayer(int playerNumber)
    {
        if (playerNumber < 0 || playerNumber >= m_players.Count)
            return false;
        return UnsafeUnselectPlayer(playerNumber);
    }

     /// @brief Internal Method to unselect a specific player
    /// 
    /// The difference between this method and @ref UnselectPlayer is that 
    /// this method does not check that the player number is legal but rather assumes
    /// it is legal (checked in the @ref UnselectPlayer method).
    /// @param playerNumber the number of the player to be unselected
    /// @return True on success, false on failure
    protected virtual bool UnsafeUnselectPlayer(int playerNumber)
    {
        NIPlayerCandidateObject user = m_players[playerNumber].User;
        m_players[playerNumber].User = null;
        if (user != null)
            return user.UnselectUser();
        else
            return true;
    }

    /// @brief Internal method to initialize the players list.
    /// 
    /// @note The players initialization is done during the mono behavior @ref Awake stage to enable
    /// calls to GetPlayer during the mono behavior start stage of @b other objects.
    protected virtual void InitPlayers()
    {
        // initialize the players to have an unselected user for each player.
        m_players = new List<NISelectedPlayer>();
        for (int i = 0; i < m_MaxNumberOfPlayers; i++)
        {
            NISelectedPlayer player = new NISelectedPlayer();
            player.User = null;
            m_players.Add(player);
        }
    }

    /// @brief Get a string to describe the selection type
    /// 
    /// @return The string description of the selection type
    public abstract string GetSelectionString();

    /// @brief Internal method to initialize the player manager
    /// 
    /// @note InternalInit is called from the mono behavior @ref Start method. It assumes the players 
    /// (see @ref InitPlayers) were initialized before (during @ref Awake);
    protected virtual void InternalInit()
    {
        if (m_contextManager == null)
            m_contextManager = FindObjectOfType(typeof(OpenNISettingsManager)) as OpenNISettingsManager;
        if (m_contextManager == null)
            throw new NullReferenceException("No context manager in NIPlayerManager");
        m_contextManager.UserGenrator.UserNode.NewUser += new EventHandler<NewUserEventArgs>(NewUserCallback);
        m_contextManager.UserGenrator.UserNode.LostUser += new EventHandler<UserLostEventArgs>(LostUserCallback);
        m_users = new List<NIPlayerCandidateObject>();
    }

    /// @brief Utility method to get a player from the openNI user id
    /// 
    /// @param userID The user id to look for
    /// @return The player object. If none matches, null is returned
    protected NISelectedPlayer GetPlayerFromUserID(int userID)
    {
        foreach (NISelectedPlayer player in m_players)
        {
            if (player.Valid && player.User!=null && player.User.OpenNIUserID == userID)
                return player;
        }
        return null;
    }

    /// @brief Utility method to get the player number from the user
    /// 
    /// @param user The user whose player we seek.
    /// @return The player number (-1 if not found).
    protected int GetPlayerIdFromUser(NIPlayerCandidateObject user)
    {
        for (int i = 0; i < m_players.Count; i++)
        {
            if (m_players[i].Valid && m_players[i].User == user)
                return i;
        }
        return -1;
    }
    /// @brief Utility method to get a user object from the openNI user id
    /// 
    /// @param userID The user id to look for
    /// @return The user object. If none matches, null is returned
    protected NIPlayerCandidateObject GetUserFromUserID(int userID)
    {
        foreach (NIPlayerCandidateObject user in m_users)
        {
            if (user.OpenNIUserID == userID)
            {
                return user;
            }
        }
        return null;
    }


    /// @brief a list of players. 
    /// 
    /// @note The list of players does not change during the running of the game even if @ref m_MaxNumberOfPlayers changes!
    protected List<NISelectedPlayer> m_players; 
    /// @brief a list of users states. A new element is added and an old is removed as per the identification and loss of users.
    protected List<NIPlayerCandidateObject> m_users;

    /// @brief Internal method to remove a user
    /// 
    /// @note this removes from both the players list and
    /// @param userID The user to remove.
    protected virtual void RemoveUser(int userID)
    {
        for(int i=0; i<m_players.Count; i++)
        {
            if(m_players[i].User==null)
                continue;
            if(m_players[i].User.OpenNIUserID == userID)
            {
                UnselectPlayer(i);
                break;
            }
        }
        for (int i = 0; i < m_users.Count; i++)
        {
            if (m_users[i].OpenNIUserID == userID)
            {
                m_users[i] = m_users[m_users.Count - 1];
                m_users.RemoveAt(m_users.Count - 1);
                return;
            }
        }        
    }

    /// @brief Internal method to add a new user
    ///  
    /// @param userID The ID of the user to add.
    protected virtual void AddNewUser(int userID)
    {        
        m_users.Add(GetNewUserObject(userID));
    }

    /// @brief Internal method to get a new user object
    ///  
    /// @param userID The ID of the user to add.
    /// @return The user object.
    protected virtual NIPlayerCandidateObject GetNewUserObject(int userID)
    {
        return new NIPlayerCandidateObject(m_contextManager, userID);
    }

    /// @brief callback for updating structures when a user is lost
    /// 
    /// This callback is called when a user is lost. It removes the user from all structures...
    /// @param sender who called the callback
    /// @param e the arguments of the event.
    private void LostUserCallback(object sender, UserLostEventArgs e)
    {
        m_contextManager.Log("Lost user, userID=" + e.ID, NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Skeleton, NIEventLogger.VerboseLevel.Verbose);
        RemoveUser(e.ID);
    }



    /// @brief callback for updating structures when a user is found
    /// 
    /// This callback is called when a user is found. It adds the user to the data structures (including
    /// a new unique ID).
    /// @param sender who called the callback
    /// @param e the arguments of the event.
    private void NewUserCallback(object sender, NewUserEventArgs e)
    {
        m_contextManager.Log("found new user, userID=" + e.ID, NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Skeleton, NIEventLogger.VerboseLevel.Verbose);
        AddNewUser(e.ID);
    }



}


