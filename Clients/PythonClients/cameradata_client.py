from __future__ import print_function
import grpc

from cameradata import cameradata_pb2
from cameradata import cameradata_pb2_grpc

#from imu import imu_pb2
#from imu import imu_pb2_grpc

import matplotlib.pyplot as plt
import matplotlib.image as mpimg
import os
import io
import PIL.Image as image
from array import array
import datetime
import time



def run():
    with grpc.insecure_channel('localhost:50070') as channel:

        # camera service
        
        channel = grpc.insecure_channel('localhost:50070')
        stub = cameradata_pb2_grpc.CameradataServiceStub(channel)
        #while True:
        for imgChunk in stub.StreamImagedata(cameradata_pb2.CameradataRequest(operation="streaming")):
            # This is hardcoded, should be dynamic in the future.
            # TODO - The client should agree with the server which resolution the image
            # is in.
            img = image.frombytes("RGB",(1058, 545), imgChunk.imagedata, 'raw')
            img.save("test.bmp")

        #forces = [1.0, 2.0, 3.0]
        #torque = [4.0, 5.0, 6.0]

        #channel = grpc.insecure_channel('localhost:50060')
        #stub = imu_pb2_grpc.IMUServiceStub(channel)
        #while True:
            #imuData = stub.TransferIMUData(imu_pb2.IMURequest(forces=forces, torque=torque))
            #print("imuData: ", imuData)
            #time.sleep(.16)
            

if __name__ == '__main__': run()
