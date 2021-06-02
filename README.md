# ImPres - An Immersive Presentation System

## System Functionalities
The Impres Systems allows users to create and present 3D presentations.
Those presentations can contain 2D content (presented on the so-called "canvas") and 3D content.
There exist two types of 3D content.
One can be manipulated by all partitioners of the presentation (elements in the so-called "handout").
And the other one that can not be manipulated during the live presentation (elements in the so-called "scene").
Augmented Reality (AR) is used to present the 3D elements of the presentation.

## System Architecture
The System consists of 3 parts.

This repository contains the 2D Editor for the presentations.

The 3D editor part can be found in [this repository](https://github.com/rwth-acis/Immersive-presentation---3D-editor).

The backend that connects all parts can be found in [this repository](https://github.com/rwth-acis\Immersive-presentation---Backend-Coordinator).

## Getting started

First, the backend should be up and running at a specified `<backend-addr>`.
Then the two editors (the (2D editor](https://github.com/rwth-acis/Immersive-presentation---2D-editor) and the [3D editor](https://github.com/rwth-acis/Immersive-presentation---3D-editor) can be initialized.)
As all architecture parts are needed, the getting started section of all architecture parts, each covers the same complete setup process for all parts.

### Prerequisites
A MySQL Database which can be accessed by the backend
- MySQL database that uses the schema defined in the `<databasesetup.sql>` file in the [backend repository](https://github.com/rwth-acis\Immersive-presentation---Backend-Coordinator).
- User with the necessary login credentials that can be used by the backend to connect to the database.

For the backend
- [Node.JS](https://nodejs.org/en/) installed (tested with version 14.15.4)
- A proxy pass from the `<backend-addr>` to a free port (here called `<backend-port>`)
- A folder where the presentation files can be stored with a `<presfolder-path>`(the user that will execute the Node.js application needs read and write access to this folder)

For the 2D Editor
- Windows (Version higher or equal to XP - tested with Windows 10)
- Microsoft Visual Studio installed (Version 2010 or higher - tested with version 2019)
- .NET 4.x installed
- The Visual Studio WPF workload installed

For the 3D Editor
- Recommended [Unity version](https://unity3d.com/de/get-unity/download/archive): 2019.4.6f1
- [Microsoft Mixed Reality Toolkit v2.4.0](https://github.com/microsoft/MixedRealityToolkit-Unity/releases/tag/v2.4.0) (already included in the project)
- [Photon PUN 2](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922) (download through Unity's asset store window in the editor)
- Visual Studio (tested with VS 2019)
- For HoloLens Development:
  - Windows 10 development machine
  - Windows 10 SDK ([10.0.18362.0](https://developer.microsoft.com/de-de/windows/downloads/windows-10-sdk))
- For Android Development:
  - [ARCore SDK](https://github.com/google-ar/arcore-unity-sdk/releases) (tested with ARCore SDK for Unity v1.12.0)
  - Android SDK 7.0 (API Level 24) or later

### Backend

- Clone the [backend repository](https://github.com/rwth-acis/Immersive-presentation---Backend-Coordinator) to a server that fulfills all prerequisites.
- Create a .env file in the project folder and set the following Environment variables:

Environment Variable | Value or description
-------------------- | --------------------
PRODUCTION | if in production environment 1 else 0
PORT | `<backend-port>`
MYSQL_HOST | address where to reach the MySQL database
MYSQL_USER | name of the MySQL user
MYSQL_PASSWORD | password of the MySQL user
MYSQL_DATABASE | name of the MySQL database
JWT_SECRET | a secret string used to generate the JWT
SALTROUNDS | rounds of encryption (e.g. 9)
PRES_DIR | `<presfolder-path>`
OIDCCLIENTID | [Learning Layers](http://results.learning-layers.eu/) client id for [OIDC](https://openid.net/connect/)
OIDCCLIENTSECRET | [Learning Layers](http://results.learning-layers.eu/) client secret for [OIDC](https://openid.net/connect/)

- execute the `<coordinator.js>` script
- optional: use a process manager to keep the API running (tested with [pm2](https://pm2.keymetrics.io/))

### 2D Editor

- Clone the [2D repository](https://github.com/rwth-acis/Immersive-presentation---2D-editor) to a computer fulfilling the prerequisites for the 2D editor.
- Set the corresponding values for the [Learning Layers](http://results.learning-layers.eu/) client id and secret for [OIDC](https://openid.net/connect/) in the solution properties.
- When you want to use your own `<backend-addr>` change the constant `<BACKENDADDR>` in the `<CoordinatorConnectorLibrary>` accordingly.

#### Troubleshooting 2D Editor

In the case that some references are missing try to use the NuGet packet manager integrated in Visual Studio 2019 to reinstall Newtonsoft.Json, IdentityModel.OidcClient, and RestSharp.

### 3D Editor

- Clone the [3D repository](https://github.com/rwth-acis/Immersive-presentation---3D-editor) on a machine that fulfills the prerequisites for the 3D editor and the targeted build system (HoloLens or Android)
- Start Unity and open the clones repository
- Download "PUN 2 - FREE" by Exit Games from the Asset Store (Ctrl + 9).
- Select all the PUN 2 assets in the window that appears once the download is finished
- A window will appear asking for an appId
- Create a free Photon Engine account on the [Photon Website](https://www.photonengine.com/).
- Create a PHOTON REALTIME application in the Photon Console (do not create a PHOTON PUN application)
- Enter the appId of the created realtime application in the window that appeared in Unity
- If the window did not appear is closed already, go to Window > Photon Unity Networking > Highlight Server Settings, and there you can enter the app ID in the inspector under "Settings/ App Id Realtime".
- Open the "WelcomeScene". Unity will ask to import "TMP Essentials". Click on the upper button to install it.
- Whenever you want to test the system start the application with the "StartScene" selected because this scene handles the initialization of the system and no other scene will work correctly without this scene being active
- Information on how to build the project can be found in the [ARCore Guide](https://developers.google.com/ar/develop/unity/quickstart-android).