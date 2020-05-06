from . import remotecontrol_pb2

fwdForce = remotecontrol_pb2.GeneralizedForce(
    x = 5000.0, 
    y = 0.0,
    z = 0.0,
    k = 0.0,
    m = 0.0,
    n = 0.0
)

leftForce = remotecontrol_pb2.GeneralizedForce(
    x = 0.0, 
    y = -5000.0,
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