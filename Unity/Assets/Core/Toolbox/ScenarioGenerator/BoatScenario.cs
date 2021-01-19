using UnityEngine;
using System;

namespace Gemini.EMRS.ScenarioGenerator
{
    public class BoatScenario
    {
        public Vector3[] position;
        public float[] heading;
        public double[] time;

        private GameObject _boatObject;
        private int _boatNumber;
        private string _scenarioPath;

        public BoatScenario(string csvScenarioPath, GameObject instantiatedBoatPrefab, int boatNumber)
        {
            _scenarioPath = csvScenarioPath;
            _boatNumber = boatNumber;
            _boatObject = instantiatedBoatPrefab;


            heading = vesselCSVHeadings();
            position = vesselCSVPositions();
            time = vesselCSVTimes();
        }

        private int ScenarioIndex = 0;
        public double UpdateVessel()
        {
            _boatObject.transform.position = position[ScenarioIndex];
            Quaternion QuaternionRot = Quaternion.AngleAxis(heading[ScenarioIndex], new Vector3(0, 1, 0));
            _boatObject.transform.rotation = QuaternionRot;

            // Incrementing or repeating the scenario
            if (ScenarioIndex < time.Length-1)
            {
                ScenarioIndex += 1;
            }
            else
            {
                ScenarioIndex = 0;
            }
            double nextScenarioTime = time[ScenarioIndex];

            return nextScenarioTime;
        }

        private float[] vesselCSVHeadings()
        {
            return ReadCSV.readColumnData<float>("V"+_boatNumber.ToString()+"Heading", _scenarioPath);
        }

        private Vector3[] vesselCSVPositions()
        {
            float[] xPos = ReadCSV.readColumnData<float>("V" + _boatNumber.ToString() + "x", _scenarioPath);
            float[] yPos = ReadCSV.readColumnData<float>("V" + _boatNumber.ToString() + "y", _scenarioPath);
            Vector3[] positions = new Vector3[xPos.Length];
            for(int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector3(yPos[i], 0, xPos[i]);
            }
            return positions;
        }

        private double[] vesselCSVTimesOld()
        {
            string[] timeAsStrings = ReadCSV.readColumnData<string>("TimeOfDay", _scenarioPath);
            double[] seconds = new double[timeAsStrings.Length];
            for(int i = 0; i < timeAsStrings.Length; i++)
            {
                seconds[i] = TimeSpan.Parse(timeAsStrings[i]).TotalSeconds;
            }
            return seconds;
        }
        private double[] vesselCSVTimes()
        {
            return ReadCSV.readColumnData<double>("TimeInSecondsPosix", _scenarioPath);
        }
    }
}