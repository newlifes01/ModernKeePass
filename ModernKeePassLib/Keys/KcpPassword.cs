/*
  KeePass Password Safe - The Open-Source Password Manager
  Copyright (C) 2003-2014 Dominik Reichl <dominik.reichl@t-online.de>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography.Core;
#if PCL
using Windows.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

using ModernKeePassLib.Security;
using ModernKeePassLib.Utility;

namespace ModernKeePassLib.Keys
{
	/// <summary>
	/// Master password / passphrase as provided by the user.
	/// </summary>
	public sealed class KcpPassword : IUserKey
	{
		private ProtectedString m_psPassword;
		private ProtectedBinary m_pbKeyData;

		/// <summary>
		/// Get the password as protected string.
		/// </summary>
		public ProtectedString Password
		{
			get { return m_psPassword; }
		}

		/// <summary>
		/// Get key data. Querying this property is fast (it returns a
		/// reference to a cached <c>ProtectedBinary</c> object).
		/// If no key data is available, <c>null</c> is returned.
		/// </summary>
		public ProtectedBinary KeyData
		{
			get { return m_pbKeyData; }
		}

		public KcpPassword(byte[] pbPasswordUtf8)
		{
			SetKey(pbPasswordUtf8);
		}

		public KcpPassword(string strPassword)
		{
			SetKey(StrUtil.Utf8.GetBytes(strPassword));
		}

		private void SetKey(byte[] pbPasswordUtf8)
		{
			Debug.Assert(pbPasswordUtf8 != null);
			if(pbPasswordUtf8 == null) throw new ArgumentNullException("pbPasswordUtf8");

#if PCL
			var sha256 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
			var pbRaw = sha256.HashData(pbPasswordUtf8.AsBuffer());
#else
			SHA256Managed sha256 = new SHA256Managed();
			byte[] pbRaw = sha256.ComputeHash(pbPasswordUtf8);
#endif

			m_psPassword = new ProtectedString(true, pbPasswordUtf8);
			m_pbKeyData = new ProtectedBinary(true, pbRaw.ToArray());
		}

		// public void Clear()
		// {
		//	m_psPassword = null;
		//	m_pbKeyData = null;
		// }
	}
}
