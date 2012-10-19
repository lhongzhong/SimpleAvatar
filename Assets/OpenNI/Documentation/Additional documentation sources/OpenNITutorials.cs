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
// it is aimed at the OpenNI samples tutorials

/**
@page OpenNISingleSkeletonTutorial Single Skeleton Sample

Welcome to the first tutorial. In this tutorial, we will place a model on the scene and allow a user 
to control it. We will also look at some debugging tools that help us to understand the process.

@note The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.

<H1>Step 1: Creating the basic objects</H1>

<H2>Update the scene</H2>
The first thing we need to do is to start adding the basic objects, the first of which is the 
@ref OpenNISettingsPrefab. While this prefab can be built manually, and in some cases even broken into its
individual objects, in almost all cases the developer can drag and drop this prefab to the scene and 
configure it using the inspector.<br>
 
Let's take a look at this object. We will begin by opening it up and looking at its child objects.<br>
- @b Query child object: We use a regular query as there is no requirement for anything special. 
  @note For actual games it is better to use queries to limit to tested implementations.
- @b Logger child object: Nothing needs to be changed in the logger's filter, we want to see all the 
  logging messages for now. <br>After debugging, we can turn off some or all of the options to filter 
  out irrelevant messages.
- <b> OpenNI configuration </b> child object: The inspector for this object provides us with 
  configuration options to make OpenNI behave as needed for the game. For now configure the 
  following:
    - Check the <b> Mirror behavior </b> box (we are looking from behind).
    - Leave the <b>XML file</b> field empty as we don't need any special XML file. 
      @note There is no real need for XML files in most cases. The three main exceptions are:
        - When we want to do manual queries (and the queries provided by @ref NIQuery are not enough).
        - When we want to play a recording for @b either image @b or depth nodes (instead of both, 
          in which case the playback filename options should be used).
        - When we want to change the OpenNI logging options (for example to create a log file which
          provides OpenNI logging).
    - Make sure the <b> Use Image Generation?</b> checkbox is clear, as we do not have an RGB image
      in this sample. @note The current samples do not use the RGB image at all. Currently it 
      is used only by the NIImagemapViewerUtility utility.
    - Check the <b> Use user and skeleton?</b> box as the skeleton is the basis of this tutorial.
      In addition change the <b>Smoothing factor</b> value to 0.5 which is a good choice when 
      controlling a skeleton.
    - The prefab has already taken care of linking the <b> Logger object </b> and <b> query object </b> 
      to the OpenNI configuration object so leave it at its default.
    - Leave the <b>Playback filename</b> field empty as we want to get the data from the live sensor
      rather than some recording.

Additionally, add the @b NIDepthViewer and and @b NIRadarViewer prefabs (located under 
Prefabs/Utility prefabs) to the scene. These will enable us to see the depth 
data (i.e. knowing what the sensor sees) and which users are identified.<br><br><br>

<H2>Play the game</H2>
Press play and look at the results. <br>
Nothing much is seen in the game window (only the depth and radar viewers) but various messages 
appear in the console.
<H3>Look at the initialization process in the console</H3>
The logger dumps the initialization process into the console.<br>
As the scene loads, various initializations are performed. These are sent to the console based on 
the filters of the logger in @ref OpenNISettingsPrefab. <br>
You should see something like:<br><br>

<code>
In NIContext:Init(Logger (NIEventLogger))
In OpenNIContext.InitContext with logger=Logger (NIEventLogger) query=Query (NIQuery)
Creating type=Depth
In NIUserAndSkeleton:Init(Logger (NIEventLogger))
initializing with context
Creating type=User
etc.</code><br><br>

If any error messages appear, specifically exceptions or "context invalid" messages, this usually 
means that OpenNI framework, an OpenNI compliant middleware (e.g. NITE) or the sensor drivers are
either not installed or an old version of them is installed. In this situation it is best to 
reinstall them (see the readme.rtf file and @ref OpenNIBeforeYouStart for more information). 
Also make sure the XML string is empty. If this does not solve the problem, please contact 
support@primesense.com<br>

When ending the game by pressing play again to return to edit mode, messages will appear telling us 
everything is being released.

<H3>Look at what the sensor sees in the Depth Viewer</H3>
The sensor can show depth information. The NIDepthViewer shows the depth image in the upper right
corner of the screen. We can configure the NIDepthViewer in a number of ways
(see @ref NIDepthmapViewerUtility for more information) but the main elements are:
- @b Context: This is basically the OpenNISettingsManager object. Generally speaking, you should drag the
relevant object from the OpenNISettingsPrefab. However, in practice we can leave it as "None", as the 
viewer will find the object by itself (assuming just one exists). @note This is true for many objects!
- <b>Place to draw</b>: Defines the rectangle the depth will be drawn on. The x,y values represent the 
distance from the relevant corner (defined in the @b Snap variable) and the width and height set the 
size of the window.
- @b Factor (which is default to 4): Sets the ratio between the original resolution provided by the sensor
and the one drawn. Specifically, if the sensor provides a resolution of QVGA (320*240) and we use a 
factor of 4, then we will only use a resolution of 80*60 (taking one pixel in 4 in each direction or 
a total of 1/16 of the pixels). This is used for performance as the NIDepthViewer is a debug tool and
not very efficient. @note Do not try to use a factor of 1 unless you have an extremely powerful computer. 
On most computers, a value of 4 is the best to use (2-3 for stronger ones).
- <b>Depth Map color</b>: Sets which base color is used.

<H3>Find the users in the Radar Viewer</H3>
The NIRadarViewer allows us to see users. A user is a moving object in the scene. <br>
Users are considered "players", i.e. someone who controls the input, only after they are selected 
and start tracking. When a user is just detected, they are considered uncalibrated and are in the
uncalibrated color (red by default). When they are selected they become calibrating which prepares
them for tracking (orange by default) and after a short time become calibrated (yellow by default).
They become tracking (green by default) when the skeleton is actually tracked and the user 
controls the input. 
@note If the user was calibrated and then became unselected, they will remain in the calibrated 
color which means that if they become selected then they can be tracked much faster. 
See the user selection below for more information.<br>

Similar to the depth viewer, we can configure the radar viewer in many ways (see 
@ref NIRadarViewerUtility for more information). Its main elements are:
- @b Context: This is basically the OpenNISettingsManager object similar to the depth viewer.
- <b>Place to draw</b>: Again this is similar to the depth viewer (and all viewers) which defines 
the rectangle the viewer will be drawn on. 
- <b>Radar real world dimensions</b>: This limits the area (in real world coordinates) where we 
  look for the users and normalizes the results. 
  See @ref NIRadarViewerUtility::m_radarRealWorldDimensions for more information.
- <b>Uncalibrated color</b>: Sets which color to use for uncalibrated users.
- <b>Calibrating color</b>: Sets which color to use for users in the middle of their calibration 
  process.
- <b>Calibrated color</b>: Sets which color to use for calibrated users which are not tracking 
  (e.g. because they are unselected).
- <b>Tracking color</b>: Sets which color to use for tracked users.

<H1>Step 2: Add the skeleton and user selection</H1>
<H2>Add the skeleton model</H2>
The goal of this tutorial is to control a model so the first step is to add a model to control.
To do this, drag the ConstructionWorkerPrefab from the samples prefabs into the scene.<br>
For a short tutorial on how to create such a prefab (and to better understand how to configure it), 
see @subpage OpenNICreatingASkeletonModelPrefabTutorial.

@note A second model is supplied under the MovableSoldier prefab. This is provided for convenience to 
have another example. The model here was imported from an older asset in a previous version of Unity.

Leave all the configuration of the model as in the prefab (specifically, player 0 is used as this
tutorial supports just one player).

<H2>Add a User selection method</H2>
While the scene now contains a skeleton model, pressing "play" now will leave the model
in a "psi" position (a position where the skeleton stands erect with arms to 
the side and the forearms are pointed up). This is because the skeleton model is attached to 
"player 0" but nothing defines who "player 0" is (i.e. we do not know which player controls it).<br>

Defining the user selection is based on the capabilities provided by the @ref UserSelectionModule 
module. For the purpose of the first tutorial, we will assume a simple user selection scheme: the 
user closest to the sensor is the player.<br><br>

Create a new, empty game object and rename it Player Manager. Drag the NIPlayerManagerCOMSelection 
script to it. 
- Set the number of player to 1 (we only have one skeleton to control).
- leave the number of retries at 0. This represents how many times to try to calibrate after failing. 
  Usually, failure to calibrate means the user is not a human being, but at times, it could just mean
  the user is partially obstructed (outside the field of view) or stands in a position which is hard 
  for the algorithm to calibrate from. If the target is a human, retrying after the user has moved a 
  bit can help the calibration to succeed. For our purposes, 0 is good enough as we can always move
  a bit to retry.
- Leave Max distance as is (100000 is 100 meters). This is the distance we consider someone with no 
  better info to be at.
- Leave the Failure penalty at its default. This is the distance we move backward any user who failed
  to calibrate (unless they move) so we won't get locked on them.
- Leave the Min change to retry at its default of 20 cm. This means that we delete the "failed" tag 
  from a user which moved from than 20cm from the position in which they failed to calibrate. Do not
  confuse this with the number of retries.
- Leave the Hysteresis at 10cm. This means that if someone is selected we consider them to be 10cm 
  closer to avoid going back and forth between two users who are very close to each other.

<H2>Play the game</H2>
Press play and stand in front of the sensor, move around and see how the skeleton mimics your 
movements.

<H1>Step 3: Making it look better</H1>
While the sample is working fine now, it does not look pretty, so now we will improve its appearance:

<H2>Add a light</H2>
The scene right now has no lights, so let's add one. Add a new directional light game object and set its
rotation to 45, 315,45.

<H2>Center things</H2>
At the moment the soldier is small, change the camera's position to 0,0,-3 to center and zoom in.
<br><br>
<H2>Play the game</H2>
Press play and look at the results. <br>
The tutorial now has a skeleton controlled model that is waiting for us to enter the scene and
allows us to control its movements. We can also see which users are identified and their 
calibration state in the Radar Viewer and see the depth result from the sensor in the Depth Viewer.<br><br>

@note For best results make sure you enter the scene and play with no obstructions, i.e. the entire
body should be visible to the sensor!.

<H1>Enjoy your victory, you have completed your first sample!</H1>
Continue to the next sample @ref OpenNIMultipleSkeletonsTutorial or return to @ref OpenNISamplesList.
*/

/**
@page OpenNICreatingASkeletonModelPrefabTutorial Creating A model prefab to use with a skeleton controller tutorial
One of the most important things needed when using the skeleton controller is having a good model it can
control (see @ref AddingNewSkeletonModels "Adding New Skeleton Models" for more information). This 
tutorial aims at creating a prefab to be used (The ConstructionWorkerPrefab provided is the end result)
from the construction worker model provided in the Unity standard assets.
<H1>Step 1: Import the model</H1>
We want to import the character controller standard assets (go to Assets->Import package->character 
controller). While we can simply import everything, some of the assets there are not very useful for us.
We are only interested in the worker model. To import it, press the "none" button at the bottom left to
uncheck all assets and then check the checkbox to the left of the 
Standard Assets/Character Controllers/Sources/PrototypeCharacter asset (this will automatically check
all its children, which include the model itself: constructor.FBX and the required prerequisites such
as textures and materials).<br>
Drag the constructor model to the scene.<br>
<H1>Step 2: Fix the model up</H1>
First of all, we do not need the animations provided with it, so we can just remove this component.<br>
@note this severs the prefab connection.
The model must be standing in a "T" pose (i.e. standing erect with the arms extending 180 degrees to the
sides). The model we use is not 100% that. For this purpose we should fix up its orientation.<br>
As can be seen from the scene view, the hands of the construction workers are slanted down. We can 
fix that simply by changing the orientation of the relevant joints. Open up the child objects and do
the following changes:
- The orientation of Bip001 L Clavicle should be 0,270,180
- The orientation of Bip001 L UpperArm should be 0,0,0
- The orientation of Bip001 R Clavicle should be 0,90,180
- The orientation of Bip001 R UpperArm should be 0,0,0
@note some minor tweaks were made to other joints to round off the numbers...

<H1>Step 3: Connect the skeleton controller</H1>

Add a @ref NISkeletonController script to the model This script is responsible for controlling the
skeleton and look at its inspector (implemented in @ref NISkeletonControllerInspector). <br>

The inspector begins with the <b>Controlling player</b> section which is used to define which player
controls the skeleton. The <b>Player manager</b>  object represents the NIPlayerManager (i.e. the
user selection object) used. If nothing is entered then the first found in the scene is used.
The <b>Player Number</b> represents which player (numbered from 0 onward) to use. By default this is
player 0 (i.e. the first player). @note if multiple skeleton controllers use the same player number 
and the same player manager then they will follow the same user's movement, one cloning the 
other.<br><br>

The next section is the <b>Joints to control</b> section which includes a large number of public 
transforms (Head, Neck, Torso, Hands etc.) These are the transforms representing the joints which can 
be controlled by the script. Drag and drop the relevant game objects from the hierarchy to the 
relevant joint. Only joints which are filled will be controlled<br>
@note In different implementations of OpenNI, different joints are supported. The joints attached here
are the ones supported in the supplied implementation!

<br>For this tutorial, connect the joints to the child objects of the construction worker's game 
object according as follows:
- Head: Bip001 Head
- Torso: Bip001 Pelvis
- Left Shoulder: Bip001 L UpperArm
- Left Elbow: Bip001 L Forearm
- Left Hand: Bip001 L Hand
- Right Shoulder: Bip001 R UpperArm
- Right Elbow: Bip001 R Forearm
- Right Hand: Bip001 R Hand
- Left Hip: Bip001 L Thigh
- Left Knee: Bip001 L Calf
- Left Foot: Bip001 L Foot
- Right Hip: Bip001 R Thigh
- Right Knee: Bip001 R Calf
- Right Foot: Bip001 R Foot

<br>
The next section is the <b>What to update</b> section. This section is responsible for deciding what
is controlled:
- <b>Update Root Positions? </b> states whether the skeleton moves as a whole based on the position 
  of the torso. 
    - Check this box so the skeleton can move around with the player.
- <b>Update Joint Positions?</b> states whether the skeleton should move the attached joints to a new
  position (based on the joint's position in the real world).
    - Make sure this box is unchecked.
- <b>Update Joint Orientation?</b> states whether the joints should change their orientation to match
  the skeleton. 
    - Make sure this box is checked.

The decision when to use the joints positions and orientation depends on the type of model controlled
by the script. A rigged model (such as the one in this tutorial), is prepared so that changing the 
orientation of one joint, changes the position of the joints attached to it. Making the script change
their position will interfere with this and make the model stretch in unnatural ways and therefore 
for rigged models, only the orientation should be controlled and not the position.<br>
A schematic model (e.g. a stick figure or a skeleton made from cubes) is not rigged and therefore
the position of the joints should be updated.<br><br>
The <b>Scaling & limitations</b> section allows scaling between the real world coordinate system and
the skeleton coordinate system as well as limiting the speed of rotation:
- <b>"Rotation Dampening"</b> is used to smooth the movement to avoid jerky changes by limiting the
  rotation speed. It should not be changed from the default unless the the joint's rotation appear
  jerky (and even then, use @ref OpenNISettingsManager's smoothing factor first).
    - Leave this as the default of 15.
- @b Scale is used to define the transformation scale from real world coordinates to unity 
  coordinates. The sensor receives the position data in mm and unity works in meters so the 
  default of 0.001 is generally good. However, if the model in the game is 0.9m high but the average
  user is 1.7m high then 1700:0.9 means ~0.005 would be a good scale.
    - Leave this as the default of 0.001
- <b>Torso speed scale</b> is used to change the speed in which the model moves in the world.
    - Leave this as the default of 1.

The last portion to edit is the <b>Lines Debugger</b> object.<br> 
@par 
The lines debugger object is responsible for drawing lines between the joint's positions. There are 
two line debuggers provided as prefabs in the package: 
- The SkeletonLineDebugger prefab which is based on the the @ref NISkeletonControllerLineDebugger 
  script. 
- The SkeletonLineRenderer prefab which is based on the @ref NISkeletonControllerLineRenderer script
  (which in turn extends NISkeletonControllerLineDebugger).<br>

@par
Both scripts contain an array, each element of which contains two joints which define a limb (by
telling us to draw a line between them). The difference between the two is the way in which they
draw the lines:<br>
- NISkeletonControllerLineDebugger uses Unity's Debug.DrawLine which means the lines are only drawn 
if the relevant Gizmos are turned on (i.e. mainly used in the scene view and only in the editor).
- NISkeletonControllerLineRenderer on the other hand uses unity's LineRenderer which means it can be
seen in the game even in stand alone mode. In any case the color of the line is based on the 
confidence of its end points.<br><br>

@par 
For the current model, leave this empty as the model is fully textured.<br><br>

<H1>Step 4: Make it into a prefab</H1>
Congratulations you now have a model, all you need to do is make into a prefab (which is already available) 
Under ConstructionWorkerPrefab.
@note In order not to interfere with the process, the imported elements were moved in the package to 
Assets/OpenNI/Graphical assets/Construction Artwork/. This means you can simply import the standard 
assets package with no conflicts.
*/

/**
@page OpenNIMultipleSkeletonsTutorial Multiple Skeletons Sample
This sample expands the previous one by having two “models” controlled by two players. In addition,
it will introduce skeleton built based on game objects rather than fully rigged models (very useful 
for prototyping and debugging).

@note The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.

@note It is assumed that @ref OpenNISingleSkeletonTutorial has already been completed. 
While it is not necessary for the completion of the sample, this tutorial will not repeat elements
explained before.

<H1>Step 1: Creating the basic objects</H1>
Similar to the @ref OpenNISingleSkeletonTutorial, add the @ref OpenNISettingsPrefab to the scene (and
configure it to turn off the image generator). Add the @b RadarViewer and @b DepthViewer prefabs 
as well. In addition, since we might want to see the users, add a NIUserViewer prefab. This will show
the users seen by the sensor as pixels of a specific color (depending on the user id).

<H1>Step 2: Add the skeletons</H1>
Add the FullBodySkeletonCubes prefab and the UpperBodySkeletonCubes prefab to the scene. These two 
prefabs are very similar with one exception; the UpperBodySkeletonCubes prefab has no legs.<br>
These two skeleton models are very similar to the one used in the previous sample but they are also
very different. Their main goal is to provide a quick prototyping and debugging tool. You can make
the game using one (or both) of these prefabs and later on replace them with a different skeleton based
on a fully textured, fully rigged model.<br>
The skeletons are built by manually creating game objects. These game objects are all placed together,
which is why, in the scene viewer, you will just see a heap of objects and nothing which looks like a 
skeleton. <br>
Since these skeletons are not rigged (they are just a hierarchy of game objects), we need to manually
place their joints. The skeleton controller can do this for us by getting information from the 
skeleton. Therefore, unlike the rigged skeleton from the previous sample, the skeleton controller
of these prefabs has the Update Joint Positions check box marked. <br>
Also note that the scale here is different because we are using schematic objects.<br>
Position the two skeleton in y=1.5 and x=+/-2 respectively<br><br>

If we wanted to create these prefabs from scratch, all we  had to do was create a hierarchy of game
objects, add the skeleton controller script to the base object, drag and
drop the various joints to their proper place and configure it the same as the prefab.<br><br>

We want the two skeletons to be controlled by two different players. Make sure the player number of 
the FullBodySkeletonCubes is 0 and the player number for the UpperBodySkeletonCubes is 1.<br><br>


<H1>Step 3: Control the skeleton</H1>
As before, we need to add a player manager.<br>
Create an empty game object, name it Player Manager and add the @ref NIPlayerManagerPoseSelection 
script to it.<br>

Change the <b>Max allowed players</b> field to 2 (we have 2 players). Leave the <b>Num retries</b> as
0 (although a higher number is also an option if you have problems).<br>
We want to wave in order to select and wave again to unselect. Look at the <b>pose to select</b> 
field. Assuming you are using the middleware supplied this should show the Psi pose and a drop down 
menu is available to change it to Wave (among other options). If the field is empty, it means that
the list of legal nodes was not loaded. To load it, press on the <b>Update legal poses</b> button 
(or press play and stop the game immediately after). If you are using a different middleware, other
poses might be available (or even none...).<br>
Assuming you are using the supplied middleware, choose the Wave option.<br>
We also want to be able to unselect (to change players). A player will of course be unselected if 
they leave the field of view of the sensor for long enough but we could also choose a pose to 
unselect. Check the <b>Use unselection pose</b> box and choose Wave again in the <b>Pose to unselect</b>
field which appeared. This means that if someone waves they are selected as a player and if they wave
again they become unselected.<br> 
In order to avoid a situation where the same wave will cause a selection and then an unselection 
(or vice versa), change the <b>Time between switching</b> field to 3. This means that at least 3
seconds without a wave should pass before another wave is accepted for that user.

<H2>Play the game</H2>
Press play.<br>
Two players can play the game now. A user becomes a player if they wave (and 3 seconds after that they
can wave again to get unselected).<br>
@note Waving can some times be missed. To get the best results from waving, make most of your movement
in the wrist, make the movement relatively fast and make the motions not too long.

<H1>Step 4: Adding debugging tools</H1>
In order to better understand how the skeleton behaves we would like to see the skeleton's limbs 
(which is doubly important when using the model for prototyping)<br>
Add the SkeletonLineDebugger prefab to the game. Attach it to the skeleton controller of UpperBodySkeleton. This prefab which implements @ref NISkeletonControllerLineDebugger can be used to see the 
lines connecting the joints. The lines colors will change based on the confidence of the points (which
helps us discover problems with poses where certain joints are hidden from the sensor). These lines are
seen by default in the scene view only. To see them in the game view, enable the gizmos option in the 
game window. @note The lines will not be seen in standalone builds.<br>
Add the SkeletonLineRenderer prefab to the game. Attach it to the skeleton controller of FullBodySkeleton.
This prefab which implements @ref NISkeletonControllerLineRenderer can be used to see the
lines connecting the joints very similar to @ref NISkeletonControllerLineDebugger. The main difference
is that this script uses the LineRenderer component of Unity. This means the following:
- It is possible to change way the lines look (by changing the script).
- The lines will be seen in standalone builds.
- There is a larger overhead for this (drawing the lines,adding game objects to the hierarchy in real time
  for the line renderers).


<H1>Step 5: Making it look better</H1>
<H2>Add light and camera position</H2>
Similar to the previous tutorial, Add a directional light (at rotation 45, 315,45) and change
the camera position to 0,0,-7.
 
<H2>Add messages when we need to calibrate</H2>
We want the users to know what to do in order to be selected.  To solve this, we use the 
CalibrationMessage prefab. Simply drag it to the scene.<br> 
The calibrationMessage prefab uses the @ref NISkeletonCalibrationMessageUtility script to provide a
visual cue to the selection process. It tells us which players are unselected and what action to 
perform to become selected.<br>
- Change the <b>Action to Select</b> to "Wave to be selected (and unselected)". This will provide a
string message telling us to wave<br>
- Check the <b>All players Message</b> box. This will mean the message will be seen as long as even one
player is unselected (the default, of unchecked, means even one player is selected the message is no
longer displayed).
- Drag the WaveGesture image from Graphical assets/Images/ to the @b Image field.<br>

<H2>Press play</H2>
We Now have two skeletons that are waiting for us to control them. We can have two different players
take control of the two skeletons and move them by using the Wave gesture to select and unselect. 
The skeletons are schematic. One of them shows the full body and the other the upper body only.<br>
We can also see which users are identified in the Users Viewer, their calibration state in the Radar 
Viewer and the Depth in the Depth Viewer.<br><br>

<H1>Enjoy your victory, you have completed the sample!</H1>
Continue to the next sample @ref OpenNIInputControlTutorial or return to @ref OpenNISamplesList.
*/



/**
@page OpenNIInputControlTutorial Control a cursor using the input (skeleton-based)

This sample shows how to define Axes using the NIInput class and control a simple cursor with it.

@note The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.

<H1>Step 1: Setting the scene</H1>
As this is the third sample already, we will quickly move through the basic setting of the
scene, which is very similar to previous samples:
- Drag in an @ref OpenNISettingsPrefab and uncheck the "Use Image" check box. Change the smoothing
  factor of the skeleton to 0.95. This will make the movement much more stable and much smoother, 
  which is good when we want to track a specific point.
- Add a Radar Viewer prefab.
- Create an empty game object (name it PlayerMapper) and attach the @ref NIPlayerManagerCOMSelection 
  script to it (for simplicity we will let the closest user control the input). Change the maximum
  allowed players to 1.


<H1>Step 2: Add an input</H1>
<H2>Add the InputControllerPrefab</H2>
Add a @ref InputControllerPrefab to the scene. This prefab contains an @ref NIInput script which 
provide the ability to define axes similar to the regular Unity's InputManager and access them in a
way very similar to the use of the Input class (for more information, see @ref NIInputConcept). The
input is controlled by input sources: Point trackers and gestures (see @ref OpenNIPointTrackerConcept
and @ref OpenNIGestureConcept for more information).<br>

<H2>Add a point tracker</H2>
The @ref InputControllerPrefab contains a gesture manager and a hand tracker manager to manage these
sources but they do not contain any trackers in them. In this tutorial, all we do is move a cursor 
around so we don't really need any gestures, but we do need to decide what to track. <br><br>
Create a new game object, name it Tracker and attach the @ref NISkeletonTracker script to it. 
This script enables us to track a skeleton joint. Let us configure it:<br>
- Look at the <b>Who to track</b> section. This section tells us what joint the tracker is following
    - Set the <b>player Num</b> field to 0 (we want to track the first and only player).
    - Change the <b>Joint to track</b> field to rightHand. This basically tells us what joint is 
      being tracked (for the player above).
- Leave the @b context and <b>Player manager</b> as is (They will be filled automatically).
- Change the <b>StartPos type</b> to  StaticModifierToOtherJoint.
    - This enum (defined in @ref NISkeletonTracker::StartingPosReferenceType) changes 
      @ref NISkeletonTracker::m_StartPosModifier which is responsible to 
      define the starting position (see @ref OpenNIPointTrackerConcept for more information). This
      defines @b HOW to get the starting position.
    - The StaticModifierToOtherJoint is used to set the position as a change from another joint. In
      this tutorial we want the starting position of the right hand to be 30cm to the right and 
      10cm above the torso (which is a comfortable position).
    - Change the <b>cur position of</b> field to Torso.
    - Change the <b>Modified by</b> field to 300,100,0. @note the values are in mm!

.
<H2>Configure the input</H2>
While the tracker is now defined and configured, it is not yet attached it to the manager, so let's 
do that now. Go to the HandsManager child of the InputController game object and change the tracker's
array size to 1. Drag the tracker object there.<br>
We are going to define two axes, one for horizontal movement and the other for vertical movement.
Go to the InputController object and configure it:
- The <b>Hand trackers manager</b> and <b>Gesture manager</b> fields are already defined in the 
  prefab (to be the child objects) so no further configuration is needed on them.
- Open the @b Axes tree. By default it contains 2 axes: Horizontal and Vertical (which match those
  defined in the regular input). 
- Change the @b name field of the Horizontal axis to NI_X (this will control the 'x' movement on the
  screen) and configure it as follows:
    - Make sure the <b>%NIInput axis?</b> checkbox is checked. This will block NIInput from checking
      unity regular input for this axis (as we only define it here and not in unity's Input manager).
    - Leave the @b Gesture field as None (we don't handle gestures in this tutorial).
    - Leave the @b Dead field as 0.001 (we do not need a dead zone).
    - We want to be able to move the cursor comfortably along the entire screen. For this purpose we need
      to define a virtual area to move the hand and normalize the values accordingly.
    - Change the @b normalization field to 320. This provide us a movement range of 32cm in each
      direction of the 'x' axis (this is aimed so we can cover the whole screen without having to 
      stretch out too much but provide sufficient resolution so that not every small change would 
      cause us to jump).
    - Change the @b sensitivity field to 0.5 (the normalization would cause a value between -1 and 1
      and we want a value between -0.5 to 0.5)
    - Leave the @b Invert checkbox as unmarked (we do not want to invert the movement).
    - Change the @b type to HandMovementFromStartingPoint. This is used to enable an easy
      movement range where the center of the movement is in the middle of the movement range.
    - Make sure the @b Axis field is xAxis
    - Change the <b>Tracker to use</b> field to "skeleton tracker for player 0 tracking joint 
      RightHand" this is the text defined for the skeleton tracker we added to the manager.
- Change the @b name field of the Vertical axis to NI_Y (this will control the 'y' movement on the
  screen) and configure it as follows:
    - Make sure the <b>%NIInput axis?</b> checkbox is checked. 
    - Leave the @b Gesture field as None.
    - Leave the @b Dead field as 0.001 
    - Change the @b normalization field to 240. This is different than the 'x' axis to preserve
      the sensitivity (based on the ratio of the sensor).
    - Change the @b sensitivity field to 0.5 
    - Check the @b Invert checkbox (the y axis of the sensor and the y axis of the screen are inverted).
    - Change the @b type to HandMovementFromStartingPoint. This is used to enable an easy
      movement range where the center of the movement is in the middle of the movement range.
    - Make sure the @b Axis field is yAxis
    - Change the <b>Tracker to use</b> field to "skeleton tracker for player 0 tracking joint 
      RightHand" this is the text defined for the skeleton tracker we added to the manager.

<H1>Step 4: Add the cursor</H1>
Create an empty game object (Name it cursor) and attach the @ref NIHandPositionInputPositionOnly 
script to it. This script positions a box (using OnGUI) on a position based on the GetAxis result. 
<br><br> 
<H2>Press play.</H2>
Watch the cursor move (in the 2D world) by moving your hand around hand.<br><br>

<H2>Food for though</H2>
This tutorial assumed the input is based on movement only. NIInput is aimed at easy integration with
the regular input so that it is easy to have two types of controls in one game: both natural 
interaction and classic.<br>

An easy way to do this would be to use the same name for both inputs (e.g. use the Horizontal and
Vertical names for the axes and uncheck the <b>NIInput axis?</b> box. This will get the axis 
information from both inputs. To make it behave properly, just set the dead zone to 0.99 and the
sensitivity to 1 and use the values of +/-1 achieve a behavior such as clicking the keyboard. A 
similar behavior can be achieved by using the DeltaHandMovement type to simulate mouse movement.

<H1>Enjoy your victory, you have completed the sample!</H1>
Continue to the next sample @ref OpenNINIGUITutorial or return to @ref OpenNISamplesList.


*/

/**
@page OpenNINIGUITutorial NIGUI (skeleton-based)

This sample shows us how to create a GUI using the NIGUI tools. We will be seeing various GUI
elements and we will be able to control them by using the right hand of the first player. In order
to simulate a left mouse click, e.g. to click on a button, we will use the timed steady gesture.
This means that when the right hand does not move, a counter will begin and a click will occur after
several seconds.

@note 
- The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.
- We assume the previous sample was followed, as input etc. is not explained again.
- To properly view the GUI please set the resolution of the screen to 1024*768 and maximize the view. If this is 
  not done, some of the controls might be drawn outside the screen.
 
<H1>Step 1: Add the basic objects:</H1> 
Begin similar to previous samples:
- Drag in an @ref OpenNISettingsPrefab and uncheck the "Use Image" check box. Change the smoothing
  factor of the skeleton to 0.95. This will make the movement much more stable and much smoother, 
  which is good when we want to track a specific point.
- Add a Radar Viewer prefab.
- Create an empty game object (name it PlayerMapper) and attach the @ref NIPlayerManagerCOMSelection 
  script to it (for simplicity we will let the closest user control the input). Change the maximum
  allowed players to 1.

<H1>step 2: Create and configure the input:</H1> 
We will be using the input to control the GUI. We will do so by tracking the position of the right
hand of the first player. We will click using the steady gesture (for which we will set a timed click).

<H2>Create the tracking and gesture objects and the input object</H2>
<H3>create a skeleton point tracker.</H3>
This is very similar to the previous sample: Create a new game object, name it Tracker and attach 
the @ref NISkeletonTracker script to it. Configure it as follows:
- Set the <b>player Num</b> field to 0.
- Change the <b>Joint to track</b> field to rightHand.
- Leave the @b context and <b>Player manager</b> as is.
- Change the <b>StartPos type</b> to  StaticModifierToOtherJoint.
    - Change the <b>cur position of</b> field to Torso.
    - Change the <b>Modified by</b> field to 300,100,0.

<H3>Create the gesture.</H3>
Create an empty game object (name it GestureFactory) and drag the NISteadySkeletonGestureFactory to
it. The gesture factory is used to create the gesture (see @ref OpenNIGestureConcept. For more
details on the settings below, see @ref NISteadySkeletonGestureFactory).
- Set the Time to click to 2 seconds (this is how long we need to wait)
- Set the Time to reset to 10000 (we don't want to reset)
- Set the Steady Test time to 0.5 (we check the steady over 0.5 seconds)
- Set the SteadyStdSqrThreshold to 2 
- Set the UnsteadyStdSqrThreshold to 8 
- Set the MaxMoveFromFirstSteady to 40

<H3>Add an input controller and configure it</H3>
Again this is similar to the previous sample. Add a new Input controller prefab into the scene and
configure it as follows:
- Go to the HandsManager child of the InputController game object and change the tracker's 
  array size to 1. Drag the Tracker game object there.<br>
- Go to the GestureManager child of the InputController game object and change the gesture's 
  array size to 1. Drag the GestureFactory game object there.<br>

We will be using the default cursor (see below), which forces us to define the following axes:
- NIGUI_X to move along the x axis
- NIGUI_Y to move along the y axis
- NIGUI_CLICK to click

For this purpose, make sure the axes are configured as follows:
- NIGUI_X
    - Name: NIGUI_X
    - NIInput axis?: select the check box (this will NOT be defined in InputManager)
    - Gesture: None
    - Dead: 0.001
    - Sensitivity: 0.5
    - Invert: not checked
    - Type: HandMovementFromStartingPoint
    - Normalization: 320
    - Axis: xAxis
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"
- NIGUI_Y
    - Name: NIGUI_Y
    - NIInput axis?: select the check box (this will NOT be defined in InputManager)
    - Gesture: None
    - Dead: 0.001
    - Sensitivity: 0.5
    - Invert: not checked
    - Type: HandMovementFromStartingPoint
    - Normalization: 240
    - Axis: yAxis
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"
- NIGUI_CLICK
    - Name: NIGUI_CLICK
    - NIInput axis?: select the check box (this will NOT be defined in InputManager)
    - Gesture: SteadySkeletonGesture
    - Dead: 0.001
    - Sensitivity: 1
    - Invert: not checked
    - Type: Gesture
    - Normalization: -1
    - Axis: xAxis
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"

<H1>Step 3: Add a %NIGUI prefab</H1>
Add the NIGUI prefab to the scene.<br>

Since we wish to showcase the GUI we need a GUI system. To do this we will use %NIGUI (see 
@ref NIGUIConcepts). The @ref NIGUIPrefab provides a quick and easy way to add it to the scene. 
The configuration for NIInput in the previous step was done with the prefab's cursor in mind so
we already have a fully functioning %NIGUI.<br><br>


<H1>Step 4: Add the GUI example script</H1>
While we now have a functioning GUI system, we still need to create the actual buttons, scrollbars
etc. To showcase all of the supported functionality and GUI elements (such as buttons, scroll area, 
sliders etc.) a simple script @ref GUIExample is provided as part of the samples scripts.<br>

Create an empty game object (name it ExampleGUI) and attach the @ref GUIExample script to it. 

<H2>Press play.</H2>
Enter the scene and click on the various buttons, change the sliders and interact with the GUI.

@note 
- In order to click, place the cursor in the relevant position and leave your hand steady there. 
  The cursor will slowly change color. When it is fully green, it will click. If the hand is not 
  steady enough, the click will not occur.
- When using sliders (and scroll bars), you can click on the actual scroll thumb, which will make
  the scrollbar/slider move with the movement of the hand until a new click is used.
- Some of the labels will react to clicking on buttons, sliders etc. to provide feedback.

<H1>Enjoy your victory, you have completed the sample!</H1>
Continue to the next sample @ref OpenNISimpleGameTutorial or return to @ref OpenNISamplesList.
*/


/**
@page OpenNISimpleGameTutorial A simple game example

In this sample, we will create a very simple game using everything we have learned so far. The game will
include a skeleton (build from the cubes). In addition, balls will fall from the sky in random 
intervals and random places. The user will be able to hit the balls, sending them flying. A score
board will count the number of balls that have fallen and how many the user hits. The user will be able to
strike a pose that will stop the game and show two buttons: to continue or to quit.

@note 
- The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.
- We assume the previous sample was followed, as input etc. is not explained again.

<H1>Step 1: Add the basic objects:</H1> 
Begin similar to previous samples:
- Drag in an @ref OpenNISettingsPrefab and uncheck the "Use Image" check box. Change the smoothing
  factor of the skeleton to 0.95. This will make the movement much more stable and much smoother, 
  which is good when we want to track a specific point.
- Add a Radar Viewer prefab.
- Create an empty game object (name it PlayerMapper) and attach the @ref NIPlayerManagerCOMSelection 
  script to it (for simplicity we will let the closest user control the input). Change the maximum
  allowed players to 1.
- Add the FullBodySkeletonCubes prefab. Make sure the <b>Player number</b> field is 0.

Pressing play now would allow us to see the users in the Radar viewer and control the skeleton.<br>

<H1>Step 2: Add the balls creation</H1>
We want to start dropping balls from the sky.<br>
- Create an empty game object (name it %BallCreator) and attach the @ref BallCreator script to it.
    - The ball creator is a simple script that creates the balls, counts them and shows (through the OnGUI
      method) the current score. 
- Drag and drop the BallPrefab prefab to the "prefab" variable. 
    - The BallPrefab is a simple sphere rigid body aimed at colliding with the joints. The @ref NICollider 
      script was added to it which basically applies force based on the collision.
- Drag and drop the skeleton (i.e. the FullBodySkeletonCubes object) to the "where" variable. 
    - The script creates the balls relative to the skeleton's position.<br><br>



<H2>Make the balls fly away when hit</H2>
If you would press play now you would see see balls falling from the sky but if you hit them, 
nothing will happen even though we would expect the balls to be thrown away (or rather the 
@ref NICollider script to handle this). <br>
This happens because the @ref NICollider script's OnTriggerEnter method which handles
the collision,  moves the balls by applying a force in the direction of the collider's (the joint)
speed. The joints however are moved by the skeleton controller, which positions them in the correct 
place but does not change their speed (if it did then the joints might have moved differently than 
the positioning). Because of this the joint's speed is always 0.<br> <br>
To solve this we need to manually calculate the speed of the joints and we do this with the 
@ref JointSpeeder script.<br>
The JointSpeed script (when attached to a joint) is responsible for calculating its speed based on 
its position over time. The @ref NICollider in turn uses this value to choose the direction it
will apply force to.<br>
Add the @ref JointSpeeder script to every joint in the skeleton (allowing all joints
to hit the balls). @note This will break the prefab connection of the skeleton.

<br>
<H2>Press play</H2>
Now you can hit the balls and they will fly away.

<H1>Step 3: Choosing an exit gesture</H1> 
We want the player to be able to end the game. For this purpose we need to identify a gesture which
will bring up the game menu. This game menu will enable the player to either resume playing or
exiting. OpenNI arena recommends using the cross hands pose for exiting. To achieve this we can do
one of the following:
- The user node might support the required pose. Indeed the supplied NITE implementation has support
  for the CrossHandsPose. The @ref NIUserPoseGestureFactory can be used to create a gesture from any
  user pose. We will use this gesture for the exit pose in this tutorial.
- Not all implementations support the CrossHandsPose. So you can always create the appropriate gesture
  yourself. In order to learn how to create gestures (by using the exit gesture as an example), 
  follow the @subpage OpenNIGestureTutorial. tutorial<br>

<H1>step 4: Add the GUI</H1> 
Although we have now created an exit gesture, we have not yet started to use it. 

<H2>Create the GUI hierarchy</H2>
We will first create an empty game object and name it GUI. All game objects below
should be children of that game object (this is used to clean up the hierarchy and not for any code
reason).

- Create an empty game object (name it Gestures) and create two empty child game objects for it. 
Name them SteadyGesture and ExitGesture.
- The ExitGesture object will be used to know when the player wants to open up the game menu. 
    - Attach the @ref NIUserPoseGestureFactory script to the ExitGesture game object and configure
      it as follows:
        - Change the <b>Pose to detect</b> field to CrossHandsPose @note If there is no pull down 
          menu with poses it usually means that the list is not updated. Either run the game or press
          the <b>Update legal poses</b> button. If there is still no list then it means the node does
          not support poses (use the tutorial exit pose instead).
        - change the <b>Time to hold pose</b> field to 1 seconds.
    - @note If you want to use the exit pose created in the @ref OpenNIGestureTutorial instead of 
      the @ref NIUserPoseGestureFactory solution, Attach the @ref NIExitPoseGestureFactory to the 
      ExitGesture game object. Set its parameters as follows:
           - Set the Time to hold pos to 1 seconds (this is how long we need to wait)
           - Set the max move speed to 100 (we allow movement of up to 10 cm per second overall)
           - Set the Time to save points to 0.5 (we will consider points along 0.5 second)
           - Set the Angle tolerance to 20 (this means that we don't really need 45 degrees for each 
             arm, but rather that 20-65 degrees is suitable).
- The @ref NISteadySkeletonGestureFactory script will be used to "click" on buttons (similar to the  
  @ref OpenNINIGUITutorial sample). 
    - Attach the NISteadySkeletonGesture script to the SteadyGesture game object and configure as 
      follows:
        - Set the Time to click to 2 seconds (this is how long we need to wait)
        - Set the Time to reset to 10000 (we don't want to reset)
        - Set the Steady Test time to 0.5 (we check the steady over 0.5 seconds)
        - Set the SteadyStdSqrThreshold to 2 
        - Set the UnsteadyStdSqrThreshold to 8 
        - Set the MaxMoveFromFirstSteady to 40
- Create a new game object, name it Tracker and attach the @ref NISkeletonTracker script to it. 
  Configure it as follows:
    - Set the <b>player Num</b> field to 0.
    - Change the <b>Joint to track</b> field to rightHand.
    - Leave the @b context and <b>Player manager</b> as is.
    - Change the <b>StartPos type</b> to  StaticModifierToOtherJoint.
        - Change the <b>cur position of</b> field to Torso.
        - Change the <b>Modified by</b> field to 300,100,0.
- Add an InputController prefab to the scene. Attach the point tracker and two gestures to its managers.
- Add a NIGUI prefab to the scene.

<H2>Configure the input </H2>
We will be using the default cursor (see below), which means we need to use the following axes:
- NIGUI_X to move along the x axis
- NIGUI_Y to move along the y axis
- NIGUI_CLICK to click

Configure the axes as follows:
- NIGUI_X
    - Name: NIGUI_X
    - NIInput axis?: Select the check box (this will NOT be defined in InputManager)
    - Gesture: None
    - Dead: 0.001
    - Sensitivity: 0.5
    - Invert: not checked
    - Type: HandMovementFromStartingPoint
    - Normalization: 320
    - Axis: xAxis
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"
- NIGUI_Y
    - Name: NIGUI_Y
    - NIInput axis?: Select the check box (this will NOT be defined in InputManager)
    - Gesture: None
    - Dead: 0.001
    - Sensitivity: 0.5
    - Invert: Checked
    - Type: HandMovementFromStartingPoint
    - Normalization: 240
    - Axis: yAxis
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"
- NIGUI_CLICK
    - Name: NIGUI_CLICK
    - NIInput axis?:Select the check box (this will NOT be defined in InputManager)
    - Gesture: SteadySkeletonGesture
    - Dead: 0.001
    - Sensitivity: 1
    - Invert: not checked
    - Type: Gesture
    - Normalization: -1
    - Axis: xAxis
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"

In addition, we want to detect the exit gesture based on the input as well.<br>
Configure a fourth axis as follows:

- SkeletonSwitch
    - Name: SkeletonSwitch
    - NIInput axis?: Select the check box (this will NOT be defined in InputManager)
    - Gesture: ExitPoseGesture
    - Dead: 0.001
    - Sensitivity: 1
    - Invert: not checked
    - Type: Gesture
    - Normalization: -1
    - Axis: xAxis
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"

<H1>Step 4: Add the GUI control script</H1>
While we have configured the GUI, we have not yet added the GUI itself. <br>
Attach the @ref SkeletonGuiControl script to the GUI game object and set its image variable to be 
the exit_sample image (in the Graphical assets/Images directory). <br>
This script keeps tabs on the mode we are in (@ref SkeletonGuiControl.SkeletonGUIModes). We begin in
SkeletonMode, where the skeleton is seen and the GUI is deactivated. In this mode, 
@ref SkeletonGuiControl simply tries to locate the exit pose. <br>
Once the pose is detected, a GUI telling us to hold the pose will appear and after a little time we
will move to GUIMode in which the skeleton is deactivated (can't be seen) and the GUI appears.<br><br>

<H2>Press play</H2>
Playing the game now will include all the functionality of the game. We can see the balls, hit 
them, keep score and when we stand in the exit pose (hands crossed in front of us) for a couple
of seconds, a GUI will appear allowing us to return to the game or exit.
@note Because of the use of Application.Quit, exiting will only work in the standalone version. Inside 
the editor it will do nothing.

<H1>Step 5: Improve the way it looks</H1>
We now have a functional game but we should make it look better:
- Set the camera options:
    - Position the camera at 0,0,-10
    - Set the camera's background to brown (R=160, G=145,B=125,A=255)
    - Add the smoothFollow script. Set its parameters as:
        - Target: drag & drop the skeleton controller main object
        - Distance: 7
        - Height: 0
        - Height: 2
        - Height: 3
- Add a floor
    - Create a new plane game object. Rename it to Floor.
    - Position it at 0,0,0 and scale it to 10, 10, 10
    - On its mesh renderer, drag in the Floor material (Available from General Assets->Samples Assets->Floor assets->Materials
    - @note We do not correct the skeleton's position relative to the floor. 
    - @see @subpage PinningToTheFloor
- Add a directional light
    - Set the direction to: 45,300,45

<H2>Press play.</H2>
You now have a fully functioning (albeit simple) working game. The user closest to the sensor 
controls the skeleton movement. Balls drop from the sky and the player needs to hit them. A score 
board is used to see the current score (how many balls were hit out of how many). The user can 
strike a pose (the exit pose), which will stop the game and show two buttons; to continue or to quit.

<H1>Enjoy your victory, you have completed the sample! Now you know how to use the OpenNI module</H1>
return to @ref OpenNISamplesList.
*/


/**
@page OpenNIGestureTutorial Adding a New Gesture Example
<H1>Overview</H1>
One of the most common extensions to this package is the addition of new trackers (and mostly gesture trackers).
In the future, gesture packs will probably be added.<br>
For this purpose, it is important to understand how to add gestures and new trackers. For simplicity, 
we will focus on creating a new gesture.<br>
It is not uncommon for a game to have two modes: The first is the actual playing of the game (in which we 
control the skeleton, move around and actually play). The second is a UI mode in which we are presented 
with a menu (resume, exit, save, load etc.).<br>
In order to access the menu in the middle of a game session, a special movement must be made. This is commonly known as the exit pose,
as it is also used to exit the program. The exit pose recommended by OpenNI Arena is to stand erect with
the hands crossed in front of the body. For our purposes, we will use the following, simplistic 
implementation.<br>
We will track the right and left hands and elbows and we will consider the user to be in the relevant pose once the 
following conditions are met:
- There is very little movement (very low average speed).
- The right hand is angled around 45 degrees toward the left
- The left hand is angled around 45 degrees toward the right
- The right hand is to the right from the left
- Both hands are above their opposite elbows.

@note This is a very simplistic implementation, useful mainly for demonstration and explanation and not ready for 
production. It might catch other gestures that are not the exit pose and may miss the pose on certain angles
(where the hands and elbows are too close to the body for example). <br>
For our purposes, however, this is enough since we do not have any conflicting poses in our sample.<br>

To implement it, we will need to implement two classes:
- @ref NIExitPoseDetector - This will perform the actual detection of the exit pose
- @ref NIExitPoseGestureFactory - This is used to create and initialize the detector.

<H1>Creating the Detector</H1>

Implementing a gesture detector is relatively easy, all we need to do is extend the @ref NIGestureTracker
abstract class.<br>
In order to track the positions, we will use the @ref NITimedPointSpeedListUtility class. We will add the
position of the hands and elbows to objects of this class so that we have an average position and speed
for each of them.<br>

@note The @ref NIExitPoseDetector is well documented (both at the API level in @ref NIExitPoseDetector and inside the 
methods themselves to understand how they work). Because of this, we will not explain every detail of the 
implementation but instead will stay at a high level and leave the reader to delve in.

The implementation of the @ref NIGestureTracker abstract class can be divided into the following main parts:

<H2>Initializing, updating and releasing</H2>
<H3>Initialization</H3>
@ref NIExitPoseDetector (as a child of @ref NIGestureTracker) is @b NOT a mono-behavior object. Instead it is created
using "new" in the @ref NIExitPoseGestureFactory. For this purpose some of its initialization is done using a 
constructor (see @ref NIExitPoseDetector.NIExitPoseDetector "NIExitPoseDetector").<br>

@ref NIGestureTracker provides various initialization methods. While @ref NIGestureTracker.Init "Init" is the one
often visible to other methods, @ref NIGestureTracker.InternalInit "InternalInit" is the one to be overridden by the implementer.
In our case, we need the hand tracker to be a skeleton and therefore we check that this is true as part of the
initialization.<br><br>



<H3>Updating</H3>
@ref NIGestureTracker.UpdateFrame "UpdateFrame" is called every frame (by the @ref NIGestureFactory object). This is used to update
the tracking information. In our case, this is used to fill up the points of where the relevant joints (hands, elbows)
are located and to analyze them to see if we are in the pose. Furthermore, it is assumed that the pose is considered
"triggered" only when we have held it for a while. This is also checked in the updating (and when relevant an event
is triggered).
<H3>Releasing</H3>
The @ref NIGestureTracker.ReleaseGesture "ReleaseGesture" method is used to release everything we need. 
In this case, this is trivial.

<H2>Gesture detection feedback</H2>

@ref NIGestureTracker provides several options to query for the state of the gesture:
- @ref NIGestureTracker.GetLastGestureTime "GetLastGestureTime" - tells us the time (in Time.time) the gesture 
  was last detected.
- @ref NIGestureTracker.GetLastGestureFrame "GetLastGestureFrame" - tells us the frame (in Time.frameCount) the
  gesture was last detected.
- @ref NIGestureTracker.GestureInProgress "GestureInProgress" - tells us if the gesture is in progress and how it is 
  progressing. 
  @note While we have an internal detection of the pose (i.e. telling us that the user is holding the exit pose) this
  is @b NOT a detection of the gesture. For many gestures, the behavior is not a "detected"/"not detected" result but
  rather has a progress. For our exit pose, once we detect the pose for the first time, the user has to hold that pose
  for a certain time. For this reason, the result of the detection is a value between 0 (not detected) and 1 (detected
  and held for the time we need). A value in the middle represents the portion of the holding time (so if we need to
  hold the pose for 2 seconds and 1 second has past then the value would be 0.5).<br>
  Other gestures might have various meaning for this flow. For example, if we have a gesture of sending the arms in a 
  "T" pose, we might start detecting when the arms are at 80 degrees from the body and slowly rise in value to the 
  full effect at 90 degrees.
- Using events - If one wants to know immediately when a gesture is detected (in our case held for the relevant time)
  events should be used. For this purpose, @ref NIGestureTracker provides us with an event handler 
  (@ref NIGestureTracker.m_gestureEventHandler "m_gestureEventHandler" using the delegate 
  @ref NIGestureTracker.GestureEventHandler "GestureEventHandler") we can use to register or unregister the event to.

<H2>Implementing the detection</H2>
The logic of detecting the actual pose is implemented by getting the positions of the joints (hands, elbows) in every
frame (see @ref NIExitPoseDetector.FillPoints "FillPoints"). A test is then performed to see if we are in
pose (inside @ref NIExitPoseDetector.TestExitPose "TestExitPose", which also checks the speed in
@ref NIExitPoseDetector.IsSteady "IsSteady"). The @ref NIExitPoseDetector.UpdateFrame "UpdateFrame"
method calls all these methods and does the book keeping to check when the pose was first detected. 
When enough time has passed it calls NIExitPoseDetector.InternalFireDetectEvent "InternalFireDetectEvent" (which calls
@ref NIGestureTracker.DetectGesture "DetectGesture") to fire the event.

@note In our example, we use only the player number from the hand tracker. On other gesture trackers (such as 
the @ref NISteadySkeletonHandDetector) we track the actual point.

<H1>Creating the factory</H1>
The factory creation is even simpler than the detector creation. The factory extends @ref NIGestureFactory, which 
is responsible for the update method (to call update frame) and to provide an inspector to configure everything. 
For this reason all @ref NIExitPoseGestureFactory needs to do is add the public variables
(to be configured in the inspector) and create a new @ref NIExitPoseDetector inside 
@ref NIExitPoseGestureFactory.GetNewTrackerObject "GetNewTrackerObject" with the relevant constructor parameters, 
and then return it.<br>
We also implement the @ref NIExitPoseGestureFactory.GetGestureType "GetGestureType" method that provides us with a 
string. This string is used by @ref NIGestureManager to provide inspectors, such as the @ref NIInput Inspector 
(@ref NIInputInspector), a name to enable the user to distinguish between gestures.

<H1>Important notes</H1>
The exit gesture here is a very simple example of how to create a gesture. It is @b NOT meant as a 
robust solution!<br><br>
The exit gesture is built upon @ref NISkeletonTracker which implies tracking a specific joint, but 
it is in fact irrelevant which joint is used (only the player matters because the whole skeleton is
used to detect it).

*/
