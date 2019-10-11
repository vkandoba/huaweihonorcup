namespace Borderer
{
    public class ImageParameters
    {
        public const int __totalSize = 512;

        public int P { get; }
        public int M { get; }
        public int K { get; }

        public ImageParameters(int p)
        {
            P = p;
            M = __totalSize / p;
            K = 2 * M * (M - 1);
        }

        protected bool Equals(ImageParameters other)
        {
            return P == other.P;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ImageParameters) obj);
        }

        public override int GetHashCode()
        {
            return P;
        }
    }
}