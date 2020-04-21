using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class CameraServiceGen 
{
    static int id = 0;
    static int port = 50070;

    public static int GenID()
    {
        return id++;
    }

    public static int GenPort()
    {
        return port++;
    }
}

