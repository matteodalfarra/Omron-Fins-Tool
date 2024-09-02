# Omron FINS Communication Tool

## Description

The **Omron FINS Communication Tool** is a software developed in C# using [.NET 6.0](https://dotnet.microsoft.com/it-it/download/dotnet/6.0) and the **[POmronFinsTCP.Net](https://github.com/ping9719/OmronFinsTCP.Net)** library. This tool allows you to interact with Omron PLCs via the FINS (Factory Interface Network Service) communication protocol. The project consists of four main parts, each providing specific functionalities to facilitate communication and management of Omron PLCs.

The project is continuously evolving, and the source code is regularly updated to improve functionality and support new data types.

## Features

1. **PLC Connection**
   - In the first section of the application, you can specify the IP address and port of the PLC to establish a connection. This feature allows for quick configuration of the connection with the control device.

2. **Node Reading**
   - The second part of the tool allows you to read nodes from the PLC. This feature is useful for obtaining information about available nodes and checking their status.

3. **Value Sending**
   - In the third section, you can remotely send values to the PLC. This feature enables you to modify PLC data directly from the application, facilitating the sending of commands or updates to values.

4. **Log Viewing**
   - The final part of the application provides an interface to view logs generated during the use of the tool. This helps to monitor operations and diagnose any issues during communication with the PLC.

## Requirements

- **.NET 6.0**: Ensure that .NET 6.0 is installed on your system to run the tool.
- **[POmronFinsTCP.Net](https://www.nuget.org/packages/POmronFinsTCP.Net)**: The library used for FINS communication must be included in the project.
