using System;
using System.Collections.Generic;

namespace LifeSim.Core.Domain.Careers
{
    [Serializable]
    public class CareerStep
    {
        public string titleKey;
        public int requiredIQ;
        public int requiredEQ;
        public int baseSalary;
    }

    [Serializable]
    public class CareerTrack
    {
        public string key;
        public string nameKey;
        public List<CareerStep> ladder = new List<CareerStep>();
    }
}