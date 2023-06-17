// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright (C) LibreHardwareMonitor and Contributors.
// Partial Copyright (C) Michael Möller <mmoeller@openhardwaremonitor.org> and Contributors.
// All Rights Reserved.

using System;
using LibreHardwareMonitor.PawnIO;

namespace LibreHardwareMonitor.Hardware.Motherboard.Lpc;

internal class LpcPort
{
    private PawnIO.LpcIO _pioLpc;

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
        _pioLpc = new PawnIO.LpcIO();
        var detected = _pioLpc.Detect(registerPort == 0x2e ? 0 : 1);
        Vendor = (ChipVendor)(detected >> 32);
        ChipIdRevision = (ushort)(detected & 0xFFFF);
    }

    public ushort RegisterPort { get; }

    public ushort ValuePort { get; }

    public ChipVendor Vendor { get; }

    public ushort ChipIdRevision { get; }

    public byte ReadByte(byte register)
    {
        return _pioLpc.ReadByte(register);
    }

    public void WriteByte(byte register, byte value)
    {
        _pioLpc.WriteByte(register, value);
    }

    public ushort ReadWord(byte register)
    {
        return (ushort)((ReadByte(register) << 8) | ReadByte((byte)(register + 1)));
    }

    public void Select(byte logicalDeviceNumber)
    {
        WriteByte(DEVICE_SELECT_REGISTER, logicalDeviceNumber);
    }

    public void Enter()
    {
        _pioLpc.Enter();
    }

    public void Exit()
    {
        _pioLpc.Exit();
    }

    public byte ReadIoPort(ushort port)
    {
        return _pioLpc.ReadPort(port);
    }

    public void WriteIoPort(ushort port, byte value)
    {
        _pioLpc.WritePort(port, value);
    }

    public bool IsGigabyteControllerEnabled()
    {
        return _pioLpc.IsGigabyteControllerEnabled();
    }

    public bool SetGigabyteControllerEnabled(bool enable)
    {
        return _pioLpc.SetGigabyteControllerEnabled(enable);
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
