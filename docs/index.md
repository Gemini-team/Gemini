# Overview

Welcome to the official Gemini Simulator website and documentation
Gemini is a Unity-based visual simulator originally developed by graduate students at NTNU in Trondheim, Norway. The project began as a simulator for the Milliamperé Autonomous ferry. The purpose behind Gemini is to provide a foundation for EMR (Electromagnetic Radiation) based sensors, such as optical cameras, LiDAR and Radar for use in development and testing of autonomous applications. In addition to providing the simulated sensor data from inside the simulation environment, Gemini will expose an API that allows developers to interface and communicate with the simulated vessel(s) and environment. At the moment there does not exist a single executable for Unity and it is only provided as a Unity project. One of the primary goals in the near future is to have a simple release of an executable ready, with all the minimum functionality for operating and interfacing with a simple autonomous vessel.


# Installation \(Windows 10\)

For the time being, Gemini can only be run on Windows 10.

Running Unity requires support of DirectX10 and a dedicated GPU is recommended.

## 1. Install Unity Hub

Download Unity Hub from this [link](https://unity3d.com/get-unity/download). Choose the option for only Unity Hub.

Run 'UnityHubSetup.exe' and follow the instructions given. 

## 2. Create a Unity account \(optional\)

Students can receive free access to the full version of Unity by creating an account and applying for a Student Developer pack. This is not necessary to run the simulator.

Go to the [Unity Store](https://store.unity.com/#plans-individual) and select 'Student'.

Students are required to provide authorization. Select 'Get access', and follow the three steps given. If you do not already have a Github account, you will be required to create one.

If your Github account is created with an email adress other than your student address, add your student address to your Github user: Settings &gt; Emails &gt; Add email address. This will substantially speed up the Student Developer Pack application.

## 3. Install Git

Skip this step if you have already installed Git. If you are unsure whether or not it is installed, open the command prompt \(Windows key + 'Command Prompt'\) and run:

```text
git version
```

Download Git for Windows from this [link](https://gitforwindows.org/). 

Run the installer file and follow the instructions given. All of the configurable options can remain as default.

After installing,  run the 'Git CMD' application once to initialize it.

## 4. Clone the repository

Open the command prompt \(Windows key + 'Command Prompt'\) and enter the following:

```text
git clone https://github.com/Gemini-team/Gemini.git
```

## 5. Open the project in Unity Hub and install Unity

Run Unity Hub. Select 'Add' in the top right corner.

Navigate to the cloned repository. It should be located at C:\Users\your\_username\autoferry-gemini\ Select the 'Autoferry' folder.

Once the project is added, double click on the warning triangle. A pop-up message should appear informing you that the editor version is missing. Click 'Install' in the bottom right corner. 

Optionally select or deselect Microsoft Visual Studio Community 2019. It is not necessary to run the simulator, but it is useful if no other programming IDE's are installed.

## 6. Install gRPC dependencies

Download the zipped folder 'grpc\_unity\_package.2.26.0-dev.zip' at this [link](https://packages.grpc.io/archive/2019/12/a02d6b9be81cbadb60eed88b3b44498ba27bcba9-edd81ac6-e3d1-461a-a263-2b06ae913c3f/index.xml) \(first option under the C\# header\). Once downloaded,  extract or copy the 'Plugins' folder into the autoferry-gemini\Autoferry\Assets\Networking folder. The path should end up being ...Assets\Networking\Plugins.

## 7. Download Unity packages

For the time being, the necessary Unity packages are kept in a private cloud drive for licensing reasons. Contact robin.stokke@njordchallenge.com to request access.

When you have received access, download 'GeminiModels.unitypackage' and 'Trondheim.unitypackage'.

## 8. Source file generation dependencies \(optional\)

#### C\#

Generating source files for the services in Unity requires a Protobuf and gRPC compiler for C\#, downloadable from this [link](https://packages.grpc.io/archive/2020/06/039e7759c5202f7cfb808d4d55d4cde531b951c5-d225705a-89df-4405-a33f-df7d0073a69d/index.xml). Under 'gRPC protoc Plugins', choose either grpc-protoc\_windows\_x64 or x86, depending on the running operating system \(64 or 32 bit\).

#### Python

Generating source files for clients requires a Protobuf and gRPC compiler for Python. If Python is not already installed, download the most recent stable Windows executable installer \(again depending on the OS running, 32 or 64 bit\) and follow the installation instructions. Afterwards, open the command prompt and enter the following commands:

```text
py -m pip install grpcio
py -m pip install grpcio-tools
```

## 9. Run Unity and import packages

In Unity Hub, double click the Autoferry project to open it. This will take a moment, particularly the first time.

Once Unity boots, add the packages by selecting Assets &gt; Import package &gt; Custom package in the top header. Find the Unity packages downloaded in the previous step, import one of them, and repeat for the second package. This process also takes a while.

Gemini is now installed.



