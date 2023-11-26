﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace JeepDiag.WPF.DRB
{
    public static class Drb
    {
        public const byte MinTableAddress = 0xF0;
        public const byte MaxTableAddress = 0xFD;
        public static byte[] TableAddresses = Enumerable.Range(MinTableAddress, MaxTableAddress).Select(i => (byte)i).ToArray();

        public const byte MinMemoryAddress = 0x00;
        public const byte MaxMemoryAddress = 0xEF;
        public static byte[] MemoryAddresses = Enumerable.Range(MinMemoryAddress, MaxMemoryAddress).Select(i => (byte)i).ToArray();

        public static class Commands
        {
            public const byte StoredDtcs = 0x10; // Diagnostic Trouble Codes
            public const byte PendingDtcs = 0x11;
            public const byte HighSpeedMode = 0x12;
            public const byte SetupAtm = 0x13; // Actuator Testing Mode
            public const byte ReadDiagnosticData = 0x14;
            public const byte ReadMemoryAddress = 0x15;
            public const byte ReadEcuID = 0x16;
            public const byte ClearDtcs = 0x17;
            public const byte ControlAsdRelay = 0x18;
            public const byte TempIdleSpeed = 0x19; // 77 - FF -> 900 - 2000 RPM
            public const byte SwitchTest = 0x1A;
            public const byte LowSpeedMode = 0xFE;
        }

        public static class Dtc
        {
            public static string DecodeClearDtcResponse(byte[] data)
            {
                if (data.Length < 1)
                    throw new DrbException("SCI-bus error", data);

                if (data.SequenceEqual(ClearDtcError))
                    return "ERROR";
                else if (data.SequenceEqual(ClearDtcSuccess))
                    return "SUCCESS";

                throw new DrbException("Unknown response", data);
            }

            public static ICollection<string> DecodePendingDtcResponse(byte[] data)
            {
                if (data.Length < 3)
                    throw new DrbException("SCI-bus error", data);

                var dtcs = new List<string>();

                if (data[1] == 0 && data[2] == 0)
                    return dtcs;

                for (int i = 0; i < 2; i++)
                    if (DtcMap.TryGetValue(data[i], out var dtc))
                        dtcs.Add(dtc);

                return dtcs;
            }

            public static ICollection<string> DecodeStoredDtcResponse(byte[] data)
            {
                if (data.Length < 3)
                    throw new DrbException("SCI-bus error", data);

                if (CalcStoredDtcChecksum(ref data, data.Length - 1) == data[^1])
                    throw new DrbException("Checksum error", data);

                int dtcCount = data.Length - 3;
                var dtcs = new List<string>(dtcCount);

                if (dtcCount == 0)
                    return dtcs;

                for (int i = 1; i < data.Length - 2; i++)
                    if (DtcMap.TryGetValue(data[i], out var dtc))
                        dtcs.Add(dtc);

                return dtcs;
            }

            private static int CalcStoredDtcChecksum(ref byte[] data, int length)
            {
                byte cs = 0;

                for (byte i = 0; i < length; i++)
                    cs += data[i];

                return cs;
            }

            public static byte[] ClearDtcError = new byte[] { 0xFF };
            public static byte[] ClearDtcSuccess = new byte[] { 0xE0, 0xE0, 0xE0 };

            public static readonly IDictionary<byte, string> DtcMap = new Dictionary<byte, string>()
            {
                { 0x00, "UNRECOGNIZED DTC" },
                { 0x01, "NO CAM SIGNAL AT PCM" },
                { 0x02, "INTERNAL CONTROLLER FAILURE" },
                { 0x03, "LEFT BANK O2 SENSOR STAYS ABOVE CENTER (RICH)" },
                { 0x04, "LEFT BANK O2 SENSOR STAYS BELOW CENTER (LEAN)" },
                { 0x05, "CHARGING SYSTEM VOLTAGE LOW" },
                { 0x06, "CHARGING SYSTEM VOLTAGE HIGH" },
                { 0x07, "TURBO BOOST LIMIT EXCEEDED" },
                { 0x08, "RIGHT BANK O2 SENSOR STAYS ABOVE CENTER (RICH)" },
                { 0x09, "RIGHT BANK O2 SENSOR STAYS BELOW CENTER (LEAN)" },
                { 0x0A, "AUTO SHUTDOWN RELAY CONTROL CIRCUIT" },
                { 0x0B, "GENERATOR FIELD NOT SWITCHING PROPERLY" },
                { 0x0C, "TORQUE CONVERTER CLUTCH SOLENOID / TRANS RELAY CIRCUITS" },
                { 0x0D, "TURBOCHARGER WASTEGATE SOLENOID CIRCUIT" },
                { 0x0E, "LOW SPEED FAN CONTROL RELAY CIRCUIT" },
                { 0x0F, "CRUISE CONTROL SOLENOID CIRCUITS" },
                { 0x10, "A/C CLUTCH RELAY CIRCUIT" },
                { 0x11, "EGR SOLENOID CIRCUIT" },
                { 0x12, "EVAP PURGE SOLENOID CIRCUIT" },
                { 0x13, "INJECTOR #3 CONTROL CIRCUIT" },
                { 0x14, "INJECTOR #2 CONTROL CIRCUIT" },
                { 0x15, "INJECTOR #1 CONTROL CIRCUIT" },
                { 0x16, "INJECTOR #3 PEAK CURRENT NOT REACHED" },
                { 0x17, "INJECTOR #2 PEAK CURRENT NOT REACHED" },
                { 0x18, "INJECTOR #1 PEAK CURRENT NOT REACHED" },
                { 0x19, "IDLE AIR CONTROL MOTOR CIRCUITS" },
                { 0x1A, "THROTTLE POSITION SENSOR VOLTAGE LOW" },
                { 0x1B, "THROTTLE POSITION SENSOR VOLTAGE HIGH" },
                { 0x1C, "THROTTLE BODY TEMP SENSOR VOLTAGE LOW" },
                { 0x1D, "THROTTLE BODY TEMP SENSOR VOLTAGE HIGH" },
                { 0x1E, "COOLANT TEMPERATURE SENSOR VOLTAGE LOW" },
                { 0x1F, "COOLANT TEMPERATURE SENSOR VOLTAGE HIGH" },
                { 0x20, "UPSTREAM O2 SENSOR STAYS AT CENTER" },
                { 0x21, "ENGINE IS COLD TOO LONG" },
                { 0x22, "SKIP SHIFT SOLENOID CIRCUIT" },
                { 0x23, "NO VEHICLE SPEED SENSOR SIGNAL" },
                { 0x24, "MAP SENSOR VOLTAGE LOW" },
                { 0x25, "MAP SENSOR VOLTAGE HIGH" },
                { 0x26, "SLOW CHANGE IN IDLE MAP SENSOR SIGNAL" },
                { 0x27, "NO CHANGE IN MAP FROM START TO RUN" },
                { 0x28, "NO CRANKSHAFT REFERENCE SIGNAL AT PCM" },
                { 0x29, "IGNITION COIL #3 PRIMARY CIRCUIT" },
                { 0x2A, "IGNITION COIL #2 PRIMARY CIRCUIT" },
                { 0x2B, "IGNITION COIL #1 PRIMARY CIRCUIT" },
                { 0x2C, "NO ASD RELAY OUTPUT VOLTAGE AT PCM" },
                { 0x2D, "SYSTEM RICH, L-IDLE ADAPTIVE AT LEAN LIMIT" },
                { 0x2E, "EGR SYSTEM FAILURE" },
                { 0x2F, "BAROMETRIC READ SOLENOID CIRCUIT" },
                { 0x30, "PCM FAILURE SRI MILE NOT STORED" },
                { 0x31, "PCM FAILURE EEPROM WRITE DENIED" },
                { 0x32, "TRANSMISSION 3-4 SHIFT SOLENOID / TRANSMISSION RELAY CIRCUITS" },
                { 0x33, "SECONDARY AIR SOLENOID CIRCUIT" },
                { 0x34, "IDLE SWITCH SHORTED TO GROUND" },
                { 0x35, "IDLE SWITCH OPEN CIRCUIT" },
                { 0x36, "SURGE VALVE SOLENOID CIRCUIT" },
                { 0x37, "INJECTOR #9 CONTROL CIRCUIT" },
                { 0x38, "INJECTOR #10 CONTROL CIRCUIT" },
                { 0x39, "INTAKE AIR TEMPERATURE SENSOR VOLTAGE LOW" },
                { 0x3A, "INTAKE AIR TEMPERATURE SENSOR VOLTAGE HIGH" },
                { 0x3B, "KNOCK SENSOR #1 CIRCUIT" },
                { 0x3C, "BAROMETRIC PRESSURE OUT OF RANGE" },
                { 0x3D, "INJECTOR #4 CONTROL CIRCUIT" },
                { 0x3E, "LEFT BANK UPSTREAM O2 SENSOR SHORTED TO VOLTAGE" },
                { 0x3F, "FUEL SYSTEM RICH, R-IDLE ADAPTIVE AT LEAN LIMIT" },
                { 0x40, "WASTEGATE #2 CIRCUIT" },
                { 0x41, "RIGHT BANK UPSTREAM O2 SENSOR STAYS AT CENTER" },
                { 0x42, "RIGHT BANK UPSTREAM O2 SENSOR SHORTED TO VOLTAGE" },
                { 0x43, "FUEL SYSTEM LEAN, R-IDLE ADAPTIVE AT RICH LIMIT" },
                { 0x44, "PCM FAILURE SPI COMMUNICATIONS" },
                { 0x45, "INJECTOR #5 CONTROL CIRCUIT" },
                { 0x46, "INJECTOR #6 CONTROL CIRCUIT" },
                { 0x47, "BATTERY TEMPERATURE SENSOR VOLTS OUT OF RNG" },
                { 0x48, "NO CMP AT IGNITION / INJ DRIVER MODULE" },
                { 0x49, "NO CKP AT IGNITION/ INJ DRIVER MODULE" },
                { 0x4A, "TRANSMISSION TEMPERATURE SENSOR VOLTAGE LOW" },
                { 0x4B, "TRANSMISSION TEMPERATURE SENSOR VOLTAGE HIGH" },
                { 0x4C, "IGNITION COIL #4 PRIMARY CIRCUIT" },
                { 0x4D, "IGNITION COIL #5 PRIMARY CIRCUIT" },
                { 0x4E, "FUEL SYSTEM LEAN, L-IDLE ADAPTIVE AT RICH LIMIT" },
                { 0x4F, "INJECTOR #7 CONTROL CIRCUIT" },
                { 0x50, "INJECTOR #8 CONTROL CIRCUIT" },
                { 0x51, "FUEL PUMP RESISTOR BYPASS RELAY CIRCUIT" },
                { 0x52, "CRUISE CONTROL POWER RELAY OR 12V DRIVER CIRCUIT" },
                { 0x53, "KNOCK SENSOR #2 CIRCUIT" },
                { 0x54, "FLEX FUEL SENSOR VOLTS HIGH" },
                { 0x55, "FLEX FUEL SENSOR VOLTS LOW" },
                { 0x56, "CRUISE CONTROL SWITCH ALWAYS HIGH" },
                { 0x57, "CRUISE CONTROL SWITCH ALWAYS LOW" },
                { 0x58, "MANIFOLD TUNE VALVE SOLENOID CIRCUIT" },
                { 0x59, "NO BUS MESSAGES" },
                { 0x5A, "A/C PRESSURE SENSOR VOLTS HIGH" },
                { 0x5B, "A/C PRESSURE SENSOR VOLTS LOW" },
                { 0x5C, "LOW SPEED FAN CONTROL RELAY CIRCUIT" },
                { 0x5D, "HIGH SPEED CONDENSER FAN CTRL RELAY CIRCUIT" },
                { 0x5E, "CNG TEMPERATURE SENSOR VOLTAGE LOW" },
                { 0x5F, "CNG TEMPERATURE SENSOR VOLTAGE HIGH" },
                { 0x60, "NO CCD/PCI BUS MESSAGES FROM TCM" },
                { 0x61, "NO CCD/PCI BUS MESSAGE FROM BCM" },
                { 0x62, "CNG PRESSURE SENSOR VOLTAGE HIGH" },
                { 0x63, "CNG PRESSURE SENSOR VOLTAGE LOW" },
                { 0x64, "LOSS OF FLEX FUEL CALIBRATION SIGNAL" },
                { 0x65, "FUEL PUMP RELAY CONTROL CIRCUIT" },
                { 0x66, "LEFT BANK UPSTREAM O2 SENSOR SLOW RESPONSE" },
                { 0x67, "LEFT BANK UPSTREAM O2 SENSOR HEATER FAILURE" },
                { 0x68, "DOWNSTREAM O2 SENSOR UNABLE TO SWITCH RICH/LEAN" },
                { 0x69, "DOWNSTREAM O2 SENSOR HEATER FAILURE" },
                { 0x6A, "MULTIPLE CYLINDER MISFIRE" },
                { 0x6B, "CYLINDER #1 MISFIRE" },
                { 0x6C, "CYLINDER #2 MISFIRE" },
                { 0x6D, "CYLINDER #3 MISFIRE" },
                { 0x6E, "CYLINDER #4 MISFIRE" },
                { 0x6F, "TOO LITTLE SECONDARY AIR" },
                { 0x70, "CATALYTIC CONVERTER EFFICIENCY FAILURE" },
                { 0x71, "EVAP PURGE FLOW MONITOR FAILURE" },
                { 0x72, "P/N SWITCH STUCK IN PARK OR IN GEAR" },
                { 0x73, "POWER STEERING SWITCH FAILURE" },
                { 0x74, "DESIRED FUEL TIMING ADVANCE NOT REACHED" },
                { 0x75, "LOST FUEL INJECTION TIMING SIGNAL" },
                { 0x76, "LEFT BANK FUEL SYSTEM RICH" },
                { 0x77, "LEFT BANK FUEL SYSTEM LEAN" },
                { 0x78, "RIGHT BANK FUEL SYSTEM RICH" },
                { 0x79, "RIGHT BANK FUEL SYSTEM LEAN" },
                { 0x7A, "RIGHT BANK UPSTREAM O2 SENSOR SLOW RESPONSE" },
                { 0x7B, "RIGHT BANK DOWNSTREAM O2 SENSOR SLOW RESPONSE" },
                { 0x7C, "RIGHT BANK UPSTREAM O2 SENSOR HEATER FAILURE" },
                { 0x7D, "RIGHT BANK DOWNSTREAM O2 SENSOR HEATER FAILURE" },
                { 0x7E, "DOWNSTREAM O2 SENSOR SHORTED TO VOLTAGE" },
                { 0x7F, "RIGHT BANK DOWNSTREAM O2 SENSOR SHORTED TO VOLTAGE" },
                { 0x80, "CLOSED LOOP TEMPERATURE NOT REACHED" },
                { 0x81, "LEFT BANK DOWNSTREAM O2 SENSOR STAYS AT CENTER" },
                { 0x82, "RIGHT BANK DOWNSTREAM O2 SENSOR STAYS AT CENTER" },
                { 0x83, "LEAN OPERATION AT WIDE OPEN THROTTLE" },
                { 0x84, "TPS VOLTAGE DOES NOT AGREE WITH MAP" },
                { 0x85, "TIMING BELT SKIPPED 1 TOOTH OR MORE" },
                { 0x86, "NO 5 VOLTS TO A/C PRESSURE SENSOR" },
                { 0x87, "NO 5 VOLTS TO MAP SENSOR" },
                { 0x88, "NO 5 VOLTS TO TPS" },
                { 0x89, "EATX CONTROLLER DTC PRESENT" },
                { 0x8A, "TARGET IDLE NOT REACHED" },
                { 0x8B, "HIGH SPEED RADIATOR FAN CONTROL RELAY CIRCUIT" },
                { 0x8C, "DIESEL EGR SYSTEM FAILURE" },
                { 0x8D, "GOVERNOR PRESSURE NOT EQUAL TO TARGET @ 15 - 20 PSI" },
                { 0x8E, "GOVERNOR PRESSURE ABOVE 3 PSI IN GEAR WITH 0 MPH" },
                { 0x8F, "STARTER RELAY CONTROL CIRCUIT" },
                { 0x90, "DOWNSTREAM O2 SENSOR SHORTED TO GROUND" },
                { 0x91, "VACUUM LEAK FOUND (IAC FULLY SEATED)" },
                { 0x92, "5 VOLT SUPPLY, OUTPUT LOW" },
                { 0x93, "DOWNSTREAM O2 SENSOR SHORTED TO VOLTAGE" },
                { 0x94, "TORQUE CONVERTER CLUTCH, NO RPM DROP AT LOCKUP" },
                { 0x95, "FUEL LEVEL SENDING UNIT VOLTS LOW" },
                { 0x96, "FUEL LEVEL SENDING UNIT VOLTS HIGH" },
                { 0x97, "FUEL LEVEL UNIT NO CHANGE OVER MILES" },
                { 0x98, "BRAKE SWITCH STUCK PRESSED OR RELEASED" },
                { 0x99, "BATTERY TEMPERATURE SENSOR VOLTS LOW" },
                { 0x9A, "BATTERY TEMPERATURE SENSOR VOLTS HIGH" },
                { 0x9B, "LEFT BANK UPSTREAM O2 SENSOR SHORTED TO GROUND" },
                { 0x9C, "DOWNSTREAM O2 SENSOR SHORTED TO GROUND" },
                { 0x9D, "INTERMITTENT LOSS OF CMP OR CKP" },
                { 0x9E, "TOO MUCH SECONDARY AIR" },
                { 0x9F, "DOWNSTREAM O2 SENSOR SLOW RESPONSE" },
                { 0xA0, "EVAP LEAK MONITOR SMALL LEAK DETECTED" },
                { 0xA1, "EVAP LEAK MONITOR LARGE LEAK DETECTED" },
                { 0xA2, "NO TEMPERATURE RISE SEEN FROM INTAKE HEATERS" },
                { 0xA3, "WAIT TO START LAMP CIRCUIT" },
                { 0xA4, "TRANSMISSION TEMPERATURE SENSOR, NO TEMPERATURE RISE AFTR START" },
                { 0xA5, "3-4 SHIFT SOLENOID, NO RPM DROP @ 3-4 SHIFT" },
                { 0xA6, "LOW OUTPUT SPEED SENSOR RPM, ABOVE 15 MPH" },
                { 0xA7, "GOVERNOR PRESSURE SENSOR VOLTS LOW" },
                { 0xA8, "GOVERNOR PRESSURE SENSOR VOLTS HIGH" },
                { 0xA9, "GOVERNOR PRESSURE SENSOR OFFSET VOLTS LOW OR HIGH" },
                { 0xAA, "PCM NOT PROGRAMMED" },
                { 0xAB, "GOVERNOR PRESSURE SOLENOID CONTROL / TRANSMISSION RELAY CIRCUITS" },
                { 0xAC, "DOWNSTREAM O2 SENSOR STUCK AT CENTER" },
                { 0xAD, "TRANSMISSION 12 VOLT SUPPLY RELAY CONTROL CIRCUIT" },
                { 0xAE, "CYLINDER #5 MIS-FIRE" },
                { 0xAF, "CYLINDER #6 MIS-FIRE" },
                { 0xB0, "CYLINDER #7 MIS-FIRE" },
                { 0xB1, "CYLINDER #8 MIS-FIRE" },
                { 0xB2, "CYLINDER #9 MIS-FIRE" },
                { 0xB3, "CYLINDER #10 MIS-FIRE" },
                { 0xB4, "RIGHT BANK CATALYST EFFICIENCY FAILURE" },
                { 0xB5, "REAR BANK UPSTREAM O2 SENSOR SHORTED TO GROUND" },
                { 0xB6, "REAR BANK DOWNSTREAM O2 SENSOR SHORTED TO GROUND" },
                { 0xB7, "LEAK DETECTION PUMP SOLENOID CIRCUIT" },
                { 0xB8, "LEAK DETECT PUMP SWITCH OR MECHANICAL FAULT" },
                { 0xB9, "AUXILIARY 5 VOLT SUPPLY OUTPUT LOW" },
                { 0xBA, "MISFIRE ADAPTIVE NUMERATOR AT LIMIT" },
                { 0xBB, "EVAP LEAK MONITOR PINCHED HOSE FOUND" },
                { 0xBC, "O/D SWITCH PRESSED (LOW) MORE THAN 5 MIN" },
                { 0xBD, "DOWNSTREAM O2 SENSOR HEATER FAILURE" },
                { 0xBE, "INVALID DTC" },
                { 0xBF, "INVALID DTC" },
                { 0xC0, "INVALID DTC" },
                { 0xC1, "INVALID DTC" },
                { 0xC2, "INVALID DTC" },
                { 0xC3, "INVALID DTC" },
                { 0xC4, "INVALID DTC" },
                { 0xC5, "HIGH SPEED RADIATOR FAN GROUND CONTROL RELAY CIRCUIT" },
                { 0xC6, "ONE OF THE IGNITION COILS DRAWS TOO MUCH CURRENT" },
                { 0xC7, "AW4 TRANSMISSION SHIFT SOLENOID B FUNCTIONAL FAILURE" },
                { 0xC8, "RADIATOR TEMPERATURE SENSOR VOLTS LOW" },
                { 0xC9, "RADIATOR TEMPERATURE SENSOR VOLTS HIGH" },
                { 0xCA, "NO I/P CLUSTER CCD/PCI BUS MESSAGES RECEIVED" },
                { 0xCB, "AW4 TRANSMISSION INTERNAL FAILURE (ROM CHECK)" },
                { 0xCC, "UPSTREAM O2 SENSOR SLOW RESPONSE" },
                { 0xCD, "UPSTREAM O2 SENSOR HEATER FAILURE" },
                { 0xCE, "UPSTREAM O2 SENSOR SHORTED TO VOLTAGE" },
                { 0xCF, "UPSTREAM O2 SENSOR SHORTED TO GROUND" },
                { 0xD0, "NO CAM SYNC SIGNAL AT PCM" },
                { 0xD1, "GLOW PLUG RELAY CONTROL CIRCUIT" },
                { 0xD2, "HIGH SPEED CONDENSER FAN CONTROL RELAY CIRCUIT" },
                { 0xD3, "AW4 TRANSMISSION SHIFT SOLENOID B (2-3) SHORTED TO VOLTAGE (12V)" },
                { 0xD4, "EGR POSITION SENSOR VOLTS LOW" },
                { 0xD5, "EGR POSITION SENSOR VOLTS HIGH" },
                { 0xD6, "NO 5 VOLTS TO EGR POSITION SENSOR" },
                { 0xD7, "EGR POSITION SENSOR RATIONALITY FAILURE" },
                { 0xD8, "IGNITION COIL #6 PRIMARY CIRCUIT" },
                { 0xD9, "INTAKE MANIFOLD SHORT RUNNER SOLENOID CIRCUIT" },
                { 0xDA, "AIR ASSIST INJECTION SOLENOID CIRCUIT" },
                { 0xDB, "CATALYST TEMPERATURE SENSOR VOLTS HIGH" },
                { 0xDC, "CATALYST TEMPERATURE SENSOR VOLTS LOW" },
                { 0xDD, "EATX RPM PULSE PERFORMANCE CONDITION" },
                { 0xDE, "NO BUS MESSAGE RECEIVED FROM COMPANION MODULE" },
                { 0xDF, "MIL FAULT IN COMPANION MODULE" },
                { 0xE0, "COOLANT TEMPERATURE SENSOR PERFORMANCE" },
                { 0xE1, "NO MIC BUS MESSAGE" },
                { 0xE2, "NO SKIM BUS MESSAGE RECEIVED" },
                { 0xE3, "IGNITION COIL #7 PRIMARY CIRCUIT" },
                { 0xE4, "IGNITION COIL #8 PRIMARY CIRCUIT" },
                { 0xE5, "PCV SOLENOID CIRCUIT" },
                { 0xE6, "TRANSMISSION FAN RELAY CIRCUIT" },
                { 0xE7, "TCC OR O/D SOLENOID PERFORMANCE" },
                { 0xE8, "WRONG OR INVALID KEY MESSAGE RECEIVED FROM SKIM" },
                { 0xE9, "PWM O2 HEATER PERFORMANCE" },
                { 0xEA, "AW4 TRANSMISSION SOLENOID A 1-2/3-4 OR TCC SOLENOID C FUNCTIONAL FAIL" },
                { 0xEB, "AW4 TRANSMISSION TCC SOLENOID C SHORTED TO GROUND" },
                { 0xEC, "AW4 TRANSMISSION TCC SOLENOID C SHORTED TO VOLTAGE (12V)" },
                { 0xED, "AW4 TRANSMISSION BATTERY VOLTS SENSE LOW" },
                { 0xEE, "AW4 TRANSMISSION BATTERY VOLTS SENSE HIGH" },
                { 0xEF, "AISIN AW4 TRANSMISSION DTC PRESENT" },
                { 0xF0, "INVALID DTC" },
                { 0xF1, "INVALID DTC" },
                { 0xF2, "INVALID DTC" },
                { 0xF3, "INVALID DTC" },
                { 0xF4, "INVALID DTC" },
                { 0xF5, "INVALID DTC" },
                { 0xF6, "INVALID DTC" },
                { 0xF7, "INVALID DTC" },
                { 0xF8, "INVALID DTC" },
                { 0xF9, "INVALID DTC" },
                { 0xFA, "INVALID DTC" },
                { 0xFB, "INVALID DTC" },
                { 0xFC, "INVALID DTC" },
                { 0xFD, "DUAL BYTE MODE (P-CODES)" },
                { 0xFE, "END OF DTC LIST" },
                { 0xFF, "NOT ALLOWED" },
            };
        }

        //public static class ReadDiagnosticDataArgs
        //{
        //    public const byte AmbientAirTemp = 0x01;
        //    public const byte O2SensorVoltage = 0x02;
        //    public const byte End = 0x03;
        //    public const byte End = 0x04;
        //    public const byte AmbientAirTemp = 0x01;
        //    public const byte AmbientAirTemp = 0x01;
        //    public const byte AmbientAirTemp = 0x01;
        //    public const byte AmbientAirTemp = 0x01;
        //    public const byte AmbientAirTemp = 0x01;
        //    public const byte AmbientAirTemp = 0x01;
        //    public const byte AmbientAirTemp = 0x01;
        //    public const byte AmbientAirTemp = 0x01;
        //    public const byte AmbientAirTemp = 0x01;
        //    public const byte AmbientAirTemp = 0x01;
        //    public const byte AmbientAirTemp = 0x01;
        //}

        //public static class AtmArgs
        //{
        //    public const byte StopTests = 0x00;

        //    public const byte FualInjector1 = 0x04;
        //    public const byte FualInjector2 = 0x05;
        //    public const byte FualInjector3 = 0x06;
        //    public const byte FualInjector4 = 0x1D;
        //    public const byte FualInjector5 = 0x1E;
        //    public const byte FualInjector6 = 0x1F;
        //}
    }
}
