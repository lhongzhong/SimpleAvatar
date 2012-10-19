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
// it is aimed at the samples

/**
@page OpenNISamplesOverview Samples Overview
To better understand how to use this package, samples are provided.<br> The samples are short, and include  
basic usage of the package. Each sample is a tutorial that demonstrate how to achieve a certain behavior. 
A finished version of the tutorial is supplied as a Unity Scene for reference purposes.<br><br>
The samples are designed in a specific order, which means that you should finish the first sample tutorial
before moving onto the second one. This is to prevent the later samples being overloaded with repeated 
explanations of the same thing. <br>

@see @subpage OpenNISamplesList
*/


/**
@page OpenNISamplesList OpenNI Samples List

<H1>OpenNI-based Samples</H1>
Following is a list of tutorials that create samples that can be used for teaching the use of the
@ref OpenNIPackage.

@note It is assumed that before starting working on the tutorials, all relevant installations have
been completed (see @ref OpenNIBeforeYouStart). Furthermore, each tutorial assumes a new scene was created
for it.

<H2>Single skeleton using a model</H2>
This sample shows how to prepare a simple 3D model and connect a skeleton to it. The user will be able to 
move around and the model will mimic the player's movement. <br>
For more information and the tutorial, see @subpage OpenNISingleSkeletonTutorial.

<H2>Multiple skeletons using game objects</H2>
This sample expands the previous one by having two “models” controlled by two players. In addition,
it introduces skeletons that are built based on game objects, rather than fully rigged models (very useful 
for prototyping and debugging).<br>
For more information and the tutorial, see @subpage OpenNIMultipleSkeletonsTutorial.

<H2>Control cursor with Input (skeleton-based)</H2>
This sample shows how to define Axes using the NIInput class and control a simple cursor with 
it.<br>
For more information and the tutorial, see @subpage OpenNIInputControlTutorial.

<H2> %NIGUI (skeleton-based)</H2>
This sample shows the creation of a simple GUI using the NIGUI capability and controlled by the
user’s hand.
For more information and the tutorial, see @subpage OpenNINIGUITutorial.

<H2> Simple game</H2>
This sample shows a very simple game where balls fall from the sky and the user uses the skeleton to punch
and kick them around.
For more information and the tutorial, see @subpage OpenNISimpleGameTutorial.

*/

/**
@defgroup OpenNISamples Sample Assets
@brief Included with this product are several samples aimed at helping the developer learn how 
to use the provided tools.<br>

For an overview on how to use the samples, see @ref OpenNISamplesOverview and for a list of the samples, see 
@ref OpenNISamplesList<br><br>

The samples have more than just scripts. They also use various general assets, prefabs and scenes.

<H1>Prefabs</H1>
First and foremost the samples provide skeleton prefabs. These are used to easily and quickly add
skeletons to the scene. see @ref OpenNISkeletonSamples for a list of prefabs.<br>

In addition to the skeleton prefabs, the BallPrefab is supplied to be used in the @ref OpenNISimpleGameTutorial
 
<H1>Scenes</H1>
For each of the various samples, a scene is supplied with the finished result.
 
<H1>General Assets</H1>
For the samples to look good, various models, textures and materials are needed. They are located here.

@ingroup OpenNIPackage
*/

/**
@defgroup OpenNISkeletonSamples Skeleton Sample Assets.
@brief Assets used for samples on the skeletons.<br>

These are assets used for samples involving the skeletons. They include the graphics (for the skeleton
model) and the skeleton prefabs. These are used to easily and quickly add skeletons to the scene.<br>
The following skeleton prefabs are used:
- @b ConstructionWorkerPrefab: This prefab contains a model of the construction worker from Unity's 
  standard assets for which we connected a controller. All relevant joints have already been
  connected. The construction worker moves its center to move around but uses the joint's orientation
  to move its limbs.
- @b MoveableSoldier: This prefab contains a model of a soldier (taken from one of Unity's 
  example projects) for which we connected a controller. All relevant joints have already been
  connected. The soldier moves its center to move around but uses the joint's orientation to move
  its limbs.
- @b FullBodySkeletonCubes: This prefab creates a hierarchy of game objects to represent the 
  various joints. It then includes a skeleton controller to which these joints are attached. 
  Unlike the MoveableSoldier prefab and ConstructionWorkerPrefab, here every joint is moved individually 
  as there is no rigged model to control.
- @b UpperBodySkeletonCubes: This prefab works in a similar way to FullBodySkeletonCubes with one main
  difference; it does not have legs. This can be used when you want to model only the top of the
  user, for example, so that the user can sit down.
- @b SkeletonLineDebugger: This prefab should be attached to a @ref NISkeletonController object. It will 
  draw lines between the joints. @note this creates gizmos these lines are visible in the editor only
  in the scene view (or if the Gizmos option is checked in the game options in the game view as well).
- @b SkeletonLineRenderer: This prefab should be attached to a @ref NISkeletonController object. It will 
  draw lines between the joints. @note this uses LineRenderer which means it should work in the 
  standalone build unlike SkeletonLineDebugger.

@ingroup OpenNISamples
*/

/**
@defgroup OpenNITrackerSamples Tracker Sample Assets.
@brief Sample trackers.<br>

To better understand how to use the trackers concept, and to show the usage in a game, several
gesture tracker examples are provided. 
@note These are very simple implementation aimed at showing how to extend the gestures and @b NOT as a production
ready gesture.
@ingroup OpenNISamples
*/

/**
@defgroup OpenNISpecificLogicSamples Sample Assets Required for Specific Examples.
@brief Sample assets required for specific examples.<br>

This module includes various samples used to implement specific logic in the samples. It also include the BallPrefab 
which is supplied to be used in the @ref OpenNISimpleGameTutorial
@ingroup OpenNISamples
*/
