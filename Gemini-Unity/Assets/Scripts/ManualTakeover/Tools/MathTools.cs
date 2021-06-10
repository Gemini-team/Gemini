using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathTools {
    public static int Mod(int x, int m) {
        return (x % m + m) % m;
    }

    public class RollingAverage {
        public float Average { get; private set; }
        private int count;
        private Queue<float> values = new Queue<float>();

        public RollingAverage(int count, float initialAverage) {
            this.count = count;
            Average = initialAverage;
        }

        public void PushValue(float newValue) {
            values.Enqueue(newValue);
            if (values.Count > count) {
                values.Dequeue();

                float newAverage = 0;
                foreach (float value in values) {
                    newAverage += value;
                }
                newAverage /= count;

                Average = newAverage;
            }
        }
    }
}
