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

/// @brief A Natural Interactions aware Input implementation. 
/// 
/// This class aims to behave exactly the same as the regular Input class except for supporting
/// axes and capabilities requiring Natural Interactions.
/// @ingroup OpenNIInput
public class NIInput : MonoBehaviour
{
    /// this will hold our axes information (public to be available to the inspector)
    public List<NIAxis> m_axisList;

    /// the hands tracking manager to pick the axis
    public NIPointTrackerManager m_pointsTrackingManager;

    /// the gesture manager to use
    public NIGestureManager m_gestureManager;


	/// initialization
	void Start () 
    {
        if (m_pointsTrackingManager == null)
            throw new System.Exception("Must define a hand tracking manager! - create a new game object and attach NIInputHandsManager script to it. Add some hand trackers and drag to the input");
        if (m_gestureManager == null)
            throw new System.Exception("Must define a gesture tracking manager! - create a new game object and attach NIGestureManager script to it. Add some gesture factories and drag to the input");
        foreach (NIAxis axis in m_axisList)
        {
            if (m_pointsTrackingManager.m_trackers.Length <= axis.m_sourceTrackerIndex)
                throw new System.Exception("Axis " + axis.m_axisName + " has an illegal point tracker!");
            axis.m_sourceTracker = m_pointsTrackingManager.GetTracker(axis.m_sourceTrackerString);
            if (axis.m_sourceTracker == null)
                throw new System.Exception("Axis does not have a hand to follow!!!");

            if (axis.m_gestureIndex < m_gestureManager.m_gestures.Length)
            {
                axis.m_sourceGesture = m_gestureManager.m_gestures[axis.m_gestureIndex].GetGestureTracker(axis.m_sourceTracker);
                if (axis.m_sourceGesture == null)
                    throw new System.Exception("Gesture type does not match hand tracker type");
            }
            else
                axis.m_sourceGesture = null;
        }
	}

    /// @brief A method to release the trackers of all axes.
    protected virtual void Release()
    {
        foreach (NIAxis axis in m_axisList)
        {
            if (axis.m_sourceGesture != null)
            {
                if(m_gestureManager != null)
                    m_gestureManager.m_gestures[axis.m_gestureIndex].ReleaseTracker(axis.m_sourceGesture);
                axis.m_sourceGesture = null;
            }
            if (m_pointsTrackingManager != null) 
                m_pointsTrackingManager.ReleaseTracker(axis.m_sourceTrackerString);
            axis.m_sourceTracker = null;
        }
    }


    /// @brief makes sure we release everything on quit
    public void OnApplicationQuit()
    {
        Release();
    }

    /// @brief makes sure we release everything on destroy
    public void OnDestroy()
    {
        Release();
    }
    
    /// works just like Input.GetAxis
    /// @param name the name of the axis
    /// @return the axis value
    public float GetAxis(string name)
    {
        return GetAxisInternal(name,false);
    }

    /// works just like Input.GetAxisRaw
    /// @param name the name of the axis
    /// @return the axis value
    public float GetAxisRaw(string name)
    {
        return GetAxisInternal(name,true);
    }


    /// This method tells us if a gesture on a specific axis has fired since the last update
    /// @param lastTime the time (from Time.time) when we start looking.
    /// @param axisName the axis name we want to check the gesture on
    /// @return true if at least one of the gestures belonging to axis of that name has been detected
    /// from the lastTime.
    public bool HasFiredSinceTime(float lastTime, string axisName)
    {
        foreach(NIAxis axis in m_axisList)
        {
            if(axis.m_axisName.CompareTo(axisName)!=0)
                continue;
            if(axis.m_sourceGesture==null)
                continue;
            return axis.m_sourceGesture.GetLastGestureTime() > lastTime;
        }
        return false; 
    }

    /// This method allows us to register for a callback whenever the axis fires.
    /// @note it is the responsibility of the caller to unregister using @ref UnRegisterCallbackForGesture
    /// @param eventDelegate the delegate to be called
    /// @param axisName the axis name we want to check the gesture on
    public void RegisterCallbackForGesture(NIGestureTracker.GestureEventHandler eventDelegate, string axisName)
    {
        foreach(NIAxis axis in m_axisList)
        {
            if(axis.m_axisName.CompareTo(axisName)!=0)
                continue;
            if(axis.m_sourceGesture==null)
                continue;
            axis.m_sourceGesture.m_gestureEventHandler += eventDelegate;
        }
    }

    /// This method allows us to unregister a callback previously registered using @ref RegisterCallbackForGesture
    /// @param eventDelegate the delegate to be called
    /// @param axisName the axis name we want to check the gesture on
    public void UnRegisterCallbackForGesture(NIGestureTracker.GestureEventHandler eventDelegate, string axisName)
    {
        foreach (NIAxis axis in m_axisList)
        {
            if (axis.m_axisName.CompareTo(axisName) != 0)
                continue;
            if (axis.m_sourceGesture == null)
                continue;
            axis.m_sourceGesture.m_gestureEventHandler -= eventDelegate;
        }
    }


    // the following functions are used only to call the regular input!

    /// same as Input.GetButton but also returns true if an axis has a gesture in progress
    /// @param buttonName see Input documentation
    /// @return see Input documentation
    public bool GetButton(string buttonName)
    {
        foreach (NIAxis axis in m_axisList)
        {
            if (axis.m_axisName.CompareTo(buttonName) != 0)
                continue;
            if (axis.m_sourceGesture == null)
                continue;
            if(axis.m_sourceGesture.GestureInProgress()>=1)
                return true;
        }
        return Input.GetButton(buttonName);
    }

    /// same as Input.GetButtonDown but also returns true if an axis has a gesture which started this frame
    /// @param buttonName see Input documentation
    /// @return see Input documentation
    public bool GetButtonDown(string buttonName)
    {
        foreach (NIAxis axis in m_axisList)
        {
            if (axis.m_axisName.CompareTo(buttonName) != 0)
                continue;
            if (axis.m_sourceGesture == null)
                continue;
            if (axis.m_sourceGesture.GetLastGestureTime()==Time.frameCount)
                return true;
        }
        return Input.GetButtonDown(buttonName);
    }

    /// same as Input.GetButtonUp
    /// @param buttonName see Input documentation
    /// @return see Input documentation
    public bool GetButtonUp(string buttonName)
    {
        return Input.GetButtonUp(buttonName);
    }

    /// simply called the regular input
    /// @param buttonName see Input documentation
    /// @return see Input documentation
    public bool GetKey(string buttonName)
    {
        return Input.GetKey(buttonName);
    }

    /// simply called the regular input
    /// @param buttonName see Input documentation
    /// @return see Input documentation
    public bool GetKeyDown(string buttonName)
    {
        return Input.GetKeyDown(buttonName);
    }

    /// simply called the regular input
    /// @param buttonName see Input documentation
    /// @return see Input documentation
    public bool GetKeyUp(string buttonName)
    {
        return Input.GetKeyUp(buttonName);
    }

    /// simply called the regular input
    /// @return see Input documentation
    public string[] GetJoystickNames()
    {
        return Input.GetJoystickNames();
    }

    /// simply called the regular input
    /// @param button see Input documentation
    /// @return see Input documentation
    public bool GetMouseButton(int button)
    {
        return Input.GetMouseButton(button);
    }


    /// simply called the regular input
    /// @param button see Input documentation
    /// @return see Input documentation
    public bool GetMouseButtonUp(int button)
    {
        return Input.GetMouseButtonUp(button);
    }

    /// simply called the regular input
    /// @param button see Input documentation
    /// @return see Input documentation
    public bool GetMouseButtonDown(int button)
    {
        return Input.GetMouseButtonDown(button);
    }

    /// simply called the regular input but also reinitialize the input (including releasing everything and reaquiring
    /// them!
    public void ResetInputAxes()
    {
        Release();
        Start();
        Input.ResetInputAxes();
    }

    /// simply called the regular input
    /// @param index see Input reference
    /// @return see Input reference
    public AccelerationEvent GetAccelerationEvent(int index)
    {
        return Input.GetAccelerationEvent(index);
    }

    /// simply called the regular input
    /// @param index see Input reference
    /// @return see Input reference
    public Touch GetTouch(int index)
    {
        return Input.GetTouch(index);
    }

    /// simply called the regular input
    public bool isGyroAvailable
    {
        get { return Input.isGyroAvailable;  }
    }

    /// simply called the regular input
    public Gyroscope gyro
    {
        get { return Input.gyro; }
    }

    /// simply called the regular input
    public Vector3 mousePosition
    {
        get { return Input.mousePosition; }
    }

    /// simply called the regular input
    public bool anyKey
    {
        get { return Input.anyKey; }
    }

    /// simply called the regular input
    public bool anyKeyDown
    {
        get { return Input.anyKeyDown; }
    }

    /// simply called the regular input
    public string inputString
    {
        get { return Input.inputString; }
    }

    /// simply called the regular input
    public Vector3 acceleration
    {
        get { return Input.acceleration; }
    }

    /// simply called the regular input
    public AccelerationEvent[] accelerationEvents
    {
        get { return Input.accelerationEvents; }
    }

    /// simply called the regular input
    public int accelerationEventCount
    {
        get { return Input.accelerationEventCount; }
    }

    /// simply called the regular input
    public Touch[] touches
    {
        get { return Input.touches; }
    }

    /// simply called the regular input
    public int touchCount
    {
        get { return Input.touchCount; }
    }

    /// simply called the regular input
    public bool eatKeyPressOnTextFieldFocus
    {
        get { return Input.eatKeyPressOnTextFieldFocus; }
    }

    /// simply called the regular input
    public bool multiTouchEnabled
    {
        get { return Input.multiTouchEnabled; }
    }

    /// simply called the regular input
    public DeviceOrientation deviceOrientation
    {
        get { return Input.deviceOrientation; }
    }

    /// utility function, chooses the correct axis from a vector
    /// @param vec the vector we got
    /// @param axis the axis we want from the vector
    /// @return the value of the relevant vector
    protected float GetAxisFromPos(Vector3 vec, NIAxis.AxesList axis)
    {
        switch (axis)
        {
            case NIAxis.AxesList.xAxis:
                return vec.x;
            case NIAxis.AxesList.yAxis:
                return vec.y;
            case NIAxis.AxesList.zAxis:
                return vec.z;
        }
        throw (new System.Exception("unimplemented axis type "+axis));        
    }

    /// @brief This method calculates a normalized value for a specific axis
    /// 
    /// This method takes a value (a specific axis of a position) and tries to calculated a normalized
    /// value from it.
    /// If range
    /// @param valueToCalc The value (a specific axis of a position) which we want to normalize
    /// @param range the range of normalization. If this is positive (non zero) we clamp to [-range,range] and normalize to [-1,1]. We do not change if range<=0.
    /// @param invert if this is true we multiply the result by -1
    /// @param sensitivity A multiplier of the values
    /// @param deadZone if the absolute value (after everything) is smaller than this value, we use 0.
    /// @return the normalized value
    protected float NormalizeAxisPos(float valueToCalc, float range, bool invert, float sensitivity, float deadZone)
    {
        if (range > 0)
        {
            valueToCalc = Mathf.Clamp(valueToCalc, -range, range);
            valueToCalc /= range;
        }
        valueToCalc *= sensitivity;
        if (invert)
            valueToCalc *= -1.0f;
        if (Mathf.Abs(valueToCalc) < deadZone)
            valueToCalc = 0;
        return valueToCalc;
    }

    /// This method takes one axis and calculates its value
    /// @param axis the axis we calculate on
    /// @param raw true if raw input is used, false if smooth input is used
    /// @return the axis value
    protected float CalcNumberForAxis(NIAxis axis,bool raw)
    {
        switch (axis.m_Type)
        {
            case NIAxis.NIInputTypes.Gesture:
                {
                    float val = axis.m_sourceGesture.GestureInProgress();
                    if (axis.m_invert)
                        val = -val;
                    return val * axis.m_sensitivity;
                }
            case NIAxis.NIInputTypes.HandMovement:
                {
                    Vector3 pos = raw ? axis.m_sourceTracker.CurPosRaw : axis.m_sourceTracker.CurPos;
                    // now we need the relevant axis
                    float axisValue = GetAxisFromPos(pos, axis.m_axisUsed);
                    return NormalizeAxisPos(axisValue, axis.m_maxMovement, axis.m_invert, axis.m_sensitivity, axis.m_deadZone);
                }
            case NIAxis.NIInputTypes.HandMovementFromStartingPoint:
                {
                    Vector3 pos = raw ? axis.m_sourceTracker.CurPosFromStartRaw : axis.m_sourceTracker.CurPosFromStart;
                    // now we need the relevant axis
                    float axisValue = GetAxisFromPos(pos, axis.m_axisUsed);
                    return NormalizeAxisPos(axisValue, axis.m_maxMovement, axis.m_invert, axis.m_sensitivity, axis.m_deadZone);
                }
            case NIAxis.NIInputTypes.DeltaHandMovement:
                {
                    Vector3 pos = raw ? axis.m_sourceTracker.CurDeltaPosRaw : axis.m_sourceTracker.CurDeltaPos;
                    // now we need the relevant axis
                    float axisValue = GetAxisFromPos(pos, axis.m_axisUsed);
                    // we use -1 for the range because we never normalize the delta values
                    return NormalizeAxisPos(axisValue, -1, axis.m_invert, axis.m_sensitivity, axis.m_deadZone);
                }
            default:
                {
                    throw (new System.Exception("" + axis.m_Type + " not implemented yet"));
                }
        }
        
    }

 

    /// the GetAxis and GetAxisRaw are simply translated to this function.
    /// it goes over all the axes (including the result from Input) and returns
    /// the value with the highest absolute value.
    /// @param name the name of the axis
    /// @param raw true if raw input is used, false if smooth input is used
    /// @return the axis value
    protected float GetAxisInternal(string name, bool raw)
    {
        float absRes = 0;
        float bestValue = 0;
        bool found=false; // true if we found the axis at all
        bool checkInput = false; // if true then we need to check the original input for the axis
        foreach (NIAxis axis in m_axisList)
        {
            if(axis.m_axisName.CompareTo(name)!=0)
                continue; // an irrelevant axis
            found=true;
            if(axis.m_NIInputAxisOnly==false)
                checkInput=true;
            if(axis.m_sourceTracker==null)
                continue; // we failed to get any tracker to get the input from.
            float tmpValue = CalcNumberForAxis(axis,raw);
            float absTmp = Mathf.Abs(tmpValue);
            if (absTmp > absRes)
            {
                bestValue = tmpValue;
                absRes = absTmp;
            }
        }
        if (found && checkInput == false)
            return bestValue; // we shouldn't check the original value
        
        // we need to check vs. the original input.
        float inputValue;
        if (raw)
            inputValue = Input.GetAxisRaw(name);
        else
            inputValue = Input.GetAxis(name);
        float absInput = Mathf.Abs(inputValue);
        if (absInput > absRes)
            return inputValue;
        return bestValue;
    }




    //elements used for the inspector only
    /// This saves whether the axes list is folded or not (used for the inspector only)
    public bool m_foldAllAxes;
    /// This saves whether each axis is folded or not (used for the inspector only)
    public List<bool> m_foldAxisElement;

}
