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
// it is aimed at openNI


/** 
@page OpenNILicense OpenNI Unity Toolkit License
<H1>License</H1>
OpenNI Unity Toolkit - Copyright &copy; 2011 PrimeSense Ltd.<br><br>

The OpenNI Unity toolkit is free software: you can redistribute it and/or modify  
it under the terms of the GNU Lesser General Public License as published 
by the Free Software Foundation, either version 3 of the License, or  
(at your option) any later version.<br>
                                                                       
The OpenNI Unity toolkit is distributed in the hope that it will be useful,             
but WITHOUT ANY WARRANTY; without even the implied warranty of        
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the          
GNU Lesser General Public License for more details.<br>
                                                                        
You should have received a copy of the GNU Lesser General Public License 
along with OpenNI. If not, see <http://www.gnu.org/licenses/>.          
*/

/**
@page OpenNIFramework OpenNI Framework

The OpenNI organization is an industry-led, not-for-profit organization formed to certify and promote the 
compatibility and interoperability of Natural Interaction&reg; (NI) devices, applications and middleware.<br><br>

As a first step towards this goal, the organization has made available an open source framework – the OpenNI
framework – that provides an application programming interface (API) for writing applications utilizing natural
interaction. This API covers communication with both low-level devices (e.g. vision and audio sensors), as well 
as high-level middleware solutions (e.g. for visual tracking using computer vision).<br>

The API framework itself is also sometimes referred to as OpenNI. <br><br>
For more detailed information and much more, see <a href="http://www.openni.org/">http://www.openni.org/</a> 
*/


/** 
@page OpenNISettingsPrefab OpenNISettings Prefab

The OpenNISettings prefab is aimed at providing basic configuration and settings for OpenNI 
capabilities and one place for all initialization. It contains three game objects:

<H1>Logger</H1>
The logger is used to filter out messages created by the various OpenNI elements. It filters based
on which categories and sources to show. Nominally, all options are marked, but if one or more category or 
source is not interesting, you can simply uncheck them. The messages will show up in the console (using 
Unity's Debug.Log method).<br><br>
The Logger is based on the @ref NIEventLogger script
@note In order to lower the number of messages, the verboseness can also be controlled.

@see In order to get behind the scenes OpenNI errors, one can use the SampleXML.xml to activate the
OpenNI logging to a file. This is described further in the internal documentation of SampleXML.xml.

<H1>Query</H1>
OpenNI supports many implementations and a user might have various ones installed. In order to ensure
that everything required by the game is supported, a query can be used. To build a new query,
the developer must inherit the NIQuery class, and in the construction, build a new openNI query that 
limits the implementations that can be used. For most purposes however, this is not needed as the 
basic NIQuery object is enough.<br><br>
The NIQuery script contains a list of node types with a specific query for each. The basic 
implementation contains a query for Depth nodes, Image nodes, User nodes, hands nodes and gesture 
nodes. See @ref NIQuery for more information.<br>
In These queries, the basic node description can be queried including the node name, vendor and 
minimum and maximum allowed versions. If the nodes were loaded (for example if the game is run) then
the information for the specific loaded versions can be seen below the description to provide hints
as to how to configure the queries. 
@note If for some reason the queries here are not enough, it is possible to load from an xml file.
A sample xml file is supplied (sampleXML.xml), see its internal documentation for more information.

<H1>OpenNI Configuration</H1>

<H2> Overview</H2>
The OpenNI configuration child object is the basic object used to initialize and configure OpenNI. 
It is designed to be used as a singleton and the internal structures it uses
are actual singletons (see @subpage SingletonImplementationOverview for more information). <br>
The object uses the @ref OpenNISettingsManager script to provide the functionality and uses a custom 
inspector (@ref NIOpenNISettingsManagerInspector) to provide easy access to all relevant data.
The  @ref OpenNISettingsManager class also provides easy programmatic tools to query and configure
the internal OpenNI objects. 

<H2>Custom Inspector</H2>

The custom inspector has two modes:
- Edit mode: This mode is active during editing. It enables changing the overall behavior of the base
  objects
- Run mode: This mode is active while the program is running. It limits the changes that can be made but 
  shows the state of various elements.

While in Edit mode, it provides the following customization options:
- Mirror behavior – sets if the image and depth data is mirrored or not (default is true).
- XML file – if specific nodes should be loaded
  - This is relevant mainly for OpenNI experts who want to load specifically created nodes. For most purposes, leave this empty.
- Use image generator? – If the game requires access to the RGB camera data, this should be checked.
  Otherwise, this should be unchecked to save CPU.
- Use user generator? – If the game uses the skeleton or multiple users (which is the case for most games), 
  this should be checked. Otherwise, this should be unchecked to save CPU.
- Smoothing factor – a number between 0 and 1 that sets the smoothness of skeleton movement. 
  - A value of 0 means no smoothing is performed
  - A value of 1 means smoothing is so strong that almost no movement occurs
  - A good value is 0.5 for most games.
  - For tracking the hands, for example, for GUI, a value of 0.9-0.95 is useful. Note however,
    that under these factors, the skeleton will be a little slow to respond.
- Logger and query (the objects defined above, which are by default connected in the prefab).
- Playback filename can be used to play a recording as an input instead of using the sensor. The 
  string should contain the filename (full path). An empty string means using the sensor.
- Reset Floor normal button is responsible for resetting the floor normal to "up". For more information see @ref OpenNIAdditionalTopics.
<br><br>
 
When running, the inspector shows only values that can be changed while running. In addition, it will 
show all current users for the users and skeleton view, including extra information about them, such
as userID, uniqueID, calibration state and center of mass.

*/

/** 
@page UserSelectionCapabilities User Selection
Before talking about user selection, some definitions are in order:
- A @b user is an object in the scene which is deemed by the internal algorithms as something worth
  following. This is usually a person but could also be an inanimate object (such as a chair). 
  The OpenNI UserGenerator node is responsible for managing these users.
- A @b player is someone who controls the input. This is a person who is currently playing the game.
  multiple persons can be players for a multi-player game.

The goal of the user selection process is to decide how to map users to players or in other words, 
how to decide which person in the scene is controlling which player in the game. <br>
In order to abstract the user selection process and to provide an interface for managing and accessing
players, @ref NIPlayerManager is used. This base object provide myriad methods to manage and access 
player, the most important of which is @ref NIPlayerManager::GetPlayer "GetPlayer".<br>
This method returns a @ref NISelectedPlayer object which is used to follow and manipulate the 
player.<br>
NIPlayerManager by itself doesn't really do anything, it should be inherited and extended by specific 
user selectors. A user selector is an object, extending NIPlayerManager which decides @b HOW to map
the users to players.<br>
Two implementations are provided as part of the package:
- @ref NIPlayerManagerCOMSelection which chooses the players based on who is closest
- @ref NIPlayerManagerPoseSelection which chooses the players based on a pose they strike.

In older NITE implementations, in order to calibrate the user (a necessary step for tracking), the 
user would have to stand in a "psi" pose. This was in essence a certain type of user selection: 
Select the user using the "psi" pose. This is basically using the second selector 
(@ref NIPlayerManagerPoseSelection) with the "psi" pose.<br>

If a specific game needs a different kind of user selection then all one has to do is inherit 
@ref NIPlayerManager and implement the relevant logic similar to how @ref NIPlayerManagerCOMSelection
and @ref NIPlayerManagerPoseSelection do.<br>
*/

/**
@page SingletonImplementationOverview Internal Singleton Implementation
The internal implementation of initialization and configuration uses the singleton pattern. This is 
because we aim to consolidate the use of OpenNI to a single entry point whenever possible.<br>
For this purpose the following classes are used:
- @ref NIContext, which handles the basic context everything relates to
- @ref NIImage, which handles the RGB image (if needed)
- @ref NIUserAndSkeleton, which handles the identification of users and basic skeleton capabilities

These are abstracted by the use of @ref OpenNISettingsManager.
@note Since these are singletons, all initialization and release done by everyone affects the same 
class. Additionally, when loading a level, a specific release/reinitialize is required if you want to
start from scratch. This is done automatically by @ref OpenNISettingsManager. If you would rather
keep the information, such as the calibrated skeletons, user to player mapping etc., then all 
relevant objects, such as @ref OpenNISettingsManager, must be created on the first level and 
DontDestroyOnLoad or similar solutions used.
*/



/**
@page InputControllerPrefab InputController Prefab
The InputController prefab is simply an Input with a GestureManager and PointTrackerManager already
attached. 
@see @ref NIInputConcept
@see @ref OpenNIPointTrackerConcept
@see @ref OpenNIGestureConcept
@note The prefab does not include any point trackers or gestures attached to it. That still has to
be added to the scene by the developer.
 */

/**
@page NIInputConcept NIInput Concepts

<H1>Overview</H1>
Unity supports an Input Class and an InputManager to configure it. The Input Class and InputManager 
do not support Natural Interactions, their input is limited to more traditional input facilities, such as 
keyboard, mouse and joysticks. The purpose of the NIInput class is to extend these capabilities
to Natural Interactions input. <br>
The implementation of this is the creation of a new NIInput class, which enables us to use
an object of that class instead of the regular input. 
<br><br>
The basic use of %NIInput is simple: 
- Add an NIInput game object to the scene
- Configure it in the custom inspector
- Drag and drop it to the various objects that plan to use it.
- Inside the scripts that use it, use it as if it was the regular Input class.<br>

@note Unlike the Input class, %NIInput is NOT a static variable and its configuration is done using
a custom inspector (rather than a global option as InputManager). This both simplifies the implementation and 
provides a little more flexibility. Nominally, exactly one %NIInput game object should be added to the 
scene but if you want to control several input schemes, you can add several %NIInput objects to the scene
and change the reference to %NIInput inherent in objects that use it.


<H2>Configuring an Input</H2>
The custom inspector of the %NIInput class is designed to be very similar to the  
<A HREF="http://unity3d.com/support/documentation/Components/class-InputManager.html"> 
InputManager </A> provided by Unity. Anyone who wants to define an axis can do one of the
following:
- Go to the regular InputManager and define an axis; this will affect regular input only
- Go to the custom inspector and define an axis; this will affect Natural Interactions input only
- Redefine the same axis in both to allow both regular and Natural Interactions input.<br>

The configuration of %NIInput's custom inspector looks very similar to the InputManager 
(see @ref NIInputInspector for more details). The important thing to remember is that when using
GetAxis and GetAxisRaw, the value received will be the one with the highest absolute value between
all implementations of the same axis name (similar to the definition in InputManager but extended to
include %NIInput axes as well).


<H2>Using the Input</H2>

<H3>Extending Unity's Input Class</H3>
Using %NIInput is very similar to using the regular <A HREF="http://unity3d.com/support/documentation/Manual/Input.html"> 
Input</A> provided by Unity and actually extends its <A HREF="http://unity3d.com/support/documentation/ScriptReference/Input.html"> 
functionality</A>. To use it simply call the GetAxis(axisName) method and use the results exactly as
is used on traditional input sources. Furthermore, %NIInput encompasses all the functionality of the 
Input class. What this means is that calling Input.GetAxis(axisName) is already done inside the 
GetAxis implementation. In fact, NIInput contains just about any Input methods and properties you 
need, including ones that have no relation to Natural Interactions (such as GetKeyUp method). 
ensures that, you can replace Input with the NIInput object wherever Input is regularly
used.
<br>

<H3>Special %NIInput functionality</H3>
The regular Input class has some utility methods to get direct information on specific input types,
such as GetKeyUp to know when a keyboard key was released. Similar utilities are provided for natural
interface (and specifically for gestures).<br>
When using GetAxis, the value for the gesture will be non-zero, from the time the gesture was detected
and up to a frame after it was finished. This can be divided over several frames. In fact, gestures 
are considered detected for 2 frames; the current and the last one (to avoid missing a frame!). This
is because the gesture can occur at any time during the frame. It is the responsibility of the 
caller to decide what happens if GetAxis is used.<br>
For developers who want to make sure a gesture is detected only once, two options exist:
- Use the @ref NIInput::HasFiredSinceTime method. This method receives a time and tells us if the gesture has
  occurred since that time. The regular use of this is to save the time (using Time.time) when we last
  checked for the gesture and provide that as an input. This is useful mainly for situations where it
  is important for the developer that the detection occurs at a specific time, such as inside an 
  update call.
- Use events (by using the @ref NIInput::RegisterCallbackForGesture and
  @ref NIInput.UnRegisterCallbackForGesture methods). These methods allow the user to register for 
  an event when the gesture is detected. This is mainly relevant when we want to react immediately to
  the event.

@see @ref NIInput for a complete list of its methods.

<H2>Point Trackers and Gestures</H2>
It is important to note that you can get the data on Natural Interactions for a myriad of sources. This
is very similar to being able to use multiple joysticks and multiple buttons per joystick. To simulate
this, we use point trackers (to simulate joysticks) and gestures (to simulate button clicks). 
While the point trackers themselves (@subpage OpenNIPointTrackerConcept) and the gestures themselves 
(@subpage OpenNIGestureConcept) can be used beyond input, for the purpose of the input we can 
consider them as simply that; a list of "joystick" sources, which can be different hands of different
players or even other body joints, and a list of various buttons, defined by the various gestures.

<H2>InputController prefab</H2>
For easy usage of the NIInput class, you can use the @subpage InputControllerPrefab
*/

/**
@page OpenNIPointTrackerConcept The Point Tracker Concept
Natural Interaction enables using body motion to control input. In an analogy for a mouse, this means
tracking some point for position and using a gesture to "click". Unlike a mouse, complex gestures 
can be used to create a large number of possible "clicks" and even provide them with a range of 
values. Also unlike a mouse, there are many moving elements in the body and therefore the input 
source for the gesture (as well the source of the position) can come vary. For example the source
could be any of the skeleton's joints<br>

Whatever the source is, we need a basic behavior of tracking the position (similar to getting the 
mouse's position). For this purpose, the @ref NIPointTracker was defined. This class is an 
abstraction of how we use any and all points tracking and is a base class for various point trackers.
The point trackers that inherit it can (and do) implement various behaviors to track specific points,
such as @ref NISkeletonTracker, which we can use to track specific skeleton joints for a specific 
players. <br>
The tracker provides the current position (smoothed and raw), a delta position and a delta position
compared to a starting position.<br>
The starting position is important because the position by itself is usually meaningless, the tracked
joint can have any real world coordinates (depending on when the player stands). Usually however, the
interesting information is based in a new coordinate system which is around a new center point (for
example when tracking the hand, the movement of the hand would be around a point which is where the
hand would be if the upper arm is tight with the torso and the lower arm is perpendicular toward the
sensor). For this purpose, every point tracker can define a starting position and provide positions
relative to it. The exact options for defining the starting pos is up to the specific implementation.<br>

A game might have multiple point trackers (e.g. one following the right hand and one following the
left hand) and many elements might be interested in those point trackers. In order to provide
a single place to manage them (and initialize them), @ref NIPointTrackerManager is used. 
The developer simply drags and drops the various point trackers to the manager and when any 
game object needs to use it, the manager knows when to initialize and, through reference counting,
when to free them.

@note It is possible, and very useful, to add the same type of tracker more than once to the manager.
A good example of this would be to track several players using the @ref NISkeletonTracker. You can
simply add several objects with the %NISkeletonTracker attached to them, each configured for a 
different joint of a different player.

The Package currently contains just one implementation of the tracker (@ref NISkeletonTracker), 
however, it is very easy to add new ones simply by extending the @ref NIPointTracker class. This
is aimed at allowing third parties to create extension packages with new tracker implementations (and
new kinds of gestures).
*/

/**
@page OpenNIGestureConcept The Concepts of Gesture and Gesture Factory
Gestures are the "key press" and "mouse button click" of Natural Interactions. They provide
us with events that can be used to take actions. A gesture can be something that occurs once, e.g. pushing with the
hand, which ends when the push is detected, continue over time (e.g. leaving the hand steady for
timed clicks) or even provide a new axis, e.g. making a walking gesture and getting a speed measurement 
from it.<br><br>

A gesture can work on various sources such as a skeleton joint. Because of this, a gesture is always
attached to some point tracker (see @ref OpenNIPointTrackerConcept). This could be
a general tracker tracking a point, or, more likely, a specific type of tracker, such as tracking
the skeleton.<br>
 
A gesture implements the @ref NIGestureTracker class, which supplies the interfaces to see
when a gesture was detected. This can be done by registering to events, by checking if the gesture
occurred since a given time, getting the frame when a gesture last occurred, or tracking a gesture 
currently in progress. <br>
For each specific gesture, an implementation of the %NIGestureTracker class is needed.<br>
Some gestures simply detect when something happened, while other gestures also have a timed element
in them. For example the steady gesture, implemented for a skeleton tracker by 
@ref NISteadySkeletonHandDetector, detects when the tracked hand is steady (doesn't move). 
It also counts the time where the hand doesn't move and therefore can provide the detected event only
after a specific time has passed, all the while showing us the progress.
<br><br>
Since the same gesture detection can be done on several hands, we use a gesture factory to create a 
new gesture and connect it to a relevant tracker whenever needed. The gesture factory is responsible
for the initialization. 
@note The initialization will fail if the wrong type of gesture is used with the wrong type of tracker.
For example, if you use the NITE gesture package, which includes a NITE based gesture (requires
NITE hand tracker) then the initialization will fail if you supply it with a skeleton based
OpenNI tracker.

As with point trackers, a Gesture manager (@ref NIGestureManager) is used to allow the developer to drag
and drop the gestures to the manager and gives us the legal gestures to use.
 */


/**
@defgroup OpenNIPackage OpenNI package
@brief This bundles all the functionality of OpenNI in a single module as part of the OpenNI Unity Toolkit.

The OpenNI package includes everything a user needs to create amazing games using the 
@ref OpenNIFramework. It includes the assets, samples and utilities.
@see @ref OpenNIPackageOverview
*/


/**
@defgroup OpenNIAssets OpenNI Assets
@brief This is the main module for the @ref OpenNIPackage. It includes all the code needed to create 
amazing games based on the skeleton provided by OpenNI. <br>
All of the code in this module is built upon the interfaces defined in the @ref OpenNIFramework. 
This means that if you install OpenNI with an OpenNI compliant middleware and supporting sensor, everything
in this package will work. Furthermore, all code was written inside Unity and is available here.
<br><br>
The OpenNI module includes more than just scripts, it also includes several prefabs that
can be used for faster implementation. These include:
- @ref OpenNISettingsPrefab
- @ref InputControllerPrefab
@ingroup OpenNIPackage
*/

/**
@defgroup OpenNIBasicObjects OpenNI Basic Objects
@brief The basic objects to wrap the @ref OpenNIFramework

This submodule is responsible for wrapping the basic objects of the @ref OpenNIFramework. It is
aimed at performing all relevant initialization behind the scenes and providing simple customization options.
It also provides easy to use simplified access to the internals using scripts.<br>
For simplicity, the @ref OpenNISettingsPrefab is used to package everything in an easy-to-use prefab.
@ingroup OpenNIAssets
*/

/**
@defgroup OpenNITrackers OpenNI Trackers
@brief Provides the capability to track joints and hands as well as gestures performed by them.

One of the main goals of the @ref OpenNIPackage is to provide an easy way to add new capabilities. To
do this, a tracker system has been introduced. The tracker system is composed of two main sub-modules;
the @ref OpenNIPointTrackers module, which is responsible for tracking a point (e.g. a hand) and the
@ref OpenNIGestureTrackers, which is responsible for understanding gestures and poses.<br>
<br>
The idea is that you can simply define a new tracker and add it, and then everything based on the 
trackers (such as GUI and Input) will simply be able to start using it. By using this system, 
developers can use "tracker packs" that supply new and interesting behavior. These can be added easily by simply 
dragging the new tracker to the scene.<br>

For more information, see @ref OpenNIPointTrackerConcept.
@ingroup OpenNIAssets
*/

/**
@defgroup OpenNIPointTrackers OpenNI Point Trackers
@brief Provides the capability to track joints and hands.

As stated in the @ref OpenNITrackers module, this sub-module is responsible for providing a capability
to track joints, hands or any moving point. Its goal is to become the "movement" element of a new 
"joystick" or position source.<br>

For more information, see @ref OpenNIPointTrackerConcept.
@ingroup OpenNITrackers
*/

/**
@defgroup OpenNIGestureTrackers OpenNI gesture trackers
@brief Provides the capability to track gestures.

As stated in the @ref OpenNITrackers module, this sub-module is responsible for providing a gesture
and pose tracking capability. The goal is to detect poses and gestures created by a point tracker. <br>
For more information, see @ref OpenNIGestureConcept.

@ingroup OpenNITrackers
*/

/**
@defgroup SkeletonBaseObjects OpenNI Skeleton Usage Basic Objects
@brief Provides the Capability to control models with skeleton

This sub-module is responsible for providing @ref NISkeletonController capability to control models
with the skeleton.
@ingroup OpenNIAssets
*/

/**
@defgroup UserSelectionModule Player management and user selection
@brief This sub-module is responsible for providing player management and user selection capabilities.

The logic behind this module is the fact that there could be many users in the scene and we need a
way to figure out which users to track and which users represent which players.<br>

A @b user is an entity which appears in the scene. In most
cases it is a human being (who can potentially play the game) but it could also be an inanimate 
object which the sensor decide is interesting enough to be considered.<br>
Multiple users can be part of the scene. Consider the basic case where several friends sit in the
living room and plan to play. All of them could be users but for a single player game, only one can
be a player.<br>
A @b player is someone who controls the input. In this tutorial, the player would be the user who 
controls the movements of the skeleton, but in general, a game could have many players controlling 
many, different, game elements.<br><br>

In order to manage the mapping of users to players, the @ref NIPlayerManager object is provided as a 
base object for all mappers (known as user selectors). It provides a standardized interface
for managing the players, the most important of those is the 
@ref NIPlayerManager::GetPlayer "GetPlayer" method.<br>
This method returns a @ref NISelectedPlayer object which is used to follow and manipulate the player.
NIPlayerManager itself doesn't really do anything, it should be inherited and extended by specific 
user selectors. A user selector is an object, extending NIPlayerManager which decides @b HOW to map
the users to players.<br>
Two implementations already exist in the package:
- @ref NIPlayerManagerCOMSelection which chooses the players based on who is closest
- @ref NIPlayerManagerPoseSelection which chooses the players based on a pose they strike.

In older NITE implementations for example, in order to calibrate the user (a necessary step for tracking), the 
user would have had to stand in a "psi" pose. This was in essence a certain type of user selection: 
Select the user using the "psi" pose. This is basically using the second selector 
(@ref NIPlayerManagerPoseSelection) with the "psi" pose.<br>

@ingroup OpenNIAssets
*/


/**
@defgroup OpenNIInput NIInput
@brief Extends Unity's Input class.

This sub-module is basically an implementation of the @ref NIInputConcept.
@ingroup OpenNIAssets
*/
