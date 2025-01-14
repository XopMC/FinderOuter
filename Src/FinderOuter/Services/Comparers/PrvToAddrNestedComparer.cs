﻿// The FinderOuter
// Copyright (c) 2020 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin;
using FinderOuter.Backend.Hashing;
using FinderOuter.Backend.ECC;
using System;

namespace FinderOuter.Services.Comparers
{
    public class PrvToAddrNestedComparer : PrvToAddrBase
    {
        public override bool Init(string address)
        {
            AddressService serv = new();
            return serv.CheckAndGetHash_P2sh(address, out hash);
        }

        public override ICompareService Clone()
        {
            return new PrvToAddrNestedComparer()
            {
                hash = this.hash.CloneByteArray()
            };
        }

        public override unsafe bool Compare(uint* hPt)
        {
            Scalar key = new(hPt, out int overflow);
            if (overflow != 0)
            {
                return false;
            }

            Span<byte> toHash = _calc.GetPubkey(in key, true);

            ReadOnlySpan<byte> actual = Hash160.Compress33_P2sh(toHash);
            return actual.SequenceEqual(hash);
        }

        public override unsafe bool Compare(ulong* hPt)
        {
            Scalar key = new(hPt, out int overflow);
            if (overflow != 0)
            {
                return false;
            }

            Span<byte> toHash = _calc.GetPubkey(in key, true);

            ReadOnlySpan<byte> actual = Hash160.Compress33_P2sh(toHash);
            return actual.SequenceEqual(hash);
        }

        public override bool Compare(in PointJacobian point)
        {
            Span<byte> toHash = point.ToPoint().ToByteArray(true);
            ReadOnlySpan<byte> compHash = Hash160.Compress33_P2sh(toHash);
            return compHash.SequenceEqual(hash);
        }
    }
}
