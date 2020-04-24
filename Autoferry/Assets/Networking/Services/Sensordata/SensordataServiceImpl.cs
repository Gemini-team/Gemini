using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sensordata;
using Grpc.Core;
using System.Threading.Tasks;
using Google.Protobuf;

public class SensordataServiceImpl : Sensordata.Sensordata.SensordataBase 
{
    private ByteString _data = ByteString.CopyFromUtf8("");
    private int _dataLength = 0;

    private SensordataResponse sensordataResponse;

    public ByteString Data
    {
        get => _data;
        set => _data = value;
    }
    public int DataLength
    {
        get => _dataLength;
        set => _dataLength = value;
    }

    public SensordataServiceImpl(ByteString data, int dataLength)
    {
        this._data = data;
        this._dataLength = dataLength;
    }

    public override async Task StreamSensordata(
        SensordataRequest request,
        IServerStreamWriter<SensordataResponse> responseStream,
        ServerCallContext context
        )
    {

        sensordataResponse = new SensordataResponse
        {
            Data = _data,
            DataLength = _dataLength,
        };

        await responseStream.WriteAsync(sensordataResponse);
        
    }
    
}
