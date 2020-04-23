
from __future__ import print_function
import grpc

import time

from query import query_pb2
from query import query_pb2_grpc

def run():
    channel = grpc.insecure_channel('localhost:50061')
    stub = query_pb2_grpc.QueryServiceStub(channel)


#    while True:
        #success = stub.GetAllVessels(query_pb2.AllVesselsRequest(boatID = "boat"))
#        response = stub.GetAllVesselIds(query_pb2.AllVesselIdsRequest(type = "boat"))
        #print("success: ", success.value)
        #for id in response.ids:
            #print(id)
        #time.sleep(2)

    allIds_response = stub.GetAllVesselIds(query_pb2.AllVesselIdsRequest(type = "boat"))
    my_id = allIds_response.ids[0]
    print("allIds: ", allIds_response.ids)

    while True:
        my_bounds = stub.GetVesselBounds(query_pb2.VesselBoundsRequest(vesselId = my_id))
        print("my_bounds: ", my_bounds)

        #imu_response = stub.GetVesselIMU(query_pb2.IMURequest(vesselId = my_id))
        #print(imu_response)
        time.sleep(2)

#        imu_response = stub.GetVesselIMU(query_pb2.IMURequest(vesselId = my_id))
        #print(imu_response)

if __name__ == '__main__':
    run()