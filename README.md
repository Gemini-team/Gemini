Autoferry Gemini is a realtime 3D simulation of the **Autoferry** project residing at **NTNU Trondheim**.

The initial goal of this project is to develop a simulation of the autonomous passenger ferry as possible.
We will try to achieve this in numerous ways, where our main focus at the moment are realistic implementations
of sensors and an API that allows us to control the simulation from environments outside of the simulator.

This project includes large amounts of assets files such as textures, 3D models etc. In the legacy version of this
project we were using Git LFS (Large File Storage), but we ran into issues with this. Now our solution is to have all
large assets in a specific folder which we are ignoring in git, and we rather package the contents of the folder into
a untiypackage and store it in the cloud using onedrive.

**Getting started**

* Large assets

The large assets that are needed for the project are stored as a unitypackage in a OneDrive folder, when needed contact
one of the maintainers to get access to the unitypackage. After downloading this package import into the project.
When this is done you should have a folder named IgnoredAssets under the Assets/ folder. This is the place where you 
should put all your large assets such as textures 3D models, sound files etc. This is not an ideal solution, but
the only one that is viable as of right now.

* GRPC and Protobuf Plugins

For the API services to work gRPC and Protobuf plugins are needed. These can be downloaded from the 
following URL: https://packages.grpc.io/archive/2019/12/a02d6b9be81cbadb60eed88b3b44498ba27bcba9-edd81ac6-e3d1-461a-a263-2b06ae913c3f/index.xml

Download the zipped folder grpc_unity_package.2.26.0-dev.zip. Currently this is the latest version that we have found
to be working properly, this will probably change in the future.

When the download is finished, extract the contents into the folder Assets/Networking. Then the following Plugins folder path should be
Assets/Networking/Plugins. As with the IgnoredAssets folder this folder is also ignored by git because it is quite large.

* Compiling gRPC and Protobuf files for the API

When opening the project you might encounter errors telling you that you are missing certain gRPC or Protobuf source files.


