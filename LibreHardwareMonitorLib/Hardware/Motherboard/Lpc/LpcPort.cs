// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright (C) LibreHardwareMonitor and Contributors.
// Partial Copyright (C) Michael Möller <mmoeller@openhardwaremonitor.org> and Contributors.
// All Rights Reserved.

using System;
using LibreHardwareMonitor.PawnIo;

namespace LibreHardwareMonitor.Hardware.Motherboard.Lpc;

internal class LpcPort
{
    private PawnIo.LpcIO _pawnModule;

    public enum ChipVendor
    {
        Unknown = 0,
        Winbond,
        IT87,
        Smsc
    }

    public LpcPort(ushort registerPort, ushort valuePort)
    {
        RegisterPort = registerPort;
        ValuePort = valuePort;
        if ((registerPort != 0x2e && registerPort != 0x4e) || (valuePort != 0x2f && valuePort != 0x4f))
            throw new ArgumentOutOfRangeException();
        _pawnModule = new PawnIo.LpcIO();
        long detected = _pawnModule.Detect(registerPort == 0x2e ? 0 : 1);
        Vendor = (ChipVendor)(detected >> 32);
        ChipIdRevision = (ushort)(detected & 0xFFFF);
    }

    public ushort RegisterPort { get; }

    public ushort ValuePort { get; }

    public ChipVendor Vendor { get; }

    public ushort ChipIdRevision { get; }

    public byte ReadByte(byte register)
    {
        return _pawnModule.ReadByte(register);
    }

    public void WriteByte(byte register, byte value)
    {
        _pawnModule.WriteByte(register, value);
    }

    public ushort ReadWord(byte register)
    {
        return (ushort)((ReadByte(register) << 8) | ReadByte((byte)(register + 1)));
    }

    public bool TryReadWord(byte register, out ushort value)
    {
        value = ReadWord(register);
        return value != 0xFFFF;
    }

    public void Select(byte logicalDeviceNumber)
    {
        WriteByte(DEVICE_SELECT_REGISTER, logicalDeviceNumber);
    }

    public void Enter()
    {
        _pawnModule.Enter();
    }

    public void Exit()
    {
        _pawnModule.Exit();
    }

    public byte ReadIoPort(ushort port)
    {
        return _pawnModule.ReadPort(port);
    }

    public void WriteIoPort(ushort port, byte value)
    {
        _pawnModule.WritePort(port, value);
    }

    public bool IsGigabyteControllerEnabled()
    {
        return _pawnModule.IsGigabyteControllerEnabled();
    }

    public bool SetGigabyteControllerEnabled(bool enable)
    {
        return _pawnModule.SetGigabyteControllerEnabled(enable);
    }

    public void NuvotonDisableIOSpaceLock()
    {
        byte options = ReadByte(NUVOTON_HARDWARE_MONITOR_IO_SPACE_LOCK);
        // if the i/o space lock is enabled
        if ((options & 0x10) > 0)
        {
            // disable the i/o space lock
            WriteByte(NUVOTON_HARDWARE_MONITOR_IO_SPACE_LOCK, (byte)(options & ~0x10));
        }
    }

    // ReSharper disable InconsistentNaming
    private const byte DEVICE_SELECT_REGISTER = 0x07;
    private const byte NUVOTON_HARDWARE_MONITOR_IO_SPACE_LOCK = 0x28;
    // ReSharper restore InconsistentNaming
}
