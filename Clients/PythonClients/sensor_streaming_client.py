
from __future__ import print_function

import os, sys, pygame
import pygame.camera
from pygame.locals import *


import grpc

from sensordata import sensordata_pb2
from sensordata import sensordata_pb2_grpc
from sensormanagement import sensormanagement_pb2
from sensormanagement import sensormanagement_pb2_grpc

from remotecontrol import remotecontrol_pb2
from remotecontrol import remotecontrol_pb2_grpc
from remotecontrol import forces 

#import PIL.Image as image

def load_picture(surface):
    #pic = pygame.image.load(os.path.join(os.path.dirname(__file__), "test.bmp"))
    #pic = pygmae.image.load()
    pygame.display.update()
    surface.blit(pic, [100, 100])


if __name__ == '__main__':

    pygame.init()
    surface = pygame.display.set_mode((1200, 800))

    canvas = pygame.Surface((800, 640))

    camera = pygame.Rect(0, 0, 800, 640)

    sub = canvas.subsurface(camera)

    pygame.display.set_caption("Streaming")

    # Sensormanagement
    # Port when in Editor
    sensormanagement_channel = grpc.insecure_channel('localhost:50085')

    # Port when in build
    #sensormanagement_channel = grpc.insecure_channel('localhost:50082')

    sensormanagement_stub = sensormanagement_pb2_grpc.SensorManagementStub(sensormanagement_channel)

    sensors = sensormanagement_stub.GetAllSensorsOfType(
        sensormanagement_pb2.AllSensorsOfTypeRequest(type=sensormanagement_pb2.OPTICAL))

    print(sensors)

    remotecontrol_channel = grpc.insecure_channel('192.168.1.183:50060')
    remotecontrol_stub = remotecontrol_pb2_grpc.RemoteControlStub(remotecontrol_channel)


    # Sensordata
    # Port when in Editor
    sensordata_channel = grpc.insecure_channel('localhost:50083')

    #Port when in build
    #sensordata_channel = grpc.insecure_channel('localhost:50080')

    sensordata_stub = sensordata_pb2_grpc.SensordataStub(sensordata_channel)

    optical = True
    running = True


    print(forces.fwdForce)

    while running:
        # Check for events
        events = pygame.event.get()
        for e in events:
            if e.type == QUIT or (e.type == KEYDOWN and e.key == K_ESCAPE):
                running = False     
            if e.type == KEYDOWN and e.key == K_c:
                if optical == True:
                    # Port when in editor
                    sensordata_channel = grpc.insecure_channel('localhost:50083')

                    # Port when in build
                    #sensordata_channel = grpc.insecure_channel('localhost:50080')

                    sensordata_stub = sensordata_pb2_grpc.SensordataStub(sensordata_channel)
                    optical = False
                else:
                    # Port when in Editor
                    sensordata_channel = grpc.insecure_channel('localhost:50084')

                    # Port when in build
                    #sensordata_channel = grpc.insecure_channel('localhost:50081')


                    sensordata_stub = sensordata_pb2_grpc.SensordataStub(sensordata_channel)
                    optical = True


            # TODO: Figure out why the client freezes when this is run, but the game server is fine.
            # Stopping sensor from rendering
            #if e.type == KEYDOWN and e.key == K_x:
                #success = sensormanagement_stub.StopRendering(sensormanagement_pb2.StopRenderingRequest(sensorID = 1))
            #if e.type == KEYDOWN and e.key == K_z:
                #success = sensormanagement_stub.StartRendering(sensormanagement_pb2.StopRenderingRequest(sensorID = 1))
        


            # Controlling vessel
            if e.type == KEYDOWN and e.key == K_w:
                success = remotecontrol_stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = forces.fwdForce))
            if e.type == KEYDOWN and e.key == K_a:
                success = remotecontrol_stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = forces.leftForce))
            if e.type == KEYDOWN and e.key == K_s:
                success = remotecontrol_stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = forces.bckwdForce))
            if e.type == KEYDOWN and e.key == K_d:
                success = remotecontrol_stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = forces.rightForce))
            if e.type == KEYDOWN and e.key == K_SPACE:
                success = remotecontrol_stub.ApplyForce(remotecontrol_pb2.ForceRequest(vesselId = "Ferry", generalizedForce = forces.noForce))


        # Recieving sensordata
        for dataChunk in sensordata_stub.StreamSensordata(sensordata_pb2.SensordataRequest(operation="streaming")):
            #print("data length:", dataChunk.dataLength)
            img = pygame.image.frombuffer(dataChunk.data, (800, 640), "RGB")


            #sub.blit(canvas, [100, 100])
            sub.blit(img, [100, 100])
            surface.blit(sub, (0, 0))
            surface.blit(pygame.transform.flip(sub, False, True), (0, 0))

        pygame.display.flip()
        pygame.display.update()
