using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibreHardwareMonitor.PawnIO
{
    internal class PawnIO
    {
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("PawnIOLib.dll", PreserveSig = false)]
        private static extern void pawnio_version(out uint version);

        [DllImport("PawnIOLib.dll", PreserveSig = false)]
        private static extern void pawnio_open(out IntPtr handle);

        [DllImport("PawnIOLib.dll", PreserveSig = false)]
        private static extern void pawnio_load(IntPtr handle, byte[] blob, IntPtr size);

        [DllImport("PawnIOLib.dll", PreserveSig = false)]
        private static extern void pawnio_execute(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string name, long[] in_array, IntPtr in_size, long[] out_array, IntPtr out_size, out IntPtr return_size);

        [DllImport("PawnIOLib.dll", PreserveSig = false)]
        private static extern void pawnio_close(IntPtr handle);

        private readonly IntPtr _handle;

        private static bool TryLoadDll()
        {
            var pawnioPath = Environment.GetEnvironmentVariable("PAWNIO_ROOT");
            if (!string.IsNullOrEmpty(pawnioPath))
            {
                try
                {
                    LoadLibrary(pawnioPath + Path.DirectorySeparatorChar + "PawnIOLib.dll");
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public static uint Version()
        {
            TryLoadDll();
            pawnio_version(out var version);
            return version;
        }

        public PawnIO()
        {
            TryLoadDll();
            pawnio_open(out _handle);
        }

        public PawnIO(byte[] bytes)
        {
            TryLoadDll();
            pawnio_open(out _handle);
            Load(bytes);
        }

        ~PawnIO()
        {
            if (_handle != IntPtr.Zero)
                pawnio_close(_handle);
        }

        public void Load(byte[] bytes)
        {
            pawnio_load(_handle, bytes, (IntPtr)bytes.Length);
        }

        public long[] Execute(string name, long[] input, int outLength)
        {
            var outArray = new long[outLength];
            pawnio_execute(_handle, name, input, (IntPtr)input.Length, outArray, (IntPtr)outArray.Length, out var returnLength);
            Array.Resize(ref outArray, (int)returnLength);
            return outArray;
        }
    }
}
