# Unity Setup

## What is Unity

Unity is a popular cross-platform game engine and the preferred platform for creating immersive VR experiences. The Innoactive Creator is built on top of Unity. The synergy from the both products lets you to create VR training applications fast. Unity provides technical foundation, and our tool focuses on creation of training courses.

## How to Set up Unity

### Unity Hub

There are several Unity versions and ways to download and install them. We recommend to use the Unity Hub. It is a standalone application that streamlines the process of finding, downloading, and managing your Unity projects and installations.  

Download the Unity Hub from [unity3d.com](https://unity3d.com/get-unity/download), install, and launch it.Upon the first launch, the Unity Hub will ask you to activate your Unity license. You need to log in into your Unity account. If you do not have one, you can sign up there for free.

Learn more about the Unity Hub in the Unity Technologies's [Getting Started](https://docs.unity3d.com/Manual/GettingStartedUnityHub.html) guide.

### Download Unity Using the Unity Hub

As we just installed the Unity Hub, the `Projects` and `Installs` sections should be empty. Go to `Installs`, then click `Add`, and select `Unity 2019.3`. Continue with the install wizard using the default configuration.

![Unity Hub Installs](../images/unity-setup/unity-hub-installs-panel.png "Unity Hub - Installs")

The `Installs` section shows the Unity versions currently installed and their status while they're being downloaded. After installation you can modify platform modules, show the installation directory or uninstall Unity version.

![Add Unity Version](../images/unity-setup/choose-unity-version.png "Add Unity Version")

If you already had an installation of Unity installed, you can link it to the Unity Hub. Go to `Installs`, then click at `Locate`, navigate inside the Unity Editor installation folder, and select the `Unity.exe` file.

### Create a Project Using the Unity Hub

![Unity Hub Projects](../images/unity-setup/unity-hub-projects-panel.png "Unity Hub - Projects")

Once you have installed the Unity Editor, you create a new project or load an existing one.

To create a new project, go to the `Projects` tab and then click at the `New` button. A configuration window will pop up. Select 3D from the list of templates, pick a good name for your project, and set the location for it. Click `Create`, and Unity will create a new project and open it in the Editor.

![Unity Project Settings](../images/unity-setup/setup-unity.project.png "New Unity project configuration")

If you have installed multiple Unity versions, click the triangle next to the `New` button in the `Projects` tab. There you can select which Unity version to use with the new project.

If you want to load an existing project, go to the `Projects` tab and click the `Add` button. Select the containing folder of that project. Unity will add the project to the `Projects` list.

Every time you create or load a project with the Unity Hub, it will add the project to the `Projects` list. From now on, just click on a project to open it with the Unity Editor.

![Unity Hub Project List](../images/unity-setup/unity-hub-list-of-projects.png "Projects list")

## API Compatibility Level

The Innoactive Creator requires `.Net API compatibility level` to be set to `.NET 4.X`. By default, Unity sets it to `.NET 2.0 Standard`. You will see the following error in the Unity Editor console:

![.Net API compatibility level error](../images/unity-setup/net-api-level-error.png "An error message about the incompatible .Net API level")

You can fix it through the `Player Settings` panel. In the Unity Editor, select `Edit` > `Project Settings` > `Player Settings` option in the menubar at top. The `Player Settings` panel contains up to 6 different sections. Find `Other Settings` at the bottom. Inside that section, look up for the `Api Compatibility Level*` dropdown switch. Change it to `.NET 4.X`.

![Player Settings Panel](../images/unity-setup/player-settings-other-api-level.png "Api Compatibility Level - .Net 4.x")

You can learn more about the `Player Settings` panel in the Unity Editor's [documentation](https://docs.unity3d.com/Manual/class-PlayerSettings.html).