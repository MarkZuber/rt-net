using System.Numerics;

namespace WkndRay.Pdfs
{
    public class HitablePdf : IPdf
    {
        public HitablePdf(IHitable hitable, Vector3 origin)
        {
            Hitable = hitable;
            Origin = origin;
        }

        public IHitable Hitable { get; }
        public Vector3 Origin { get; }

        public float GetValue(Vector3 direction)
        {
            return Hitable.GetPdfValue(Origin, direction);
        }

        public Vector3 Generate()
        {
            return Hitable.Random(Origin);
        }
    }
}
