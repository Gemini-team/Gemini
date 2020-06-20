

from __future__ import print_function
import grpc

from simulation import simulation_pb2
from simulation import simulation_pb2_grpc
import time

def run():
    channel = grpc.insecure_channel('localhost:50081')
    stub = simulation_pb2_grpc.SimulationStub(channel)


    fwdForce = simulation_pb2.Force(
        x = 5000.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    leftForce = simulation_pb2.Force(
        x = 0.0, 
        y = -5000.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    bckwdForce = simulation_pb2.Force(
        x = -5000.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    rightForce = simulation_pb2.Force(
        x = 0.0, 
        y = 5000.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    upForce = simulation_pb2.Force(
        x = 0.0, 
        y = 0.0,
        z = -5000.0,
        k = 0.0,
        m = 0.0,
        n = 0.0

    )

    downForce = simulation_pb2.Force(
        x = 0.0, 
        y = 0.0,
        z = 5000.0,
        k = 0.0,
        m = 0.0,
        n = 0.0
    )

    leftTorque = simulation_pb2.Force(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = -5000.0,
        m = 0.0,
        n = 0.0
    )

    rightTorque = simulation_pb2.Force(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 5000.0,
        m = 0.0,
        n = 0.0
    )

    upTorque = simulation_pb2.Force(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = -5000.0,
        n = 0.0
    )

    downTorque = simulation_pb2.Force(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 5000.0,
        n = 0.0
    )

    posNTorque = simulation_pb2.Force(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = 5000.0
    )

    negNTorque = simulation_pb2.Force(
        x = 0.0, 
        y = 0.0,
        z = 0.0,
        k = 0.0,
        m = 0.0,
        n = -5000.0
    )

    noForce = simulation_pb2.Force(
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
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = fwdForce, stepSize = 10.0))
        elif command == "s":
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = bckwdForce, stepSize = 10.0))
        elif command == "d":
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = rightForce, stepSize = 10.0))
        elif command == "a":
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = leftForce, stepSize = 10.0))
        elif command == "r":
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = upForce, stepSize = 10.0))
        elif command == "f":
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = downForce, stepSize = 10.0))
        elif command == "j":
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = leftTorque, stepSize = 10.0))
        elif command == "l":
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = rightTorque, stepSize = 10.0))
        elif command == "i":
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = upTorque, stepSize = 10.0))
        elif command == "k":
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = downTorque, stepSize = 10.0))
        elif command == "y":
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = posNTorque, stepSize = 10.0))
        elif command == "h":
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = negNTorque, stepSize = 10.0))
        elif command == " ":
            success = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = noForce, stepSize = 10.0))

if __name__ == '__main__':
    run()