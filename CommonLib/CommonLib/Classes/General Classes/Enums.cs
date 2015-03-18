using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public class Enums
    {
    }

    public enum TypeOfPlot
    {
        NAV,
        MTM,
        DRAW_DOWN,
        RETURN_DISTRIBUTION,
        MONTHWISE_RETURN,
        CURRENT_LONG_SHORT_RATIO,        
        HIST_LONG_SHORT_RATIO,
        GROSS_EXPOSURE,
        BASKET_SPREAD,
        SPREAD,
        VOLSPREAD,
        TEARB,
        YEARWISE_RETURN,
        STOCK_PARAMETERS,
        SECTOR_DIVISION,
        RECENT_SECTOR_EXPOSURE,
        RECENT_LONG_SHORT_RATIO,
        NUM_STOCKS
    };

    public enum InputFileType
    {
        D_Px_B_A_S,
        D_Px_S,
        M_D_Px_B_A_S,
        D_GExp_GTV_MTM,
        DB
    };

    public enum DistanceType
    {
        Eucledian,
        Correlation
    };

    public enum SpreadType
    {
        CashFut,
        Rolls,
        CashFutNextMonth,
        IndexArb,
        OptionSkew
    };

    public enum TimeInterval
    {
        Sec_01 = 1,
        Sec_05 = 5,
        Sec_10 = 10,
        Sec_30 = 30,
        Min_01 = 60,
        Min_05 = 300,
        Min_15 = 900,        
        Min_30 = 1800,
        Min_60 = 3600
    };

    public enum LongShortType
    {
        SHORT,
        LONG
    }

    public enum ComparisonType
    {
        X,
        Y,
        MAG
    }

    
}
