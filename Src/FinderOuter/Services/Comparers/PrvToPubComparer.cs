﻿// The FinderOuter
// Copyright (c) 2020 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Cryptography.Asymmetric.EllipticCurve;
using Autarkysoft.Bitcoin.Cryptography.Asymmetric.KeyPairs;
using Autarkysoft.Bitcoin.Encoders;
using FinderOuter.Backend.ECC;
using System;

namespace FinderOuter.Services.Comparers
{
    /// <summary>
    /// Converts private key to an <see cref="EllipticCurvePoint"/> and compares it with the pubkey (point).
    /// </summary>
    public class PrvToPubComparer : ICompareService
    {
        private byte[] pubBa;

        public bool Init(string pubHex)
        {
            return Base16.TryDecode(pubHex, out pubBa) && PublicKey.TryRead(pubBa, out _);
        }

        public ICompareService Clone()
        {
            return new PrvToPubComparer()
            {
                pubBa = this.pubBa
            };
        }

        protected readonly Calc _calc = new();
        public Calc Calc => _calc;
        public unsafe bool Compare(uint* hPt)
        {
            Scalar key = new(hPt, out int overflow);
            if (overflow != 0)
            {
                return false;
            }

            Span<byte> actual = _calc.GetPubkey(key, pubBa.Length == 33);
            return actual.SequenceEqual(pubBa);
        }

        public unsafe bool Compare(ulong* hPt)
        {
            Scalar key = new(hPt, out int overflow);
            if (overflow != 0)
            {
                return false;
            }

            Span<byte> actual = _calc.GetPubkey(key, pubBa.Length == 33);
            return actual.SequenceEqual(pubBa);
        }

        public bool Compare(in PointJacobian point)
        {
            ReadOnlySpan<byte> actual = point.ToPoint().ToByteArray(pubBa.Length == 33);
            return actual.SequenceEqual(pubBa);
        }


        public bool Compare(byte[] key)
        {
            Scalar sc = new(key, out int overflow);
            if (overflow != 0)
            {
                return false;
            }

            Span<byte> actual = _calc.GetPubkey(sc, pubBa.Length == 33);
            return actual.SequenceEqual(pubBa);
        }

        public bool Compare(Scalar key) => Compare(Calc.MultiplyByG(key));
    }
}
