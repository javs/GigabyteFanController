// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright (C) LibreHardwareMonitor and Contributors.
// Partial Copyright (C) Michael Möller <mmoeller@openhardwaremonitor.org> and Contributors.
// All Rights Reserved.

using GigabyteFanController.LibreHardwareMonitor;
using LibreHardwareMonitor.Hardware;
using System.Runtime.InteropServices;

namespace GigabyteFanController
{

    /// <summary>
    /// This is a controller present on some Gigabyte motherboards for both Intel and AMD,
    /// that is known to only control the 2nd ITE Super IO chip fans.
    /// This class can disable it so that the regular IT87XX code can drive the fans.
    /// </summary>
    internal class GigabyteController
    {
        private readonly Model _model;
        private bool? _initialState;

        public GigabyteController(Model model) => _model = model;

        public bool Enable(bool enabled)
        {
            switch (_model)
            {
                case Model.B550_AORUS_PRO:
                    return AMDEnable(enabled);
                default:
                    return false;
            }
        }

        private bool AMDEnable(bool enabled)
        {
            if (!Ring0.WaitPciBusMutex(10))
                return false;

            // see D14F3x https://www.amd.com/system/files/TechDocs/55072_AMD_Family_15h_Models_70h-7Fh_BKDG.pdf 
            uint AmdIsaBridgeAddress = Ring0.GetPciAddress(0x0, 0x14, 0x3);

            const uint IOorMemoryPortDecodeEnableRegister = 0x48;
            const int MemoryRangePortEnableBit = 5;
            const uint PCIMemoryAddressforLPCTargetCycles = 0x60;
            const uint ROMAddressRange2 = 0x6C;
            const uint ControllerBaseAddress = 0xFF000900;

            // Enable MMIO for BIOS Flash
            Ring0.ReadPciConfig(AmdIsaBridgeAddress, IOorMemoryPortDecodeEnableRegister, out uint current);
            Ring0.WritePciConfig(AmdIsaBridgeAddress, IOorMemoryPortDecodeEnableRegister, current | (0x1 << MemoryRangePortEnableBit));
            Ring0.WritePciConfig(AmdIsaBridgeAddress, PCIMemoryAddressforLPCTargetCycles, 0xFF01FF00);
            Ring0.WritePciConfig(AmdIsaBridgeAddress, ROMAddressRange2, 0xFFFFFF01);

            var result = _Enable(enabled, new IntPtr(ControllerBaseAddress));
            
            // Restore previous value
            Ring0.WritePciConfig(AmdIsaBridgeAddress, IOorMemoryPortDecodeEnableRegister, current);

            Ring0.ReleasePciBusMutex();

            return result;
        }

        private bool _Enable(bool enabled, IntPtr PCIMMIOBaseAddress)
        {
            // Map PCI memory to this process memory
            if (!InpOut.Open())
                return false;

            IntPtr mapped = InpOut.MapMemory(PCIMMIOBaseAddress, ControllerAddressRange, out IntPtr handle);

            // Update Controller State
            if (mapped == IntPtr.Zero)
                return false;

            if (!_initialState.HasValue)
                _initialState = Convert.ToBoolean(Marshal.ReadByte(mapped, ControllerEnableRegister));

            Marshal.WriteByte(mapped, ControllerEnableRegister, Convert.ToByte(enabled));

            InpOut.UnmapMemory(handle, mapped);

            return true;
        }

        /// <summary>
        /// Restore settings back to initial values
        /// </summary>
        public void Restore()
        {
            if (_initialState.HasValue)
                Enable(_initialState.Value);
        }

        const int ControllerEnableRegister = 0x47;
        const uint ControllerAddressRange = 0xFF;
    }
}
