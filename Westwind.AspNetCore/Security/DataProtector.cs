using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Westwind.Utilities;

namespace Westwind.AspNetCore.Security
{

    /// <summary>
    /// This class uses the .NET Core Data Protection APIs to encrypt
    /// values. This is the same class used for encryption of authentication
    /// data, cookies etc. and this class provides an easy way to encode strings
    /// and return encoded strings that can be use for manual cookie processing.
    ///
    /// This class is a singleton so there can only be one unique instance per
    /// application.
    /// </summary>
    public class DataProtector
    {

        /// <summary>
        /// A unique identifier for this data 
        /// </summary>
        public static string UniqueIdentifier { get; set; } = "DataProtector_531";


        static IDataProtector _dataProtector;
        static object _protectorLock = new object();


        /// <summary>
        /// Internally called to create a Data Protector instance
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public static IDataProtector GetDataProtector(string uniqueIdentifier = null, bool force = false)
        {
            if (string.IsNullOrEmpty(uniqueIdentifier))
                uniqueIdentifier = UniqueIdentifier;

            if (force || _dataProtector == null)
            {
                lock (_protectorLock)
                {
                    if (_dataProtector == null)
                    {
                        IDataProtectionProvider dataProtectionProvider = DataProtectionProvider.Create(UniqueIdentifier);
                        _dataProtector = dataProtectionProvider.CreateProtector(UniqueIdentifier + "_312");
                    }
                }
            }

            return _dataProtector;
        }

        /// <summary>
        /// Encrypts a string and returns it as a binary BinHex or Base64 string
        /// </summary>
        /// <param name="dataToProtect"></param>
        /// <param name="stringEncodingMode"></param>
        /// <returns></returns>
        public static string Protect(string dataToProtect,
            DataProtectionStringEncodingModes stringEncodingMode = DataProtectionStringEncodingModes.Base64)
        {
            var dataProtector = GetDataProtector();
            byte[] protectedBytes = Encoding.UTF8.GetBytes(dataToProtect);
            byte[] plainBytes = dataProtector.Protect(protectedBytes);
            
            if (stringEncodingMode == DataProtectionStringEncodingModes.BinHex)
                return StringUtils.BinaryToBinHex(plainBytes);

            return Convert.ToBase64String(plainBytes);
        }


        /// <summary>
        /// Decrypts a previously protected strings from BinHex or Base64 binary data
        /// and returns the orignal string data.
        /// </summary>
        /// <param name="protectedStringData"></param>
        /// <param name="stringEncodingMode"></param>
        /// <returns></returns>
        public static string UnProtect(string protectedStringData,
            DataProtectionStringEncodingModes stringEncodingMode = DataProtectionStringEncodingModes.Base64)
        {
            var protector = GetDataProtector();

            byte[] protectedBytes;
            if (stringEncodingMode == DataProtectionStringEncodingModes.BinHex)
                protectedBytes = StringUtils.BinHexToBinary(protectedStringData);
            else
                protectedBytes = Convert.FromBase64String(protectedStringData);

            byte[] plainBytes = protector.Unprotect(protectedBytes);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }

    public enum DataProtectionStringEncodingModes
    {
        BinHex,
        Base64
    }
}
