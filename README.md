# Fast C# AIS Log Decoder

## Purpose 

This project builds on a series of efforts to increase the speed and efficiency of processing large amounts of raw AIS log data.

## Description

This test project is built as a C# console application.

The console application has four primary processes, controlled via bool settings in [program.cs](https://github.com/mwmckenzie/fast-csharp-ais-log-decoder/blob/main/AisDataProcessing/Program.cs). There is a built-in stopwatch functionality which prints the total run-time to the console after every successful application execution.

## Status

For demonstration and sharing of ideas/concepts only (at least at the current stage of development).

Basic functionality is complete and generally satisfies tests for valid output, as well as generally meeting goals for processing speed.

Application and script structure and syntax is still quite rough and no attempt has been made to produce 'finished' or 'final' code. There is quite a bit of redundant and unoptimized code throughout and is in need of quite a bit of refactoring.

## Design

Arrange pipeline to:
- Flag invalid records on read where validity can be checked without calculation/conversion
- Filter first by invalid records
- Then filter by any other values that do not need conversion or calculation
- Find unique values to convert
- Create lookups for conversion
- Convert values only once
- Replace values in original records with conversions or calculations using hash-table/dictionary retrieval


## Benefits 

### Conversion Calculation Efficiency Improvements

Original methodology
- Full conversion of the log line
- Calculations required were O(n) where n is the number of log lines multiplied by the number of fields that require conversion (cf) multiplied by the number of conversions needed per field (cpf) to achieve desired end state
- Roughly, calculations = n * cf * cpf

Example

        1 bil log lines
        12 fields that require conversion
        10 fields that require two separate conversions (6-bit to binary and binary to decimal, for instance)
        = 120 bil calculations

Improved Methodology
- Improved conversion processing will be O(n) where n is the unique value count
- Speed improves (relative to a 'convert by record' approach) as the ratio of redundent records per conversion type increases (for example, an average of 10, 100, or 1000 records per MMSI or Epoch value)

### Replacement and Filtering Search Efficiency

- Lookup value search will be O(1) search time per replacement or filter
- This is achieved via use of hash-tables/dictionaries for all (appropriate) lookup functions

### In-Stride Database Creation

- A beneficial side-effect of the prior processes are databases created through the use of serialized lookup dictionaries/tables

### Fast-Filtering Can be Appended After Every Stage

- Use the work of the prior stage/process to further filter/refine the data being passed forward to further reduce downstream processing time

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

> MMSI Dictionary Example: 254947000

- Format: MMSI (key)
- Faster version of a simple list
- Allows for easy expansion later if need arises for a lookup by MMSI

### Build Helper Dictionaries 

- Only run as a seperate/stand-alone process for testing and timing observations
- Helper objects contain dictionaries for replacement lookups in later steps/processes
- Generally match the serialized dictionary files, though these can be optimized as needed in the helpers for specific functions/needs

### 'Second Pass' Processing

> Example: 254947000,1,00,0000,wh0Pfp9TI65`Vk7gl2<0t,28/02/2022 00:00:00

- Format: MMSI, Msg ID, Repeat Binary, Binary Segment, Remaing Payload (6-Bit Ascii), Date/Time
- Ingests 'first-pass' processed log lines
- Performs a fast hash-table replacement for the first 6 chars of the payload
- Performs a fast hash-table replacement for the epoch 
- Appends MMSI, Msg ID, Repeat Binary, and converted Binary Segment to the remaining payload characters (removes the initial 6)
- Appends the Date/Time
- Saves new log lines


## Results

### Current Final Output Structure

Format: MMSI, Msg ID, Repeat Binary, Binary Segment, Remaing Payload (6-Bit Ascii), Date/Time

#### Example Extract from Final Log Conversion:

    254947000,1,00,0000,wh0Pfp9TI65`Vk7gl2<0t,28/02/2022 00:00:00
    538007404,1,00,0000,02mPdn@pHIg@=GJon06sd,28/02/2022 00:00:00
    247380270,18,00,0000,008;?q46Ci?;Q3wTUoP06,28/02/2022 00:00:00
    477076700,1,00,0000,00HPdgr`HhuA2dR5n0@06,28/02/2022 00:00:00
    247066850,1,00,0000,P0NPfF98I4r3nfOv02@0H,28/02/2022 00:00:00
    538006934,1,00,0000,1030`c5@IAkooGpa00D1?,28/02/2022 00:00:00

> Note: Only partial payload is left to decode (column 5)

### Test Conditions

#### Two raw AIS Log files
- Log 1: 779k lines
- Log 2: 737k lines
- Total lines: 1.516m lines

### Full Process Time

> Time: approx 3.3 sec

<img src="https://user-images.githubusercontent.com/57882845/192149026-4057834c-0bb4-4466-816e-6bc797642665.png" width="400">
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


