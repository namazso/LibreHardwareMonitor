using System;

namespace LibreHardwareMonitor.PawnIo
{
    internal class LpcIO
    {
        readonly long[] _singleArgArray = new long[1];
        readonly long[] _doubleArgArray = new long[2];
        readonly PawnIO _pawnIO;

        public LpcIO()
        {
            _pawnIO = PawnIo.PawnIO.LoadModuleFromResource(typeof(LpcIO).Assembly, $"{nameof(LibreHardwareMonitor)}.Resources.PawnIO.LpcIO.bin");
        }

        public long Detect(int slot)
        {
            _singleArgArray[0] = slot;
            return _pawnIO.Execute("ioctl_detect", _singleArgArray, 1)[0];
        }

        public byte ReadByte(byte register)
        {
            _singleArgArray[0] = register;
            return (byte)_pawnIO.Execute("ioctl_read", _singleArgArray, 1)[0];
        }

        public void WriteByte(byte register, byte value)
        {
            _doubleArgArray[0] = register;
            _doubleArgArray[1] = value;
            _pawnIO.Execute("ioctl_write", _doubleArgArray, 0);
        }

        public void Enter()
        {
            _pawnIO.Execute("ioctl_enter", [], 0);
        }

        public void Exit()
        {
            _pawnIO.Execute("ioctl_exit", [], 0);
        }

        public byte ReadPort(ushort port)
        {
            _singleArgArray[0] = port;
            return (byte)_pawnIO.Execute("ioctl_pio_read", _singleArgArray, 1)[0];
        }

        public void WritePort(ushort port, byte value)
        {
            _doubleArgArray[0] = port;
            _doubleArgArray[1] = value;
            _pawnIO.Execute("ioctl_pio_write", _doubleArgArray, 0);
        }

        public bool IsGigabyteControllerEnabled()
        {
            return _pawnIO.Execute("ioctl_set_gigabyte_controller", [-1], 1)[0] != 0;
        }

        public bool SetGigabyteControllerEnabled(bool enable)
        {
            return _pawnIO.Execute("ioctl_set_gigabyte_controller", [enable ? 1 : 0], 1)[0] != 0;
        }
    }
}
