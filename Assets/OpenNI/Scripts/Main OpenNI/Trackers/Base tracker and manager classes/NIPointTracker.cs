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

/// @brief A base class for point trackers
/// 
/// This class is a base class for all point trackers and provides utilities for tracking the 
/// position (both smoothed out and Raw) as well as allowing to add gestures to it.
/// @ingroup OpenNIPointTrackers
public abstract class NIPointTracker : MonoBehaviour 
{
    /// a link to the object with the NI context. We will be following the user tracking from here.
    public OpenNISettingsManager m_context;

    /// mono-behavior initialization
    /// @note if one needs to do initialization, override @ref InternalAwake instead of 
    /// using this...
    public void Awake()
    {
        InternalAwake();
    }

    /// returns a unique name for the tracker type.
    /// @note this is what is used to identify the tracker
    /// @return the unique name.
    public abstract string GetTrackerType();


    /// return the last smooth good position (preferably from the current frame...)
    public abstract Vector3 CurPos
    {
        get;
    }

    /// return the last raw good position (preferably from the current frame...)
    public abstract Vector3 CurPosRaw
    {
        get;
    }

    /// return the delta of the last smooth good position (preferably from the current frame...) from the last
    /// frame's good position.
    public abstract Vector3 CurDeltaPos
    {
        get;
    }

    /// return the delta of the last Raw good position (preferably from the current frame...) from the last
    /// frame's good position.
    public abstract Vector3 CurDeltaPosRaw
    {
        get;
    }


    /// return the delta of the last smooth good position (preferably from the current frame...) from the 
    /// starting position
    public abstract Vector3 CurPosFromStart
    {
        get;
    }

    /// return the delta of the last Raw good position (preferably from the current frame...) from the 
    /// starting position
    public abstract Vector3 CurPosFromStartRaw
    {
        get;
    }


    /// returns the initial position of the point.
    /// @note in the basic implementation this is the focus point (as that is the initial position
    /// of the primary point when using focus). 
    public abstract Vector3 StartingPos
    {
        get;
    }

    /// tells us if we are valid
    public bool Valid
    {
        get { return m_valid; }
    }

    /// @brief performs the initialization
    /// 
    /// This method performs the initialization of the hand to a specific context.
    /// @note in most cases child classes should NOT override this method but rather instead
    /// override InitInternalStructures
    /// @param newContext the context to use.
    /// @return true on success, false on failure.
    public virtual bool InitTracking(OpenNISettingsManager newContext)
    {
        StopTracking(); // to make sure it is released.
        if (InitContext(newContext) == false)
        {
            StopTracking();
            return false;
        }

        if (InitInternalStructures() == false)
        {
            StopTracking();
            return false;
        }
        m_valid=true;
        return true;
    }
    
  
    /// @brief makes sure we release everything on quit
    /// 
    /// This will make sure everything the context is released when quitting (important for playing and
    /// stopping in the editor).
    /// @note - This is based on the MonoBehavior.OnAppliactionQuit.
    public void OnApplicationQuit()
    {
        StopTracking();
    }

    /// @brief makes sure we release everything on destroy
    /// 
    /// This will make sure everything released when the object is destroyed
    /// (important for playing and stopping in the editor).
    /// @note - This is based on the MonoBehavior.OnDestroy.
    public void OnDestroy()
    {
        StopTracking();
    }


    /// @brief Releases all internal objects.
    /// 
    /// Release all internal objects in a reverse order to initialization.
    /// @note after releasing this object becomes uninitialized!!!
    /// @note any who plan to override this must call the base at the END of the method (as
    /// InitTracking changes the context to null!).
    public virtual void StopTracking()
    {
        m_valid = false;
        m_context = null;
    }



    // protected members

    /// holds true if we are initialized
    protected bool m_valid;
    
    // protected methods

    /// an internal method to initialize the context. 
    /// @param newContext the context to initialize
    /// @return true on success (which also initializes the context) and false on failure 
    /// (releasing is the responsibility of the caller)
    protected virtual bool InitContext(OpenNISettingsManager newContext)
    {
        // checks we have a context and hands.
        if (newContext.Valid == false)
        {
            newContext.Log("received invalid context!", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.Hands, NIEventLogger.VerboseLevel.Errors);
            return false;
        }
        m_context = newContext;
        return true;
    }

    /// an internal method to initialize the internal structures
    /// @note in most cases, inheriting methods should NOT override the base @ref InitTracking but
    /// rather override this method.
    /// @return true on success and false otherwise.
    protected abstract bool InitInternalStructures();

    /// this method does initialization when the Awake method is called. It should be overriden instead
    /// of Awake...
    protected virtual void InternalAwake()
    {
        m_valid = false;
        if (m_context == null)
            m_context = FindObjectOfType(typeof(OpenNISettingsManager)) as OpenNISettingsManager;
    }
}
