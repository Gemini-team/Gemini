
from __future__ import print_function
import grpc

from force import force_pb2
from force import force_pb2_grpc
import time

def run():
    channel = grpc.insecure_channel('localhost:50087')
    stub = force_pb2_grpc.ForceStub(channel)


    fwdForce = force_pb2.GeneralizedForce(
        x = 5000.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    leftForce = force_pb2.GeneralizedForce(
        x = 0.0, 
        y = -5000.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    bckwdForce = force_pb2.GeneralizedForce(
        x = -5000.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    rightForce = force_pb2.GeneralizedForce(
        x = 0.0, 
        y = 5000.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    upForce = force_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = -5000.0,
        k = 0.0,
        m = 0.0,
        n = 0.0

    )

    downForce = force_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 5000.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    leftTorque = force_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = -5000.0,
        m = 0.0,
        n = 0.0
    )

    rightTorque = force_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 5000.0,
        m = 0.0,
        n = 0.0
    )

    upTorque = force_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = -5000.0,
        n = 0.0
    )

    downTorque = force_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 5000.0,
        n = 0.0
    )

    posNTorque = force_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 5000.0
    )

    negNTorque = force_pb2.GeneralizedForce(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = -5000.0
    )

    noForce = force_pb2.GeneralizedForce(
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
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = fwdForce))
        elif command == "s":
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = bckwdForce))
        elif command == "d":
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = rightForce))
        elif command == "a":
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = leftForce))
        elif command == "r":
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = upForce))
        elif command == "f":
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = downForce))
        elif command == "j":
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = leftTorque))
        elif command == "l":
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = rightTorque))
        elif command == "i":
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = upTorque))
        elif command == "k":
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = downTorque))
        elif command == "y":
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = posNTorque))
        elif command == "h":
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = negNTorque))
        elif command == " ":
            success = stub.ApplyForce(force_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = noForce))

if __name__ == '__main__':
    run()