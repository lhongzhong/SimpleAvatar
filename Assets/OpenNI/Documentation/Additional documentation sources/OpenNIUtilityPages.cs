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
// this file is used for documentation rather than code.
// it is aimed at the utilities

/**
@page NIGUIPrefab The NIGUI Prefab

The %NIGUI prefab is aimed at providing easy access to the @ref NIGUI class. Since %NIGUI 
itself is a static class, it does not need a prefab, however %NIGUI requires a cursor. The
prefab provides a cursor object, using @ref DefaultNIGUICursor, and automatically
sets it as the cursor using the @ref NICursorInitializer script.

@see @ref NIGUIConcepts
*/

/**
@page NIGUIConcepts NIGUI Concepts
The goal of @ref NIGUI is to provide an easy to use GUI creator that uses Natural Interactions.<br>

NIGUI is not provided as a basic object in the package (i.e. part of the @ref OpenNIAssets module)
but rather part of the Utilities module (@ref OpenNIUtilities) as it is more of an example of how to
implement a GUI system than a fully fleshed solution. That said, %NIGUI is built on the notion of 
providing an almost transparent wrapper to Unity's own GUI. The basic idea is to remain true to
<A HREF="http://unity3d.com/support/documentation/Components/GUI%20Scripting%20Guide.html"> 
Unity's GUI scripting guide </A>, replacing the regular 
<A HREF="http://unity3d.com/support/documentation/ScriptReference/GUI.html"> Unity's GUI class </A>
with the @ref NIGUI class.<br>
%NIGUI does not implement all elements of GUI. This is for two reasons: <br>
- The first is that some elements are simply not required, for example, a label will work exactly the same as there is no
interaction with it. Therefore, there is no reason for 
NIGUI to rewrite it.<br><br>
- The second is that some elements are more problematic to implement and 
therefore are left for future upgrade or for user implementation. In this case, the various 
GUI windows options were not implemented. <br><br>

As with the regular GUI class, %NIGUI is a static class, but unlike the regular GUI class it requires
a cursor. The reason for this is that while the regular input (mouse, keyboard) controls the regular
operating system level cursor, Natural Interactions elements are written inside Unity. This means that
we need to use a cursor that follows the hand and performs the clicking. The cursor must extend
@ref NIGUICursor and it must be added to the NIGUI class using the SetCursor method.
@note Currently NIGUI supports only one cursor at a time.

A default cursor is provided, @ref DefaultNIGUICursor, which uses the input for control. This cursor
assumes you have created the following axes in NIInput: 
- NIGUI_X to control the 'x' axis. 
- NIGUI_Y to control the 'y' axis 
- NIGUI_CLICK to click. 

If you want to control NIGUI both from the Natural Interactions input and from regular input, 
such as mouse, the appropriate axes must be defined in InputManager.<br>

@subpage NIGUIPrefab is provided for easy access to the NIGUI class.
*/

/**
@defgroup OpenNIUtilities Utility Assets
@brief The utilities are tools that are supplied for the developer. They are meant
to be used as debug tools or, in the case of %NIGUI, an implementation reference.<br>

The tools are divided into three groups:
- Viewers: These are the Radar, Image, User and Depth Viewers that are used to
  display what the sensor sees.
- @ref NISkeletonCalibrationMessageUtility is used to remind the developer to enter 
  calibration pose
- %NIGUI is a package for GUI development.

@ingroup OpenNIPackage
*/

/**
@defgroup OpenNIGeneralUtilities General Utilities
@brief General utilities

@ingroup OpenNIUtilities
*/


/**
@defgroup OpenNIDebugUtilities Debug Utilities
@brief Utilities used in building the project and debugging.

The utilities in this group are aimed at debugging and building the scene and are @b NOT aimed at
the actual game. In addition to the scripts, the CalibrationMessage prefab is used which uses the 
@ref NISkeletonCalibrationMessageUtility to tell us which skeletons require calibration.
@ingroup OpenNIUtilities
*/

/**
@defgroup OpenNIViewerUtilities Viewer Debug Utilities
@brief Viewer utilities.

The utilities in this group are viewers aimed at showing debug data from the sensor such as depth.
@ingroup OpenNIDebugUtilities
*/

/**
@defgroup OpenNIGUIUtiliites NIGUI Utilities
@brief NIGUI is aimed at extending the GUI class to be NI aware.

See @ref NIGUIConcepts
@ingroup OpenNIUtilities
*/
