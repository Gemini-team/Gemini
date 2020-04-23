from __future__ import print_function

import os, sys, pygame
import pygame.camera
from pygame.locals import *


import grpc

from cameradata import cameradata_pb2
from cameradata import cameradata_pb2_grpc

#import PIL.Image as image

def load_picture(surface):
    #pic = pygame.image.load(os.path.join(os.path.dirname(__file__), "test.bmp"))
    #pic = pygmae.image.load()
    pygame.display.update()
    surface.blit(pic, [100, 100])


if __name__ == '__main__':


    #pygame.init()
    #surface = pygame.display.set_mode((1200, 900))

    #pygame.display.set_caption("Streaming")

    #load_picture(surface)



    #while True:

        #pygame.display.flip()



    pygame.init()
    #surface = pygame.display.set_mode((1700, 950))
    surface = pygame.display.set_mode((1200, 800))

    #canvas = pygame.Surface((1600, 900))
    canvas = pygame.Surface((1200, 800))

    camera1 = pygame.Rect(0, 0, 1024, 534)
    #camera1 = pygame.Rect(0, 0, 800, 450)
    #camera2 = pygame.Rect(800, 0, 800, 450)
    #camera3 = pygame.Rect(0, 450, 800, 450)
    #camera4 = pygame.Rect(800, 450, 800, 450)

    sub1 = canvas.subsurface(camera1)
    #sub2 = canvas.subsurface(camera2)
    #sub3 = canvas.subsurface(camera3)
    #sub4 = canvas.subsurface(camera4)

    pygame.display.set_caption("Streaming")

    #channel = grpc.insecure_channel('localhost:50070')
    #stub = cameradata_pb2_grpc.CameradataServiceStub(channel)

    channel1 = grpc.insecure_channel('localhost:50070')
    stub1 = cameradata_pb2_grpc.CameradataServiceStub(channel1)

    #channel2 = grpc.insecure_channel('localhost:50071')
    #stub2 = cameradata_pb2_grpc.CameradataServiceStub(channel2)

    #channel3 = grpc.insecure_channel('localhost:50072')
    #stub3 = cameradata_pb2_grpc.CameradataServiceStub(channel3)

    #channel4 = grpc.insecure_channel('localhost:50073')
    #stub4 = cameradata_pb2_grpc.CameradataServiceStub(channel4)

    #with grpc.insecure_channel('localhost:50070') as channel:
    running = True
    while running:
        # Check for events
        events = pygame.event.get()
        for e in events:
            if e.type == QUIT or (e.type == KEYDOWN and e.key == K_ESCAPE):
                running = False     

        for imgChunk in stub1.StreamImagedata(cameradata_pb2.CameradataRequest(operation="streaming")):
            #img = pygame.image.frombuffer(imgChunk.imagedata, (800, 450), "RGB")
            img = pygame.image.frombuffer(imgChunk.imagedata, (1024, 534), "RGB")
            img = pygame.transform.rotate(img, 180)

            sub1.blit(img, [100, 100])

            #sub2.blit(img, [100, 100])
            #sub3.blit(img, [100, 100])
            #sub4.blit(img, [100, 100])

            surface.blit(sub1, (0, 0))
            #surface.blit(sub2, (600, 0))
            #surface.blit(sub3, (0, 450))
            #surface.blit(sub4, (600, 450))

            #surface.blit(img, (0, 0), camera1)
            #surface.blit(img, (600, 0), camera2)
            #surface.blit(img, (0, 450), camera3)
            #surface.blit(img, (600, 450), camera4)

            #pygame.display.flip()
            #pygame.display.update()

        #for imgChunk in stub2.StreamImagedata(cameradata_pb2.CameradataRequest(operation="streaming")):
            #img = pygame.image.frombuffer(imgChunk.imagedata, (800, 450), "RGB")
            #img = pygame.transform.rotate(img, 180)

            #sub2.blit(img, [100, 100])

            #surface.blit(sub2, (800, 0))

            #pygame.display.flip()
            #pygame.display.update()

        #for imgChunk in stub3.StreamImagedata(cameradata_pb2.CameradataRequest(operation="streaming")):
            #img = pygame.image.frombuffer(imgChunk.imagedata, (800, 450), "RGB")
            #img = pygame.transform.rotate(img, 180)

            #sub3.blit(img, [100, 100])

            #surface.blit(sub3, (0, 450))

            #pygame.display.flip()
            #pygame.display.update()

        #for imgChunk in stub4.StreamImagedata(cameradata_pb2.CameradataRequest(operation="streaming")):
            #img = pygame.image.frombuffer(imgChunk.imagedata, (800, 450), "RGB")
            #img = pygame.transform.rotate(img, 180)

            #sub4.blit(img, [100, 100])

            #surface.blit(sub4, (800, 450))

        pygame.display.flip()
        pygame.display.update()
