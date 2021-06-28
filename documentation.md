# Chicken documentation

## Repo overview

The repo top level contains four directories: Chicken, ChickenMockup, docs and Text. These are explained below.
- Chicken: a Unity project directory for the real version of the chicken game.
- docs: a directory containing the WebGL build of the chicken game, enabling the game to be hosted on github pages for the KTH Github Enterprise.
- Text: a directory containing miscellaneous text files and other files.

Development happens in the Chicken directory, and builds are placed in the docs directory for online hosting. Note that this only works on the master branch.

## Chicken Unity project

The Chicken Unity project is moderately small, but requires some explanation. It runs on at least Unity version 2019.4.22f1, so any later version in the 2019.4 (LTS) family should work.

### General flow

The general flow of the project is as follows. The participant accepts the HIT on Amazon mechanical turk and clicks the so-called survey link. This takes them to the website where the game is hosted.

The game starts in the IntroScene, where participants read instructions and give consent to partake. Participants also select their avatar's gender expression and skin color. The main script governing this scene is the IntroManager.

Then, the game goes onto the trial scene. Participants are told what type of trial it is on the loading screen, then the participant gets to control when the trial starts, whereafter the participant faces a robot and can decide to swerve. A close-up of the robot is displayed on the screen. After the participant reaches the goal, the trial is over. The scene is then reloaded with new trial settings. There are three blocks of trials, one for each of the motivations of the participants (and therefore one for each payoff matrix). The order of the blocks is randomized, and the order of the trials in each block is randomized. There is an equal number of trials of each robot motivation type in each block. The main script governing this scene is the TrialManager.

Between each block of trials, the instruction scene is loaded. Depending on the motivation of the upcoming block, the instructions are different. The current score and bonus are also displayed here. The user is only required to press the continue button, but is encouraged to read the instructions. The main script governing this scene is the InstructionManager.

After all the trials are complete, the participant is taken to the ending scene. Here, the participant enters their demographic information for the experiment in three steps, is allowed to enter additional comments and report technical issues and is then given the mturk completion code in a dialog box. The participant returns to the mturk page and enters the completion code to complete the HIT.

### Project file structure

All the different directories in the project file structure is explained here. The directories are:
- BrokenVector
- Fonts
- JsonDotNet
- Materials
- MesoGames
- Models
- Plugins
- Prefabs
- RenderTextures
- Scenes
- Scripts
- Textures
- WebGLTemplates

#### BrokenVector

Directory containing all the files associated with the imported LowPolyTreePack from the Unity asset store.

#### Fonts

Directory containing fonts for use in the project. Only the bahnschrift font and the Arial font are used in the project. Arial is imported into Unity by default, while the bahnschrift font was taken from the Windows system fonts and imported into Untiy.

#### JsonDotNet

Directory created when importing the JsonDotNet package from the Unity asset store. Contains everything included in that package, meaning all that is needed in order to use the package in our own scripts.

#### Materials

Directory containing subdirectories for materials for different uses.

The EnvironmentMaterials directory contains materials that are applied to objects that are part of the environment.

The PepperMaterials directory contains materials for different colors that the pepper model can have. The materials are modified versions of materials from the pepper model.

The PlayerMaterials directory contains materials for different skin colors for player avatars. The materials are modified versions of materials from the player avatar package.

#### MesoGames

Directory created when importing the customizable player models. Contains everything that the models need, with animations, materials and everything. This directory should not need to be accessed now after the player avatar prefabs and materials have been set up.

#### Models

Directory containing imported .fbx files.

#### Plugins

Directory containing a single .jslib file that contains the functions to interact with the website where the game is hosted. Contains a single function for displaying a string (the mturk completion code) in a dialog box.

The directory is required by Unity, and has a specific structure that Unity recognizes and can work with. See [the documentation](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html) for more information.

#### Prefabs

Directory containing prefab files that are derived from game objects. Some are abstract objects containing scripts but no rendering components, others are environment models created from Unity primitives and some are created outside of Unity, imported into Unity and modified with transforms, scripts and materials to make a prefab.

Prefabs are used as premade objects. They can be imported into a scene directly, or used by a script and loaded at runtime.

#### RenderTextures

Directory containing render textures. Should contain the render texture for the robot camera.

#### Scenes

Directory containing all of the five scenes of the game.

#### Scripts

Directory containing all of the C# scripts of the game. An overview of each script is given below, and each script contains code comments.

- Appearance: Defines a data structure to hold all the data about a player avatar's appearance.
- AssetLoader: Contains functions for using the AssetSelector script's functions for loading player avatar assets, robot model assets and environment model assets into the scene at runtime. Also applies materials for different colors to player avatars and robot models.
- AssetSelector: Takes a lot of materials and prefabs that are drag-and-dropped into it in the inspector and provides functions that make an interface for the prefabs. Mostly provides access to prefabs through enumerators.
- CameraController: Simple script for ensuring that the player camera is in the correct position in every frame during a trial.
- Comments: Defines a data structure to hold information about comments that the participant enters.
- EndingManager: Driving script for the ending scene. Defines behavior for the buttons that require scripting in the scene, and validates input where required. Contacts the ExperimentDataManager script to send participant data and to initiate the sending of experiment data.
- ExperimentDataManager: Acts as a repository for all the experiment data that is acquired during the game. Contains a list of Trials, an Appearance, a Participant, a Comments and a string for the mturk completion code. Calls the EndingManager to display the dialog box. Defines a function for sending the experiment data to the mongoDB database server. Also defines the data structure that contains all the experiment data cleanly, in a way that is compatible with JSON serialization using JsonDotNet.
- Goal: Defines the behavior of the goal contained in the game, dealing with what happens when the player character enters the goal trigger.
- GroundConstructor: Defines the behavior of the ground constructor, an object that executes each time the trial scene is loaded in order to construct the ground plane with a reasonable resolution.
- HelperFunctions: Contains static helper functions that can be accessed from anywhere but do not make sense to be put in any particular script.
- InstructionManager: Driving script for the instruction scene. Defines behavior for the continue button and for showing the correct instructions depending on the motivation of the next block of trials.
- IntroManager: Driving script for the intro scene. Defines behavior for the buttons that require scripting in the scene, and validates input where required. Contacts the ExperimentDataManager script to send appearance data.
- OverlayManager: Defines the behavior of the loading screens during the trial scene. Manages the onLoadIsFinished delegate by setting the flags in the SharedVariableManager. Also defines the behavior of the post-trial score panel.
- Participant: Defines a data structure to hold all the data about a participant.
- PlayerController: Takes player input and moves the player avatar. Also defines when the game is over. Relies on the onLoadIsFinished delegate.
- PointMatrixWriter: Writes the points into a point matrix (payoff matrix) wherever there is one in a scene. This is required in order to fill in the points dynamically depending on the current motivation.
- RobotCameraController: Identical to CameraController, but used on the robot's camera.
- RobotController: Moves the robot agent, decides when to swerve. Relies on the onLoadIsFinished delegate.
- ScoreManager: Holds properties for the current score and the previous score, in order to facilitate drawing the score to the screen and calculating the score difference from one trial to the next. Also holds the bonus amount as calculated from the score and bonus rate, which is set manually in this class.
- ScoreTextWriter: Defines behavior of GameObjects with a text component that should contain the score and the bonus. It takes the score and bonus variables from the ScoreManager and formats it into the text field of the Text component of the GameObject at {0} and {1} respectively.
- SharedVariableManager: Holds variables, including flags, that multiple classes need access to and that do not make sense being part of any one of the accessing classes. Defines the UnityAction delegates onGameIsOver and onLoadIsFinished that synchronizes the behavior of other scripts in the trial scene.
- TrajectoryTracker: A script attached to any game object that should have its trajectory tracked over a period of time. At given time intervals it samples the position and the delta-position of the game object in the x-z-plane and stores them into a two-dimensional float array that can be accessed from outside the class.
- Trial: Defines a data structure to hold information about a trial.
- TrialManager: Driving script for the trial scene. Contains the definition of the trials that will be run during the trial scene loop. Calls AssetLoader to load assets into the trial scene. Relies on the onGameIsOver delegate to know when to collect the trial data and then reload the scene but with the next trial. Contacts the ExperimentDataManager to deliver the trial data after all the trials are complete. Handles cleanup of game objects that are not needed after the trial scene.

#### WebGLTemplates

Directory containing subdirectories for WebGL templates for different uses. A WebGL template is the containing html document that loads the game into the browser when running the WebGL build of the game. The templates are as follows:

The BetterTemplate directory contains the template that is primarily used when building the game. The template was created by Github user greggman int [this repo](https://github.com/greggman/better-unity-webgl-template). It defines a way of making the WebGL container take up the entire browser window, rather than being contained in a static container. Also contains code to send browser data to the unity instance.

The CleanWithAddedJS directory contains a template similar to the default template, but without the footer in the container that is usually there. Also contains code to send browser data to the unity instance.

The directory is required by Unity, and has a specific structure that Unity recognizes and can work with. See [the documentation](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html) for more information.

### Building WebGL and publishing

In order to build the game, simply go into the build settings in Unity, ensure the platform is WebGL, click Build and select any location. Publish the build by replacing the contents of the docs directory by the contents of the directory created during the building process, then adding the files in git, commit and push.

## Experiment data TODO CHANGE THIS SECTION

Experiment data is all the data that is collected during the game, and used for the purposes of the experiment. The ExperimentDataManager script manages all of the data, by collecting the pieces of data from different places in the game. The script also constructs a form and sends it to a Google Forms form.

The different categories of data are as follows:
- Amazon mechanical turk completion code: Random string of numbers and capital letters created once a participant has finished the trials and entered the participant data.
- Player referred by Amazon mechanical turk: Indicates in the experiment data whether the player was referred to the game page by Amazon mechanical turk.
- Browser data: Collected by a short javascript script in the index.html file in the docs directory and sent into the Unity instance using a SetInterval to ensure that the data is collected correctly. Consists of the name of the browser used by the participant, as well as device width, height, pixel ratio and depth and color depth. When send it is formatted as a string with each piece of data separated by a string token of the form "SPLIT". ExperimentDataManager collects the string and splits into the separate pieces of data.
- Appearance data: The player avatar's appearance data, consisting of the skin color and gender expression of the avatar. Entered by the participant in the intro scene.
- Participant data: Demographic information about the participant, entered by the participant in the ending scene.
- Comments: Additional comments and comments about technical issues entered by the player in the ending scene.
- Trial data: All the data collected during the trial scene. At the end of each trial, that trial's data is collected by the TrialManager script. At the end of the trial scene, the TrialManager sends the complete trial data to the ExperimentDataManager.

The Google Forms form is constructed beforehand, with one free-text question with no input validation created for each piece of data to be collected. After the form is created, it is previewed by clicking the preview button in the Google Forms interface. Viewing the page source and searching for "form action" gives the url that Unity should send its form data to using the POST method.

The field names in the form are entered into Unity. By viewing the page source of the previewed form in the inspector and searching for "entry." gives a long list of entry names. Each question in the form has one name associated with it on the form "entry.XXXXXXX" where the Xs is some string of numbers. For question number i in the form, the ith entry name is the one associated with the question. In Unity, the form data is constructed in the same order, in order to correspond to the Google Forms form.

The entry names can be extracted and formatted appropriately for a C# array by copying and pasting the all of the HTML input elements into Atom, opening the Find and Replace panel, pasting `<input type="hidden" name=("entry\.\d*") value="">` into the Find field and `$1, ` (with the trailing space) into the Replace field then pressing Replace All. The result can then be pasted into the entryNames array in the ExperimentDataManager.

## Amazon mechanical turk

The game is connected to Amazon mechanical turk through a HIT. The HIT is created from the "survey link" template provided by Amazon. The HIT simply provides a hyperlink for workers to click on, that leads to the website where the game is hosted. In order to complete the HIT, the worker must enter a survey completion code. This code is in turn generated in the game and provided to the worker at the very end of the game in a dialog box that the worker can copy the code from and paste it into the HIT.

To validate that the worker has truthfully completed the HIT and not made up some survey completion code, cross-validate that the completion code entered in mturk (available in the HIT results) is present in the collected data.

If the game website was referred by mturk, this will be reflected in the "Referred by mturk" field in the form. It is checked by the index.html file sending the site referrer to the Unity instance, and the ExperimentDataManager script making sure the referrer contains the string "mturk". This is not foolproof in any way, but it is sufficient for the purposes of the experiment, while also staying general enough to withstand potential changes that Amazon does to the mturk service.
