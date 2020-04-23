
from __future__ import print_function
import grpc

from remotecontrol import remotecontrol_pb2
from remotecontrol import remotecontrol_pb2_grpc
import time

def run():
    #channel = grpc.insecure_channel('localhost:50060')
    channel = grpc.insecure_channel('192.168.1.183:50060')
    stub = remotecontrol_pb2_grpc.RemoteControlStub(channel)

    fwdForce = remotecontrol_pb2.GeneralizedForce(
        x = 5000.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    leftForce = remotecontrol_pb2.GeneralizedForce(
        x = -5000.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )


    bckwdForce = remotecontrol_pb2.GeneralizedForce(
        x = -5000.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    rightForce = remotecontrol_pb2.GeneralizedForce(
        x = 0.0, 
        y = 5000.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    upForce = remotecontrol_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = -5000.0,
        k = 0.0,
        m = 0.0,
        n = 0.0

    )

    downForce = remotecontrol_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 5000.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    leftTorque = remotecontrol_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = -5000.0,
        m = 0.0,
        n = 0.0
    )

    rightTorque = remotecontrol_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 5000.0,
        m = 0.0,
        n = 0.0
    )

    upTorque = remotecontrol_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = -5000.0,
        n = 0.0
    )

    downTorque = remotecontrol_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 5000.0,
        n = 0.0
    )

    posNTorque = remotecontrol_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 5000.0
    )

    negNTorque = remotecontrol_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = -5000.0
    )

    noForce = remotecontrol_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )
    
    command = ""
    while True:
        command = input()
        if command == "w":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = fwdForce))
        elif command == "s":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = bckwdForce))
        elif command == "d":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = rightForce))
        elif command == "a":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = leftForce))
        elif command == "r":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = upForce))
        elif command == "f":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = downForce))
        elif command == "j":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = leftTorque))
        elif command == "l":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = rightTorque))
        elif command == "i":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = upTorque))
        elif command == "k":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = downTorque))
        elif command == "y":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = posNTorque))
        elif command == "h":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = negNTorque))
        elif command == " ":
            success = stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = noForce))
            
        print("succes", success)





if __name__ == '__main__':
    run()