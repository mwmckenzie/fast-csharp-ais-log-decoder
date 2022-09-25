# Fast C# AIS Log Decoder

## Purpose 

This project builds on a series of efforts to increase the speed and efficiency of processing large amounts of raw AIS log data.

## Description

This test project is built as a C# console application.

The console application has four primary processes, controlled via bool settings in [program.cs](https://github.com/mwmckenzie/fast-csharp-ais-log-decoder/blob/main/AisDataProcessing/Program.cs). There is a built-in stopwatch functionality which prints the total run-time to the console after every successful application execution.

## Status

Basic functionality is complete and generally satisfies tests for valid output, as well as generally meeting goals for processing speed.

## Primary Application Processes

### 'First Pass' Processing

- Filter Out Any Invalid Lines
- Save Only Relevant Processing Info

> Note: Relevant processing info currently only 'payload' and 'epoch'

#### Ingests Raw Log Line
> Example: !AIVDM,1,1,,B,13k8ff0wh0Pfp9TI65`Vk7gl2<0t,0*21,1646006400

#### Exports Only Line Data With Valid Payload and Epoch
> Example: 13cnPQ?P1KPf7DBI5O7CK?vb2<12,1646074221
- Payload checked for min length (28 char)
- Epoch checked for min length (10 char)

### Build Reference Dictionaries

#### Epoch Dictionary

> Example: 1646006400,28/02/2022 00:00:00

- Format: Epoch (key), Day/Month/Year HH:MM:SS

#### MMSI Dictionaries

> MMSI Binary Dictionary Example: 13k8ff0,254947000,1,00,0000

- Format: Binary Segment (key), MMSI, Msg ID, Repeat Binary, Remaining Binary from Segment
- Decodes MMSI, Msg ID
- Extracts Repeat Binary Vals (00, 01, 10, 11)
- Appends leftover binary data

### Build Helper Dictionaries 

- Only run as a seperate/stand-alone process for testing and timing observations
- Helper objects contain dictionaries for replacement lookups in later steps/processes

### 'Second Pass' Processing

- Replace Values from First Pass Results with Dictionary Lookups
- Save


## Results

### Test Conditions

#### Two raw AIS Log files
- Log 1: 779k lines
- Log 2: 737k lines
- Total lines: 1.516m lines

### Full Process Time

> Time: approx 4.4 sec

<img src="https://user-images.githubusercontent.com/57882845/192136175-451b1553-83cc-417e-94f1-80c86b6f66b5.png" width="400">
</img>


### 'First-Pass' Processing Time

> Time: approx 1.9 sec

<img src="https://user-images.githubusercontent.com/57882845/192135224-77b7f92e-be66-4a9f-adc5-2672e58258e9.png" width="400">
</img>

### Reference Dictionaries Building

#### MMSI Dictionaries

> Time: approx 0.2 sec

<img src="https://user-images.githubusercontent.com/57882845/192136605-552a5b77-0509-488e-a6a3-e7f8b772c00c.png" width="400">
</img>

#### Epoch Dictionary

> Time: approx 0.8 sec

<img src="https://user-images.githubusercontent.com/57882845/192135320-6902e18a-cbed-4a4f-aa57-bae1cc6dc2d2.png" width="400">
</img>

#### All Dictionaries

> Time: approx 1.0 sec

<img src="https://user-images.githubusercontent.com/57882845/192135357-c9667e8f-3801-404a-8151-cacce74a4ca8.png" width="400">
</img>

### Helper Builds

> Time: approx 0.1 sec

<img src="https://user-images.githubusercontent.com/57882845/192135394-e9677f4b-5eda-4b43-b412-e1ab3d755d14.png" width="400">
</img>


### 'Second-Pass' Processing Time

> Time: approx 1.4 sec

<img src="https://user-images.githubusercontent.com/57882845/192135412-1735bb5d-4650-40a0-934d-fd4a3fde4373.png" width="400">
</img>


