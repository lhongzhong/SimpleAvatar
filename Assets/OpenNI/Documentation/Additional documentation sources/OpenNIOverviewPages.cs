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
// This file is used for documentation rather than code.
// This holds the general pages for the product (overview, additional topics etc).


/**
@page Introduction Introduction
<H1>Introduction</H1>
The purpose of this document is to provide an overview of the structure and usage of the OpenNI Unity Toolkit. 
The target audience for this document are game developers who wish to create motion control games in Unity that 
take advantage of Natural Interaction&reg; capabilities.<br><br>

The Unity assets for the toolkit provide all a game developer needs to create motion controlled games in Unity, 
with OpenNI compliant sensors, such as the ASUS Xtion Pro Live which can be purchased at www.openni.org/hardware.<br><br>

In fact, creating motion controlled games on top of @subpage OpenNIFramework is already broadly done by OpenNI vibrant 
developers' community, using the various API wrappers that are provided with OpenNI Framework. However, this requires
learning the entire framework. The OpenNI Unity Toolkit builds upon this existing framework and provides tools that are
easy to use inside Unity, and that act in accordance with the Unity spirit. <br><br>

For this purpose, the toolkit provides a new framework inside Unity that packages everything in a Unity friendly 
fashion. This hides a lot of the internals of OpenNI (and its implementations) behind easy-to-use prefabs and 
mono-behavior objects, and provides drag & drop functionality, easy-to-configure inspectors, ready-made objects 
full of functionality, and extensions to existing Unity tools, such as Input and GUI. In addition, documentation 
and samples are provided to make it as easy as possible for the Unity game developer to simply dive in and create
amazing motioned controlled games.<br><br>
The product is packaged as assets that can be downloaded from the Unity Asset store.<br><br>

*/

/**
@page OpenNIPackageOverview OpenNI Package Overview
The idea behind the OpenNI package is to build upon the @ref OpenNIFramework to provide tools that 
are easy to use inside Unity and to work with them in the Unity spirit.<br><br>

In order to better understand the package, you should read the following chapters first:
- @subpage OpenNIMainCapabilities
- @subpage OpenNIFileOrganization
- @subpage OpenNIBeforeYouStart
- @subpage OpenNISamplesOverview
- @subpage OpenNIAdditionalTopics

@see @ref OpenNIPackage
<br><br><br>


In addition, full documentation is provided for all files, classes and constructs.
<H1>Further Information</H1>
 Feedback and discussion on the toolkit is available at: OpenNI-dev google group: http://groups.google.com/group/openni-dev?hl=en
<br><br>
*/

/** 
@page OpenNICentralizedInitializationAndConfigurationCapability Centralized Initialization and Configuration
One of the main goals of this package is to enable developers to easily use OpenNI to create games.
To enable this, all the internals of how to initialize and build an OpenNI applications are wrapped in
a single prefab that is used to initialize and configure OpenNI. This means that most developers will
never need to learn the internals of the @ref OpenNIFramework and instead can just add the prefab to their
scene and configure it using the inspector. More advanced users who need to get into the internals can
always use the programmatic accessors provided. This basic capability is the base for all other 
capabilities provided by this package.<br><br>

The entire wrapper is implemented in the @ref OpenNIBasicObjects module, while the prefab itself is
described in @subpage OpenNISettingsPrefab. 
*/

/**
@page OpenNISkeletonControlCapability Skeleton Control
The package is designed to make it extremely easy to gather skeleton information and to use it to control 
3D models. As such, one of the main capabilities provided is this control, which includes three main
elements:
- The ability to connect a 3D model to the skeleton controller so that the human player can control the 
  movements of that character using his own movement. This includes tracking various joints (hands, 
  legs, torso, head etc.).
- Managing several players and providing user selection capabilities.
- Tracking any joint on the skeleton and specifically tracking the position of the
  skeleton hand. This also includes detecting gestures (currently a limited selection mainly to show how
  it is done) that can be used to trigger actions. 

@see @subpage UserSelectionCapabilities and @ref NISkeletonController 
*/



/**
@page OpenNIInputManagementCapability Input Management
The package provides a new Input class (@ref NIInput) that can be used instead of the regular Input class. 
This enables adding Natural Interactions sensitive axes, as well as adding Natural Interactions to regular
Input Manager-defined axes.<br>
The Input can work seamlessly with skeleton hand tracking (similar to mouse movements) and use gestures, 
or more complex movement, to simulate clicks. It can work with any tracker.<br><br>
@see @subpage NIInputConcept, which is implemented in the @ref OpenNIInput module.
*/

/**
@page OpenNIUtilitiesCapability Utilities
The package includes various utilities included in the @ref OpenNIUtilities module. These utilities are
aimed at debugging tools, and tools that aim to show what things SHOULD look like but are
not polished enough to be considered a product.<br>
Two main groups of utilities are worth noting:
- Viewer utilities (see @ref OpenNIViewerUtilities)
- NIGUI utilities (see @subpage NIGUIConcepts and @ref OpenNIGUIUtiliites)
*/


/** 
@page OpenNIMainCapabilities OpenNI Module Main Capabilities

The @ref OpenNIPackage provides the following main capabilities:

- @subpage OpenNICentralizedInitializationAndConfigurationCapability
- @subpage OpenNISkeletonControlCapability
- @subpage OpenNIInputManagementCapability
- @subpage OpenNIUtilitiesCapability

In addition, various useful prefabs and scripts are provided in the OpenNISamples module.


*/

/**
@page OpenNIFileOrganization File Organization for the OpenNI Module

The resources and assets of the @ref OpenNIPackage are organized into a folder tree under the 
Assets/OpenNI directory. In addition, custom inspectors are provided in the Editor directory.
<br>
The OpenNI directory contains the following:
- <b>Documentation</b> - This directory holds the main documentation for the package. This includes:
    - <b>OpenNIPackageDocumentation.chm file</b>: This is the main documentation file, that holds 
       everything needed in order to use this package.
    - <b>OpenNIDoxyfile</b>: This is a doxygen configuration file aimed at creating the documentation. 
       @note It will create html files on a Documentation/OpenNI/ directory on the same level as the 
       Assets directory for those who prefer HTML to chm.
    - The <b>"Additional Documentation Sources"</b> directory: This directory includes files used for creating the
       documentation. It includes .cs files for each module, as well as additional ones, such as the
       Overview. The OpenNIMainpage contains the main page. @note The documentation for the actual code is inside the code.
    - The <b>Images</b> directory: This holds images used in the documentation (such as the OpenNI
                                   logo)
    - <b>ExampleRecord.xml</b>: This is an example for playing a recording. See 
                                @ref OpenNIRecordAndPlayback "Recording and playing back sensor data" 
                                for more information.
    - <b>SampleXML.xml</b>: This is an example xml for limiting the nodes to the supplied
                            NITE implementation and to show how to set the log to a file. For
                            more information see @ref OpenNIAdditionalTopics and the internal
                            documentation of the xml file.
- <b>Graphical assets</b> - This directory contains the various graphical assets which are used in the
  samples and include 3D models, textures, materials etc. 
- <b>Interface library</b> - This directory is divided into two sub-directories.
    - <b>Installer</b>: This holds the installer for the OpenNI framework, an OpenNI
       implementation (NITE) and the sensor's driver.<br>
       This is required both for development (see @ref OpenNIBeforeYouStart) and for deployment (see
       @ref OpenNIAdditionalTopics) as an updated version of OpenNI, an implementation of it
       (such as NITE) and the sensor drivers @b MUST be installed before any development begins and
       before playing any game using this package.
    - <b>OpenNI engine</b> - this includes a C# .Net dll containing the OpenNI interface.
- <b>Prefabs</b>: This directory is divided into three sub directories, one for each sub module of
  the @ref OpenNIPackage and includes: 
    - <b>Main OpenNI prefabs</b>: Contains the main prefabs, i.e. those useful to anyone who uses the 
       package.
    - <b>Samples prefabs</b>: Includes prefabs used in the samples.
    - <b>Utility prefabs</b>: Includes prefabs used as utility (aimed at debugging or being extended 
       by the developer)
- <b>Sample scenes</b>: This directory holds the end product of all samples, i.e what they should 
  look like after the tutorials are finished.
- <b>Scripts</b>: This is where all the code is located. The code is divided into sub-directories based on
  the sub-module they belong to.
    - <b>Main OpenNI</b>: Contains the main scripts. This is the basis for the entire package.
    - <b>Samples</b>: Contains scripts specific for the samples. They are needed to make the samples
       work and may be used by the developer as implementation examples.
    - <b>Utilities</b>: Contains scripts that supply debugging tools and provide a basis for the
       developer to extend capabilities (@ref NIGUIConcepts).
*/

/**
@page OpenNIBeforeYouStart Before you Start

Before you start developing using the @ref OpenNIPackage, you should first:<br><br>
<H1>Download the Package</H1>
Download and import the OpenNI Unity Toolkit. The easiest way to do this is to download it from the Unity Asset store.
 
<H1>Install OpenNI Framework, Implementation and the Sensor Driver</H1>
The OpenNI Unity Toolkit assumes updated versions of the following 3 components are already installed on the system. 
Without these, you cannot use this package:
- OpenNI framework
- An OpenNI compliant middleware (a middleware component that implements OpenNI API and provides motion tracking capabilities)
- Sensor driver
There are two ways to install them:
- This is based on the Primesense&trade; implementation and drivers. For sensors which use other drivers, 
  please refer to the installation media that came with the sensor or to the sensor developer's site 
  @note Two versions of the installer are supplied. They can both be used for the purpose of development 
  (and deployment). The difference is that while the "PrimeSense-Win32-FullInstaller-Redist.exe" version has 
  just the required binaries, the "PrimeSense-Win32-FullInstaller-Dev.exe" version contains samples, headers and 
  documentation. If you wish to do any development with the original framework from the .net dll or wishes to use
  recording of sensor data (see @ref OpenNIRecordAndPlayback "Recording and playing back sensor data") then the second
  ("Dev") version should be used. Otherwise the "Redist" version is enough.

   @note The installer installs the Primesense&trade; driver, for sensors from other providers, please refer to the 
   installation media that came with the sensor or to the sensor developer's site.
- Download an updated version from the Internet:
    - From the download page of <a href="http://www.openni.org/">OpenNI.org</a> 


@note <b> You MUST install the updated version before starting to develop. </b>

<br>

@note This package is aimed at development in Windows only. It was not tested on any other environment.


<H1>Documentation</H1>
Documentation is available under Assets/Documentation/PackageDocumentation.chm<br>
In addition to the documentation, a doxyfile is provided, in the same directory. It can be used to 
recreate the chm file AND create html (in a Documentation directory parallel to Assets).<br><br>
The best way to learn how to use this package is to read the sample tutorials
*/

/** 
@page OpenNIAdditionalTopics Additional Topics

<H1>Deployment</H1>

<H2>Creating an Installer</H2>
As stated in the @ref OpenNIBeforeYouStart section, one needs to install the OpenNI framework, an OpenNI
compliant middleware and the sensor drivers before starting to develop. This is also true for deployment.<br>
It is the responsibility of the developer to create an installer that installs an updated version of
the OpenNI framework, an OpenNI compliant middleware and the sensor drivers before running the game. 
For simplicity, a Windows installer is provided under 
Assets/Interface library/Installer/PrimeSense-Win32-FullInstaller-Redist.exe.<br>

The creation of the installer for the deployment target is the responsibility of the developer.

@note The installer installs the Primesense&trade; driver, for sensors from other providers, please refer to the 
installation media that came with the sensor or to the sensor developer's site.

<H2>Supported Environments</H2>
This package was tested for creating Windows standalone deployment only, and the editor was tested
on windows only. 
While in theory, any environment for which OpenNI is supported should work, no other environment was tested and no
installer is supplied. 

@note This package was tested on Unity 3.4 only. 

<H2>Setting queries</H2>
OpenNI can load different implementations of the basic nodes. This provides the advantage of easy bug 
fixes (the end user can install a new version of their nodes and everything is fixed) as well as easy
access to improvements and support for a wide range of middleware implementations. The downside of 
this is that the game was not necessarily tested under those different implementations and those 
implementations might not support all required capabilities. For example, if the user selection 
process requires support for a "wave" pose but the specific implementation has no support for the
"wave" pose then the game might behave differently from expected.<br>
To mitigate this two options are available to the developer:
- <b>Use queries inside the NIQuery script</b>. The basic idea here is to set the vendor and a minimum 
  and maximum version corresponding to the supplied versions in the installer. For example, lets assume
  we use NITE then the vendor name would be PrimeSense and the minimum version might be 1.5.1.2 while
  the maximum version would be 1.5.X.X.
- <b>Use an XML file</b>. The same queries can be added in an XML file and that file used. An example
  file is supplied under the OpenNI/documentation directory.


<H1>Loading levels</H1>
When loading a level in Unity, all game objects are destroyed and new ones are created. This means
that when loading a level, the OpenNI engine will close everything and reopen them. Typically this does
not pose any problem. However, it does mean that user detection starts from scratch. This may mean that 
the player will be required to calibrate again and that the player's mapping to skeletons might change.<br>

A possible solution for this is to use DontDestroyOnLoad. From the Unity script reference:<br>
"When loading a new level all objects in the scene are destroyed, then the objects in the new level are
loaded. In order to preserve an object during level loading call DontDestroyOnLoad on it. If the 
object is a component or game object then its entire transform hierarchy will not be destroyed either." 
<br>
This should be handled with care as ALL relevant game objects must not be destroyed. If some are 
destroyed and some are not, unexpected results may occur. The developer, therefore, should make sure
that any objects no longer needed are destroyed in an orderly fashion and that all important objects
are saved. For example, in order to save calibration data, it is enough to save the OpenNISettings 
prefab (although if the objects were created separately, all of them must be saved). If you wishe to
save the user to player mapping, then the object containing the NIPlayerManager should also 
be preserved and so on.<br>

Another important thing to note is that static classes are not destroyed. This means that NIGUI
is not recreated and if the load was handled in the middle of a BeginGroup block, the ResetGroups 
method should be called on it.

<H1>Positioning the Sensor</H1>
Correct positioning of the sensor is important for good performance. The following are general tips
for positioning the sensor correctly. These tips should probably be provided to the user as well. <br>
- The sensor should be positioned parallel to the floor on a flat surface. It should also be parallel to
  the wall.
- Position the sensor on a stable surface so it cannot move @note Speakers are not stable because of 
  their vibrations so you should not position the sensor near speakers.
- Position the sensor near the end of the surface (otherwise the surface may obstruct the view).
- Position the sensor so that it points to the center of the body mass (not too low, not too high).
- Make sure the area in front of the sensor (where you will stand) is free of obstructions.

@note The positioning has a substantial effect on performance. An example of these effects can be seen
in @subpage PinningToTheFloor

@anchor AddingNewSkeletonModels
<H1>Adding New Skeleton Models</H1>
When creating a model to be controlled by the skeleton, it is important to rig it correctly.
The best way for the skeleton to control the model is to control its joints. Therefore, the rigging
should be done so that the controlled joints control the movement. A good example for this is the 
soldier model in the sample. <br>
The following joints are best used for control:
- Torso: This is a must as the controller uses the center of the torso as a positioning tool
- Head: The head generally turns only with the body but is important for calculations to see if
  the skeleton is erect or if the user is bowing to any side (a commonly used gesture).
- Hands (left and right): These are generally used for tracking and as the arm's end point.
- Elbows (left and right): Used to control the lower arm's direction
- Shoulders (left and right): Used to control the upper arm's direction
- Hips (left and right): Used to control the upper leg's direction
- Knees (left and right): Used to control the lower leg's direction
- Feet (left and right): Used for tracking and are the leg's end point.

@note The model itself must be in a T pose (standing straight with the arms extended to both sides). Any 
change in orientation from this will cause the movement to be skewed. 

@see @ref OpenNICreatingASkeletonModelPrefabTutorial for more information.

@anchor OpenNIRecordAndPlayback
<H1>Recording and playing back sensor data</H1> 
<H2>Overview</H2> 
While most of the time developers want to work with the sensor directly, it is very useful for debugging purposes to
be able to record and load the recording data. This is useful for two things:
-# It makes it much easier to reproduce problems
-# It allows the developer to work without a sensor (for example when on the move or when working with
   other people).

In general, OpenNI provides the capability to record and load. In order to simplify it as much as possible, 
following are simple instructions on how to use the recording.
@note the current version of OpenNI records only basic nodes (depth, image). This means that any 
implementation which uses these nodes as the basis for everything is supported. For example,
the NITE implementation (used in the installer) creates the user and skeleton data from the depth and
therefore works.

<H2>Creating a recording</H2> 
OpenNI provides API for recording. However, creating a program which uses this API inside Unity would
require writing code, which unfortunately, didn't make it to this version.<br>
For developers who installed the "dev" version of OpenNI (see @ref OpenNIBeforeYouStart) there are two 
ways to create a new recording:
-# <b>Write the code</b>: Writing the code to record is not very complicated. The samples and 
   docummentation of OpenNI show how to do it.
-# <b>Use the samples</b>: 
    - <b>niBackRecorder</b> is a command line tool, which stores frames in memory in a 
   cyclic buffer. Clicking "D" sends a request to dump this cyclic buffer to an ONI file.
   In effect, it saves the last certain number of seconds, according to how it has been configured.
   This means that a developer can run the recorder in the background, run the Unity program and then save the
   result. 
    - <b>NIViewer</b> Using the NIViewer sample. For this do the following
        - Run the NIViewer sample (this is by default in "C:\Program Files\OpenNI\Samples\Bin\Release" 
          on a 32 bit machine and in "C:\Program Files (x86)\OpenNI\Samples\Bin\Release" on a 
          64 bit machine).
        - Press 'f' to toggle between full screen view and windowed view. 
        - Press 'd' and choose a filename for the recording. 
        - The recording will start in 5 seconds (to avoid the wait use 's' instead of 'd').
        - Move quickly to the Unity game and start it. 
        - After the 5 seconds have passed, play the game and record everything you wish.
        - when you are ready to finish, return to the sample and press x.
        - You now have a recording of everything you did.
        - @note you can always use '?' in the sample to get help.
    - <b>Other samples</b> are available for recording such as NiRecordRaw.
<br><br>
 
<H2>Playing a recording</H2> 
Now that we have a recording, we want to play it. There are two ways to do this:
- The OpenNI Settings manager has an option <b>Playerback filename</b> which can hold the filename 
  for a recording. If this is used then the recording will be loaded for all nodes recorded in it.
  @note this does @b NOT work properly if an XML file is used! 
- Use an xml file. For simplicity, a sample XML file is provided (ExampleRecord.xml in the 
  documentation directory). <br> The main change which needs to be done in the XML file is to change
  the filename which holds the recorded data (if one uses extensions then more production nodes 
  might be required).<br>To use the xml file, In the OpenNI configuration object of the 
  @ref OpenNISettingsPrefab (or directly from the @ref OpenNISettingsManager script) the XML file
  name needs to be set. 

<H2>Playback limitations</H2> 
Currently, while playing a recording, some limitations exist:
- Playback only works with depth and image nodes. Any implementation which does not use depth or image
  nodes as the basis of its skeleton (and all other nodes) will not work. 
  The default implementation provided in the installer uses depth and therefore works.
- Mirroring is recieved from the recording. This means that if the recording has a different mirroring
  than the mirroring defined in the OpenNISettingsManager then the one from the recording will be used.
- There might be small differences when running from sensor than from running from recording. This is due
  to various implementation issues of the recording and because of different performance loads. These 
  generally manifest in different timing issues.
*/

/** 
@page PinningToTheFloor Skeletons and Floors.

When you look at the skeleton samples it seems that at times, the skeleton floats in the air, and
at other times the skeleton's feet are in the ground. This changes when moving around.<br>
<br>
There are several reasons for this:
<H1>Sensor vs. Game Coordinate Systems</H1>
The coordinates received from the skeleton are in the sensor's coordinate system. The sensor's
origin is in the sensor itself and its axes are aligned with the sensor. Unfortunately, the
coordinate system for the game is different.<br>

<H2>Different Origin</H2>
The game coordinate system's origin is in a different position than the origin of the sensor. This means
that everything moves relative to it. Specifically, the floor can be above or below the floor used by
the game (which can be at any position).<br>

It is the responsibility of the developer to make corrections for this. For example, if the confidence of the 
feet is high when starting to track and a specific pose (such as calibration pose) is made which forces
the user to stand on the ground, one can assume that the floor is just below the feet (This
is known as pinning to the ground). Various techniques can be used to make this work well. 
For example, if jumping is intended, then one cannot pin to the ground all the time.

<H2>Different Rotation</H2>

Generally speaking, the sensor's coordinate system can have any rotation compared to the Unity game
coordinate system. In practice, assuming the sensor was positioned properly, we can assume the 
following:
- The sensor is parallel to the floor. Therefore, the rotation around the "z" axis (coming out of the 
  sensor) can be assumed to be minimal.
- The sensor can be rotated along the "y" axis (up). However, in practice this does not have a lot of
  effect on the game (everything just moves to the side). Furthermore, this can again be assumed to be
  a relatively small rotation, as we assume the users position their sensor correctly.
- The sensor can be rotated along the "x" axis (right). This means it looks upwards or downwards from
  the current position. This rotation has a big effect on floors, as the floor is no longer leveled
  on a constant "y" coordinate.

One way to find out this rotation is to force the user to stand erect sometime in the games beginning
(for example striking a calibration pose or any other game-specific gesture or pose in which the user 
stands erect). This means that the vector between the torso and the head is parallel to the floor's 
normal. <br>
Another option is to estimate if the user is standing erect by comparing the vector between the torso
and the head to the vector created by each leg (feet, knee and hip) and make sure all are parallel.
If so there is a good chance the user stands erect and these vectors are parallel to the floor normal.

@note The z axis of the sensor is always inverted to the z axis of the game.

In order to convert from sensor position (Point3D) to game position (Vector3), the static class
@ref NIConvertCoordinates is provided. 

@note While this class provides various utilities (to convert and to handle the floor normal), it is
@b NOT to be considered a fully functional correction. It is more of a place holder for the developer
to create conversions that make sense in their specific game.

<H2>Getting too Close to or too Far from the sensor</H2>

The skeleton calculations have internal heuristics to make sure the skeleton is well behaved. This 
means that if the user gets too close or too far from the sensor, the skeleton's position may 
be incorrect. Therefore, it is important to make sure that the user is always within the view of the sensor.
This is one of the most important issues in sensor positioning.
 
<H2>The Home is not the Game</H2>
This is more of a design consideration for the game developer, but it is important to notice that 
the developer must make sure the movements of the user, which are in a very limited area on a leveled
floor, make sense in the game world. If the user is required to move a lot, or the game has uneven 
terrain, it may be better to create the movement by using gestures rather than by tracking the skeleton. 
*/
