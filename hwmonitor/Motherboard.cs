using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

class nvml { 
[DllImport("GpuNvidiaPower")]
static public extern void nvmlLibInit();
    [DllImport("GpuNvidiaPower")]
static public extern int getPowerValue();
}


class Motherboard

{
   
    
    /*
     * Sensor IDs we want to log, and how they map to Record's DataPoint type
     */
    static Dictionary<Identifier, Record.DataPoint> IDMap = new Dictionary<Identifier, Record.DataPoint>
    {
        { new Identifier("intelcpu", "0", "temperature", "0"), Record.DataPoint.CPUCore0Temperature },
        { new Identifier("intelcpu", "0", "temperature", "1"), Record.DataPoint.CPUCore1Temperature },
        { new Identifier("intelcpu", "0", "temperature", "2"), Record.DataPoint.CPUCore2Temperature },
        { new Identifier("intelcpu", "0", "temperature", "3"), Record.DataPoint.CPUCore3Temperature },
        { new Identifier("intelcpu", "0", "temperature", "4"), Record.DataPoint.CPUPackageTemperature },
        { new Identifier("nvidiagpu", "0", "temperature", "0"), Record.DataPoint.GPUCoreTemperature },
        { new Identifier("intelcpu", "0", "power", "0"), Record.DataPoint.CPUPackagePower },
        { new Identifier("intelcpu", "0", "power", "1"), Record.DataPoint.CPUCoresPower },
        { new Identifier("intelcpu", "0", "power", "3"), Record.DataPoint.CPUDRAMPower },
    };

    public static Computer computer = new Computer();

    public static void Init()
    {
        computer.Open();
        computer.CPUEnabled = true;
        computer.GPUEnabled = true;
         nvml.nvmlLibInit();
    }

    public static void Update(Record Record)
    {

      Double gpupowervalue = Convert.ToDouble(nvml.getPowerValue())/1000;
        Record.Set(Record.DataPoint.GPUPower,gpupowervalue);
     
        foreach (var hardware in computer.Hardware)
        {
            hardware.Update();

            foreach (var sensor in hardware.Sensors)
            {
                if (!IDMap.ContainsKey(sensor.Identifier))
                {
                    /* we are not interested in this sensor */
                    continue;
                }
                Record.Set(IDMap[sensor.Identifier], sensor.Value);
            }
        }
    }
}
