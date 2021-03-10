#!/bin/bash

# NOTE, THIS IS ONLY SUPPOSED TO BE RUN
# FROM GEMINI-CORE IN ITS CURRENT STATE.

# Generating the csharp protobuf and grpc files.
# Need to check wheter the directory already exists
#OUTDIR=../Autoferry/Assets/Networking/ProtobufFiles/$1

# TODO: SHOULD BE CHANGED TO TAKE IN PATH AS ARG
OUTDIR=../../Gemini-Unity/Assets/Gemini/Scripts/Networking/ProtobufFiles/$1

# TODO: THIS DOES NOT WORK, SHOULD BE CHANGED TO TAKE IN PATH AS ARG.
PYTHON_OUTDIR=../Clients/PythonClients/

SRC_DIR=ProtoFiles/
if [ $# -eq 0 ]; then
    echo "Name of protobuf file not given"
    exit 1
fi

if [ -d "$OUTDIR" ]; then
    echo "Writing compiled protobuf and grpc files to: " $OUTDIR
    # if the directory exists, just overwrite the files
    ./Plugins/protoc -I=$SRC_DIR --csharp_out=$OUTDIR/ $SRC_DIR/$1/$1.proto --grpc_out=$OUTDIR/ --plugin=protoc-gen-grpc=Plugins/grpc_csharp_plugin.exe
else 
    # if the directory does not exist, make a new directory and generate the files.
    echo "The output directory did not exist, creating the directory for you!"
    mkdir $OUTDIR
    echo "Writing compiled protobuf and grpc files to: " $OUTDIR
    ./Plugins/protoc -I=$SRC_DIR --csharp_out=$OUTDIR/ $SRC_DIR/$1/$1.proto --grpc_out=$OUTDIR/ --plugin=protoc-gen-grpc=Plugins/grpc_csharp_plugin.exe
fi

# TODO: As with then new changes, generating source files for Python should not be done here

#Generating the python protobuf and grpc files.
#Here one must provide the name of the protobuf file.
#This is assumed that host OS is Windows
#Check if second argument is given
if [ $# -ge 2 ]; then
    if [ $2 == "python" ]; then

        if [ -d "$PYTHON_OUTDIR" ]; then
            echo "Writing compiled protobuf and grpc files to: " $PYTHON_OUTDIR
            py -m grpc_tools.protoc -I $SRC_DIR --python_out=$PYTHON_OUTDIR --grpc_python_out=$PYTHON_OUTDIR $SRC_DIR/$1/$1.proto
        else 
            echo "The python client output directory did not exist, creating the directory for you!"
            mkdir $PYTHON_OUTDIR
            py -m grpc_tools.protoc -I $SRC_DIR --python_out=$PYTHON_OUTDIR --grpc_python_out=$PYTHON_OUTDIR $SRC_DIR/$1/$1.proto
        fi
    fi
fi