using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;

public class CameradataImpl
{
    ByteString imagedata;
    uint imagedataLength;

    public CameradataImpl(ByteString imageData, uint imageDataLength)
    {
        this.imagedata = imageData;
        this.imagedataLength = imageDataLength;
    }

    public ByteString GetImagedata()
    {
        return imagedata;
    }

    public uint GetImagedataLength()
    {
        return imagedataLength;
    }

    public void SetImagedata(ByteString imagedata)
    {
        this.imagedata = imagedata;
    }

    public void SetImagedataLength(uint imagedataLength)
    {
        this.imagedataLength = imagedataLength;
    }
}
