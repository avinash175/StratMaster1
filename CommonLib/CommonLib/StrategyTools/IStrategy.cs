using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace CommonLib
{
    public interface IStrategy
    {
        void RunStrategy(StrategyData data);
        List<object[]> ComputeStatistics(StrategyData data);
        void SaveStatistics(string fileName, string fMTM, StrategyData data, bool IsDOD);
    }
}
