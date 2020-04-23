#!/bin/bash

# Generating the csharp protobuf and grpc files.
# Need to check wheter the directory already exists
OUTDIR=../Autoferry/Assets/Networking/ProtobufFiles/$1
SRC_DIR=ProtoFiles/
echo "Writing compiled protobuf and grpc files to: " $OUTDIR
if [ -d "$OUTDIR" ]; then
    # if the directory exists, just overwrite the files
    ./Plugins/protoc -I=$SRC_DIR --csharp_out=$OUTDIR/ $SRC_DIR/$1/$1.proto --grpc_out=$OUTDIR/ --plugin=protoc-gen-grpc=Plugins/grpc_csharp_plugin.exe
else 
    # if the directory does not exist, make a new directory and generate the files.
    echo "The output directory did not exist, creating the directory for you!"
    mkdir $OUTDIR
    echo "Writing compiled protobuf and grpc files to: " $OUTDIR
    ./Plugins/protoc -I=$SRC_DIR --csharp_out=$OUTDIR/ $SRC_DIR/$1/$1.proto --grpc_out=$OUTDIR/ --plugin=protoc-gen-grpc=Plugins/grpc_csharp_plugin.exe
fi


# Generating the python protobuf and grpc files.
# Here one must provide the name of the protobuf file.
#py -m grpc_tools.protoc -I protos --python_out=./python_client/ --grpc_python_out=./python_client/ protos/$1/$1.proto