using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace HKMCodeInstallerV2
{
    public class NetworkAPI
    {

        // USER_INFO_1 - Strucutre to hold obtained user information
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_INFO_1
        {
            public string usri1_name;
            public string usri1_password;
            public int usri1_password_age;
            public int usri1_priv;
            public string usri1_home_dir;
            public string comment;
            public int usri1_flags;
            public string usri1_script_path;
        }

        // USER_INFO_0 - Structure to hold Just Usernames
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_INFO_0
        {
            public String Username;
        }

        // NetUserAdd - To Add Users to a local machine or Network
        [DllImport("Netapi32.dll")]
        public extern static int NetUserAdd([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, ref USER_INFO_1 buf, int parm_err);

        // NetUserDel - To delete Users from a local machine or Network
        [DllImport("Netapi32.dll")]
        public extern static int NetUserDel([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username);

        // NetUserGetInfo - Returns to a struct Information about the specified user
        [DllImport("Netapi32.dll")]
        public extern static int NetUserGetInfo([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username, int level, out IntPtr bufptr);

        // NetUserSetInfo - Allows us to modify User information
        [DllImport("Netapi32.dll")]
        public extern static int NetUserSetInfo([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username, int level, ref USER_INFO_1 buf, int error);

        // NetUserChangePassword - Allows us to change a users password providing we have it
        [DllImport("Netapi32.dll")]
        public extern static int NetUserChangePassword([MarshalAs(UnmanagedType.LPWStr)] string domainname, [MarshalAs(UnmanagedType.LPWStr)] string username, [MarshalAs(UnmanagedType.LPWStr)] string oldpassword, [MarshalAs(UnmanagedType.LPWStr)] string newpassword);

        // NetUserEnum - Obtains a list of all users on local machine or network
        [DllImport("Netapi32.dll")]
        public extern static int NetUserEnum(string servername, int level, int filter, out IntPtr bufptr, int prefmaxlen, out int entriesread, out int totalentries, out int resume_handle);

        // NetAPIBufferFree - Used to clear the Network buffer after NetUserEnum
        [DllImport("Netapi32.dll")]
        public extern static int NetApiBufferFree(IntPtr Buffer);

        public NetworkAPI()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
