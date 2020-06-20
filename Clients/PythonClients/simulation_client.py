

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
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = fwdForce, stepSize = 0.1))
        elif command == "s":
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = bckwdForce, stepSize = 0.1))
        elif command == "d":
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = rightForce, stepSize = 0.1))
        elif command == "a":
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = leftForce, stepSize = 0.1))
        elif command == "r":
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = upForce, stepSize = 0.1))
        elif command == "f":
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = downForce, stepSize = 0.1))
        elif command == "j":
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = leftTorque, stepSize = 0.1))
        elif command == "l":
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = rightTorque, stepSize = 0.1))
        elif command == "i":
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = upTorque, stepSize = 0.1))
        elif command == "k":
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = downTorque, stepSize = 0.1))
        elif command == "y":
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = posNTorque, stepSize = 0.1))
        elif command == "h":
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = negNTorque, stepSize = 0.1))
        elif command == " ":
            result = stub.DoStep(simulation_pb2.StepRequest(vesselId = "Ferry", force = noForce, stepSize = 0.1))

if __name__ == '__main__':
    run()