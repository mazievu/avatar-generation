
using System;
using System.Collections.Generic;

namespace LifeSim.Core.Domain.Game
{
    [Serializable]
    public class BusinessInstance
    {
        public string instanceId; // Unique ID for this specific owned business
        public string businessId; // ID of the BusinessSO
        public int tier = 1;
        public List<string> employeeIds = new List<string>(); // Use null or empty string for empty slots
    }
}
