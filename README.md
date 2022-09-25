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

### Build Reference Dictionaries

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

<img src="https://user-images.githubusercontent.com/57882845/192136175-451b1553-83cc-417e-94f1-80c86b6f66b5.png" width="400">
</img>


### 'First-Pass' Processing Time

<img src="https://user-images.githubusercontent.com/57882845/192135224-77b7f92e-be66-4a9f-adc5-2672e58258e9.png" width="400">
</img>

### Reference Dictionaries Building

#### MMSI Dictionaries

<img src="https://user-images.githubusercontent.com/57882845/192135224-77b7f92e-be66-4a9f-adc5-2672e58258e9.png" width="400">
</img>

#### Epoch Dictionary

<img src="https://user-images.githubusercontent.com/57882845/192135320-6902e18a-cbed-4a4f-aa57-bae1cc6dc2d2.png" width="400">
</img>

#### All Dictionaries

<img src="https://user-images.githubusercontent.com/57882845/192135357-c9667e8f-3801-404a-8151-cacce74a4ca8.png" width="400">
</img>

### Helper Builds

<img src="https://user-images.githubusercontent.com/57882845/192135394-e9677f4b-5eda-4b43-b412-e1ab3d755d14.png" width="400">
</img>


### 'Second-Pass' Processing Time

<img src="https://user-images.githubusercontent.com/57882845/192135412-1735bb5d-4650-40a0-934d-fd4a3fde4373.png" width="400">
</img>



### Overall Processing Time
