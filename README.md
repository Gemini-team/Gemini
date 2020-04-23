**This is the HDRP version of the simulator, this is the version that will be maintained, the old repository has been renamed Autoferry Gemini Legacy
and will not be maintained.**


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
These can be corrected by running the generate.sh script under the Protobuf/ folder. 
To be able to compile the .proto files into protobuf and grpc source files the Protobuf and grpc compilers have to be downloaded.
To download these compilers choose the latest package under the **Build ID** column  from this URL: https://packages.grpc.io/
and download the gRPC protoc Plugins package that fits your system e.g Windows x64 for 64-bit Windows OS. 
When the package is finished downloading, unpack the protoc executable and grpc_csharp_plugin into
the Protobuf/Plugins/ folder. This folder is ignored by Git and will not be pushed up to the repository when pushing changes.

The generate.sh script takes in 2 arguments, where the first one is requires. The first argument has to be the name of the .proto file which are placed in
a folder with the same name in the folder Protobuf/ProtoFiles/. The other argument can be the name of the programming language which the client
script should be compiled to. At this moment only Python clients are supported by the generate.sh script.
An example of compiling the protobuf and grpc files for the remotecontrol service is **./generate.sh remotecontrol python**.


The convention of having the same name on folder which contains the .proto file as the .proto file itself has to be upheld.

When the generate.sh script has been run it will place the compiled protobuf and grpc src files in the correct directory in the Unity simulation folder
which is located at Autoferr/Assets/Networking/ProtobufFiles/. The client src files will be put in the Clients/ folder, and depending on which language
the protobuf and grpc source files are compiled to it will be placed in the corresponding sub folder. At the moment only Clients/PythonClients are available.

