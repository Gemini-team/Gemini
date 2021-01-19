# Gemini-API
API definitions for Gemini.

To generate source files for protobuf and gRPC using python, the following command can be used `python -m grpc_tools.protoc -I. --python_out=. --grpc_python_out=. sensor_streaming.proto`.
NOTE, this assumes that the command is run in the same directory as the `.proto` file that should be compiled are located. It will also place the generated source files in the same
directory.
