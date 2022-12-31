using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WkndRay
{
    public class SafeRandom : Random
    {
        private const int PoolSize = 2048;

        private static readonly Lazy<RandomNumberGenerator> s_rng =
            new Lazy<RandomNumberGenerator>(() => new RNGCryptoServiceProvider());

        private static readonly Lazy<object> s_positionLock =
            new Lazy<object>(() => new object());

        private static readonly Lazy<byte[]> s_pool =
            new Lazy<byte[]>(() => GeneratePool(new byte[PoolSize]));

        private static int s_bufferPosition;

        public static int GetNext()
        {
            while (true)
            {
                var result = (int)(GetRandomUInt32() & int.MaxValue);

                if (result != int.MaxValue)
                {
                    return result;
                }
            }
        }

        public static int GetNext(int maxValue)
        {
            if (maxValue < 1)
            {
                throw new ArgumentException(
                    "Must be greater than zero.",
                    "maxValue");
            }
            return GetNext(0, maxValue);
        }

        public static int GetNext(int minValue, int maxValue)
        {
            const long Max = 1 + (long)uint.MaxValue;

            if (minValue >= maxValue)
            {
                throw new ArgumentException(
                    "minValue is greater than or equal to maxValue");
            }

            long diff = maxValue - minValue;
            var limit = Max - (Max % diff);

            while (true)
            {
                var rand = GetRandomUInt32();
                if (rand < limit)
                {
                    return (int)(minValue + (rand % diff));
                }
            }
        }

        public static void GetNextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (buffer.Length < PoolSize)
            {
                lock (s_positionLock.Value)
                {
                    if ((PoolSize - s_bufferPosition) < buffer.Length)
                    {
                        GeneratePool(s_pool.Value);
                    }

                    Buffer.BlockCopy(
                        s_pool.Value,
                        s_bufferPosition,
                        buffer,
                        0,
                        buffer.Length);
                    s_bufferPosition += buffer.Length;
                }
            }
            else
            {
                s_rng.Value.GetBytes(buffer);
            }
        }

        public static double GetNextDouble()
        {
            return GetRandomUInt32() / (1.0 + uint.MaxValue);
        }

        public override int Next()
        {
            return GetNext();
        }

        public override int Next(int maxValue)
        {
            return GetNext(0, maxValue);
        }

        public override int Next(int minValue, int maxValue)
        {
            return GetNext(minValue, maxValue);
        }

        public override void NextBytes(byte[] buffer)
        {
            GetNextBytes(buffer);
        }

        public override double NextDouble()
        {
            return GetNextDouble();
        }

        private static byte[] GeneratePool(byte[] buffer)
        {
            s_bufferPosition = 0;
            s_rng.Value.GetBytes(buffer);
            return buffer;
        }

        private static uint GetRandomUInt32()
        {
            uint result;
            lock (s_positionLock.Value)
            {
                if ((PoolSize - s_bufferPosition) < sizeof(uint))
                {
                    GeneratePool(s_pool.Value);
                }

                result = BitConverter.ToUInt32(
                    s_pool.Value,
                    s_bufferPosition);
                s_bufferPosition += sizeof(uint);
            }

            return result;
        }
    }
}
