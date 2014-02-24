using System;
using System.Runtime.InteropServices;

using BOOL = System.Boolean;
using DWORD = System.UInt32;
using LPWSTR = System.String;
using NET_API_STATUS = System.UInt32;

namespace xDev.Utils.Net
{
    public class UncAccess : IDisposable
    {
        #region [ Fields ]

        private bool _disposed;
        private string _uncPath;
        private string _domain;
        private string _user;
        private string _password;
        private int _lastError;

        #endregion


        #region [ Constructors ]

        /// <summary>
        /// Basic constructor which initializes connection parameters.
        /// </summary>
        /// <param name="uncPath">Fully qualified domain name UNC path.</param>
        /// <param name="domain">Domain of User.</param>
        /// <param name="user">A user with sufficient rights to access the path.</param>
        /// <param name="password">Password of User.</param>
        public UncAccess(string uncPath, string domain, string user, string password)
        {
            this._disposed = false;
            this._uncPath = uncPath;
            this._domain = domain;
            this._user = user;
            this._password = password;
            this._lastError = 0;

            Connect();
        }

        #endregion


        #region [ Destructor ]

        /// <summary>
        /// Class destructor.
        /// </summary>
        ~UncAccess()
        {
            Dispose();
        }

        #endregion


        #region [ Properties ]

        /// <summary>
        /// Gets the the system error code.
        /// </summary>
        public int LastError
        {
            get 
            { 
                return this._lastError; 
            }
        }

        #endregion


        #region [ Public Methods ]
        
        /// <summary>
        /// Disposing of class resources.
        /// </summary>
        public void Dispose()
        {
            if (!this._disposed)
            {
                Close();
            }
            this._disposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion


        #region [ Private Methods ]

        /// <summary>
        /// Connects to a UNC path using the credentials supplied.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if it succeeds.
        /// </returns>
        private bool Connect()
        {
            try
            {
                USE_INFO_2 useinfo = new USE_INFO_2();

                useinfo.ui2_remote = this._uncPath;
                useinfo.ui2_username = this._user;
                useinfo.ui2_domainname = this._domain;
                useinfo.ui2_password = this._password;
                useinfo.ui2_asg_type = 0;
                useinfo.ui2_usecount = 1;
                uint paramErrorIndex;
                uint returncode = NetUseAdd(null, 2, ref useinfo, out paramErrorIndex);
                this._lastError = (int)returncode;
                return (returncode == 0);
            }
            catch (Exception ex)
            {
                this._lastError = Marshal.GetLastWin32Error();
                return false;
            }
        }


        /// <summary>
        /// Ends the connection to the remote resource.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if it succeeds.
        /// </returns>
        private bool Close()
        {
            try
            {
                uint returncode = NetUseDel(null, this._uncPath, 2);
                this._lastError = (int)returncode;
                return (returncode == 0);
            }
            catch(Exception ex)
            {
                this._lastError = Marshal.GetLastWin32Error();
                return false;
            }
        }

        #endregion


        #region [ Dll Imports ]

        [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern NET_API_STATUS NetUseAdd(LPWSTR UncServerName, DWORD Level, ref USE_INFO_2 Buf, out DWORD ParmError);

        [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern NET_API_STATUS NetUseDel(LPWSTR UncServerName, LPWSTR UseName, DWORD ForceCond);
        
        #endregion


        #region [ Structs ]

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct USE_INFO_2
        {
            internal LPWSTR ui2_local;
            internal LPWSTR ui2_remote;
            internal LPWSTR ui2_password;
            internal DWORD ui2_status;
            internal DWORD ui2_asg_type;
            internal DWORD ui2_refcount;
            internal DWORD ui2_usecount;
            internal LPWSTR ui2_username;
            internal LPWSTR ui2_domainname;
        }

        #endregion
    }
}
