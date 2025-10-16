using System;
using System.Collections.Generic;

namespace LifeSim.Core.Data.DTO
{
    [Serializable] public class CareerStepDTO { public string titleKey; public int requiredIQ; public int requiredEQ; public int baseSalary; }
    [Serializable] public class CareerTrackDTO { public string key; public string nameKey; public List<CareerStepDTO> ladder; }
    [Serializable] public class CareerTracksFile { public string version="1"; public List<CareerTrackDTO> items = new(); }
}