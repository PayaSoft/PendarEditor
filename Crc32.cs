namespace Paya.Cryptography
{
	using System;
	using System.Collections.Generic;
	using System.Security.Cryptography;

	/// <summary>
	///     Implements a 32-bit CRC hash algorithm compatible with Zip etc.
	/// </summary>
	/// <remarks>
	///     Crc32 should only be used for backward compatibility with older file formats
	///     and algorithms. It is not secure enough for new applications.
	///     If you need to call multiple times for the same data either use the HashAlgorithm
	///     interface or remember that the result of one Compute call needs to be ~ (XOR) before
	///     being passed in as the seed for the next Compute call.
	/// </remarks>
	public sealed class Crc32 : HashAlgorithm
	{
		#region Constants

		private const uint DefaultPolynomial = 0xedb88320u;

		private const uint DefaultSeed = 0xffffffffu;

		#endregion

		#region Static Fields

		private static uint[] _defaultTable;

		#endregion

		#region Fields

		private readonly uint _seed;

		private readonly uint[] _table;

		private uint _hash;

		#endregion

		#region Constructors and Destructors

		public Crc32()
			: this(DefaultPolynomial, DefaultSeed)
		{
		}

		public Crc32(long polynomial, long seed)
		{
			unchecked
			{
				this._table = InitializeTable((uint)polynomial);
				this._seed = this._hash = (uint)seed;
			}
		}

		#endregion

		#region Public Properties

		public override int HashSize
		{
			get { return sizeof (int)*8; }
		}

		#endregion

		#region Public Methods and Operators

		public static long Compute(byte[] buffer)
		{
			return Compute(DefaultSeed, buffer);
		}

		public static long Compute(long polynomial, long seed, IList<byte> buffer)
		{
			unchecked
			{
				return ~CalculateHash(InitializeTable((uint)polynomial), (uint)seed, buffer, 0, buffer.Count);
			}
		}

		public override void Initialize()
		{
			this._hash = this._seed;
		}

		#endregion

		#region Methods

		protected override void HashCore(byte[] buffer, int start, int length)
		{
			this._hash = CalculateHash(this._table, this._hash, buffer, start, length);
		}

		protected override byte[] HashFinal()
		{
			var hashBuffer = UInt32ToBigEndianBytes(~this._hash);
			this.HashValue = hashBuffer;
			return hashBuffer;
		}

		// ReSharper disable once SuggestBaseTypeForParameter
		private static uint CalculateHash(uint[] table, uint seed, IList<byte> buffer, int start, int size)
		{
			var crc = seed;
			for (var i = start; i < size - start; i++)
			{
				crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
			}
			return crc;
		}

		private static long Compute(long seed, IList<byte> buffer)
		{
			return Compute(DefaultPolynomial, seed, buffer);
		}

		private static uint[] InitializeTable(uint polynomial)
		{
			if (polynomial == DefaultPolynomial && _defaultTable != null)
			{
				return _defaultTable;
			}

			var createTable = new uint[256];
			for (var i = 0; i < 256; i++)
			{
				var entry = (uint)i;
				for (var j = 0; j < 8; j++)
				{
					if ((entry & 1) == 1)
					{
						entry = (entry >> 1) ^ polynomial;
					}
					else
					{
						entry = entry >> 1;
					}
				}
				createTable[i] = entry;
			}

			if (polynomial == DefaultPolynomial)
			{
				_defaultTable = createTable;
			}

			return createTable;
		}

		private static byte[] UInt32ToBigEndianBytes(uint uint32)
		{
			var result = BitConverter.GetBytes(uint32);

			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(result);
			}

			return result;
		}

		#endregion
	}
}