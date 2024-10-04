﻿using GOLD;

namespace Isagri.Reporting.StimulSoftMigration.Report.Common.GoldEngine;

public static class GoldEngineExtensions
{
    /// <summary>
    /// Faire le passe-plat de la première valeur de la réduction
    /// </summary>
    /// <param name="reduction"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static object PassOn(this Reduction reduction, int index = 0)
    {
        return reduction[index].Data;
    }
}
