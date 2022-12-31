// -----------------------------------------------------------------------
// <copyright file="IPdf.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;

namespace WkndRay.Pdfs
{
    /// <summary>
    ///   Probability Distribution Function
    /// </summary>
    public interface IPdf
    {
        float GetValue(Vector3 direction);
        Vector3 Generate();
    }
}
