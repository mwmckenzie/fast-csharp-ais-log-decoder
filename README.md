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

### Overall Processing Time
