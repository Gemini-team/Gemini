Autoferry Gemini is a realtime 3D simulation of the **Autoferry** project residing at **NTNU Trondheim**.

The initial goal of this project is to develop a simulation of the autonomous passenger ferry as possible.
We will try to achieve this in numerous ways, where our main focus at the moment are realistic implementations
of sensors and an API that allows us to control the simulation from environments outside of the simulator.

This project includes large amounts of assets files such as textures, 3D models etc. In the legacy version of this
project we were using Git LFS (Large File Storage), but we ran into issues with this. Now our solution is to have all
large assets in a specific folder which we are ignoring in git, and we rather package the contents of the folder into
a untiypackage and store it in the cloud using onedrive.

**Getting started**