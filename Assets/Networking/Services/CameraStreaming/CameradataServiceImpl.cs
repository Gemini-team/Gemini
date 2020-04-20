using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading.Tasks;
using Cameradata;
using Grpc.Core;

public class CameradataServiceImpl : Cameradata.CameradataService.CameradataServiceBase
{
    CameradataImpl cameradata;
    CameradataResponse cameradataResponse;

    public CameradataServiceImpl()
    {
        cameradata = new CameradataImpl(ByteString.CopyFromUtf8(""), 0);
    }

    public override async Task StreamImagedata(
        CameradataRequest request,
        IServerStreamWriter<CameradataResponse> responseStream,
        ServerCallContext context
        )
    {

        cameradataResponse = new CameradataResponse
        {
            Imagedata = cameradata.GetImagedata(),
            ImagedataLength = cameradata.GetImagedataLength()
        };

        await responseStream.WriteAsync(cameradataResponse);
            
            
    }
    public void SetCameradata(CameradataImpl cameradata)
    {
        this.cameradata = cameradata;
    }


}
