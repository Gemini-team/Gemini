using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotecontrolData
{
    int value = 0;

    public RemotecontrolData()
    {

    }

    public int GetValue()
    {
        return value;
    }

    public void SetValue(int value)
    {
        this.value = value;
    }

}
