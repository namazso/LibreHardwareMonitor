using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibreHardwareMonitor.PawnIO
{
    internal class LpcIO
    {
        readonly PawnIO _pawnio = new PawnIO();

        public LpcIO()
        {
            var resourceName = $"{nameof(LibreHardwareMonitor)}.Resources.PawnIO.LpcIO.bin";
            var assembly = typeof(LpcIO).Assembly;
            var stream = assembly.GetManifestResourceStream(resourceName);
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            _pawnio.Load(bytes);
        }

        public long Detect(int slot)
        {
            return _pawnio.Execute("ioctl_detect", new long[] { slot }, 1)[0];
        }

        public byte ReadByte(byte register)
        {
            return (byte)_pawnio.Execute("ioctl_read", new long[] { register }, 1)[0];
        }

        public void WriteByte(byte register, byte value)
        {
            _pawnio.Execute("ioctl_write", new long[] { register, value }, 0);
        }

        public void Enter()
        {
            _pawnio.Execute("ioctl_enter", new long[] { }, 0);
        }

        public void Exit()
        {
            _pawnio.Execute("ioctl_exit", new long[] { }, 0);
        }

        public byte ReadPort(ushort port)
        {
            return (byte)_pawnio.Execute("ioctl_pio_read", new long[] { port }, 1)[0];
        }

        public void WritePort(ushort port, byte value)
        {
            _pawnio.Execute("ioctl_pio_write", new long[] { port, value }, 0);
        }

        public bool IsGigabyteControllerEnabled()
        {
            return _pawnio.Execute("ioctl_set_gigabyte_controller", new long[] { -1 }, 1)[0] != 0;
        }

        public bool SetGigabyteControllerEnabled(bool enable)
        {
            return _pawnio.Execute("ioctl_set_gigabyte_controller", new long[] { enable ? 1 : 0 }, 1)[0] != 0;
        }
    }
}
