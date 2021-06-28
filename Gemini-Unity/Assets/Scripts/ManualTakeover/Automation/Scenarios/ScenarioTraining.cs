using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioTraining : Scenario {
    public override string ScenarioName => "Training";

    /// Should never be called during training
    public override string FailureWarning => throw new System.NotImplementedException();
}
