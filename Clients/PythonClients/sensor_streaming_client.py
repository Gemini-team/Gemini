
from __future__ import print_function

import os, sys, pygame
import pygame.camera
from pygame.locals import *


import grpc

from sensordata import sensordata_pb2
from sensordata import sensordata_pb2_grpc

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

    channel = grpc.insecure_channel('localhost:50083')
    stub = sensordata_pb2_grpc.SensordataStub(channel)

    optical = True
    running = True
    while running:
        # Check for events
        events = pygame.event.get()
        for e in events:
            if e.type == QUIT or (e.type == KEYDOWN and e.key == K_ESCAPE):
                running = False     
            if e.type == KEYDOWN and e.key == K_k:
                if optical == True:
                    channel = grpc.insecure_channel('localhost:50082')
                    stub = sensordata_pb2_grpc.SensordataStub(channel)
                    optical = False
                else:
                    channel = grpc.insecure_channel('localhost:50083')
                    stub = sensordata_pb2_grpc.SensordataStub(channel)
                    optical = True


        for dataChunk in stub.StreamSensordata(sensordata_pb2.SensordataRequest(operation="streaming")):
            #print("data length:", dataChunk.dataLength)
            img = pygame.image.frombuffer(dataChunk.data, (800, 640), "RGB")

            sub.blit(img, [100, 100])
            surface.blit(sub, (0, 0))
            surface.blit(pygame.transform.flip(sub, False, True), (0, 0))

        pygame.display.flip()
        pygame.display.update()
