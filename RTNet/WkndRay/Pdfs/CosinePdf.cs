// -----------------------------------------------------------------------
// <copyright file="CosinePdf.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;

namespace WkndRay.Pdfs
{
    public class CosinePdf : IPdf
    {
        public CosinePdf(Vector3 w)
        {
            Uvw = OrthoNormalBase.FromW(w);
        }

        public OrthoNormalBase Uvw { get; }

        public float GetValue(Vector3 direction)
        {
            float cosine = Vector3.Dot(direction.ToUnitVector(), Uvw.W);
            return cosine > 0.0f ? cosine / MathF.PI : 1.0f;  // todo: book has this as 1.0f, but that causes NaN due to div by zero
        }

        public Vector3 Generate()
        {
            return Uvw.Local(RandomService.GetRandomCosineDirection());
        }
    }
}
